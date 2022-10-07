using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Reflection;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Vision;
using Microsoft.Data.Analysis;
using Microsoft.ML.TensorFlow;

using DxMLEngine.Utilities;
using DxMLEngine.Attributes;
using DxMLEngine.Functions;
using DxMLEngine.Objects;
using Tensorflow.Keras.Engine;
using DxMlEngine.Features.ImageRecognition;

namespace DxMLEngine.Features.ImageRecognition
{
	[Feature]
	internal class ProductInspection
	{		
		[Feature]
		public static void BuildClassificationModel(string inDir, string outDir, string fileName)
		{
			var mlContext = new MLContext();
            
            var dataView = InputProductImageData(ref mlContext, inDir, FileFormat.Jpeg);
            var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.3);
            var trainData = trainTestData.TrainSet;
            var validData = mlContext.Data.TrainTestSplit(trainTestData.TestSet).TrainSet;
            var testData = mlContext.Data.TrainTestSplit(trainTestData.TestSet).TestSet;

            var model = TrainClassificationModel(ref mlContext, trainData, validData, inDir);
            var metrics = EvaluateClassificationModel(ref mlContext, model, testData);
			
            Log.Info($"Multiclass Classification Metrics");

            Console.WriteLine($"MicroAccuracy       : {metrics.MicroAccuracy:F3}");
            Console.WriteLine($"MacroAccuracy       : {metrics.MacroAccuracy:F3}");
            Console.WriteLine($"LogLoss             : {metrics.LogLoss:F3}");
            Console.WriteLine($"PerClassLogLoss     : {metrics.PerClassLogLoss:F3}");
            Console.WriteLine($"LogLossReduction    : {metrics.LogLossReduction:F3}\n");

            Console.WriteLine($"TopKAccuracy        : {metrics.TopKAccuracy:F3}");
            Console.WriteLine($"TopKPredictionCount : {metrics.TopKPredictionCount:F3}");
            Console.WriteLine($"TopKAccuracyForAllK : {metrics.TopKAccuracyForAllK:F3}");
            Console.WriteLine($"\n{metrics.ConfusionMatrix.GetFormattedConfusionTable()}");
			
			Console.Write("\nTry model (Y/N): ");
            if (Console.ReadLine() == "Y")
                TryClassificationModel(ref mlContext, model);		
			
			Console.Write("\nSave model (Y/N): ");
            if (Console.ReadLine() == "Y")
                SaveClassificationModel(ref mlContext, model, dataView!, outDir, fileName);        
		}
		
		[Feature]
		public static void InspectProductImage(string inFileModel, string inDirData, string outDir, string fileName)
		{
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFileModel, out _);
			
			var inputData = InputProductImageData(ref mlContext, inDirData, FileFormat.Jpeg);
			
			var productImages = mlContext.Data.CreateEnumerable<ProductImage>(inputData, false).ToArray();
            var predictions = ConsumeClassificationModel(ref mlContext, model, productImages); 
			
			Log.Info($"Product Image Inspection");
			for (int i = 0; i < productImages.Length; i++)
			{
				Console.WriteLine($"ImagePath      : {productImages[i].ImagePath:F3}");
				Console.WriteLine($"ImageBytes     : {productImages[i].Image:F3}");
				Console.WriteLine($"ActualLabel    : {productImages[i].Category:F3}");
				Console.WriteLine($"PredictedLabel : {productImages[i].Category:F3}\n");
			}
			
			OutputProductImageInspection(outDir, fileName, productImages, predictions, FileFormat.Csv);
		}	
		
		#region DATA CONNECTION
		
		private static IDataView? InputProductImageData(ref MLContext mlContext, string location, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Jpeg)
            {
                var paths = Directory.GetFiles(location, searchPattern: "*", searchOption: SearchOption.AllDirectories);
                
                var images = new List<ProductImage>();
                foreach (var path in paths) 
                {                    
                    var category = Path.GetFileName(path).Split("-")[0];
                    var image = new ProductImage();
                    image.ImagePath = path;
                    image.Category = category;

                    images.Add(image);
                }

                var dataView = mlContext.Data.LoadFromEnumerable(images);
                return mlContext.Data.ShuffleRows(dataView);
            }
            return null;
        }
		
		private static void OutputProductImageInspection(string location, string fileName, ProductImage[] productImages, ProductImagePrediction[] predictions, FileFormat fileFormat)
		{
            if (fileFormat == FileFormat.Csv) 
            {
                var dataFrame = new DataFrame( new List<DataFrameColumn>() 
                {
                    new StringDataFrameColumn("ImagePath"),
                    new StringDataFrameColumn("ImageBytes"),
                    new StringDataFrameColumn("ActualCategory"),
                    new StringDataFrameColumn("PredictedCategory"),
                });

                                for (int i = 0; i < productImages.Length; i++)
                {
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("ImagePath", $"\"{productImages[i].ImagePath}\""),
                        new KeyValuePair<string, object?>("ImageBytes", $"\"{productImages[i].Image}\""),
                        new KeyValuePair<string, object?>("ActualCategory", $"\"{productImages[i].Category}\""),
                        new KeyValuePair<string, object?>("PredictedCategory", $"\"{productImages[i].Category}\""),
                    };
                                        
                    dataFrame.Append(dataRow, inPlace: true);
                }

                var path = $"{location}\\Dataset @{fileName} #-------------- .csv";
                DataFrame.WriteCsv(dataFrame, path, header: true, separator: ',', encoding: Encoding.UTF8);

                var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
                File.Move(path, path.Replace("#--------------", $"#{timestamp}"));
            }			
		}

        #endregion DATA CONNECTION

        #region TRAINING & TESTING

        private static ITransformer TrainClassificationModel(ref MLContext mlContext, IDataView trainData, IDataView validData, string inDir) 
        {
            var pipeline = mlContext.Transforms.Conversion
                .MapValueToKey(inputColumnName: "Category", outputColumnName: "Label")
                .Append(mlContext.Transforms.LoadRawImageBytes(
                    inputColumnName: "ImagePath", outputColumnName: "Image", imageFolder: inDir))
                .Append(mlContext.MulticlassClassification.Trainers.ImageClassification(
                    new ImageClassificationTrainer.Options()
                    {
                        FeatureColumnName = "Image",
                        LabelColumnName = "Label",
                        ValidationSet = validData,
                        Arch = ImageClassificationTrainer.Architecture.ResnetV2101,
                        MetricsCallback = (metrics) => Console.WriteLine(metrics),
                        TestOnTrainSet = false,
                        ReuseTrainSetBottleneckCachedValues = true,
                        ReuseValidationSetBottleneckCachedValues = true
                    }))
                .Append(mlContext.Transforms.Conversion
                    .MapKeyToValue(inputColumnName: null, outputColumnName: "PredictedLabel")); ;

            var model = pipeline.Fit(trainData);
            return model;
        }

        private static MulticlassClassificationMetrics EvaluateClassificationModel(ref MLContext mlContext, ITransformer model, IDataView testData)
        {
            var predictions = model.Transform(testData);
            var metrics = mlContext.MulticlassClassification.Evaluate(predictions);

            return metrics;
        }

        #endregion TRAINING & TESTING

        #region MODEL CONSUMPTION

        private static void TryClassificationModel(ref MLContext mlContext, ITransformer model)
        {
            Console.Write("\nEnter input file path: ");
            var newInDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(newInDir))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var ourDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(ourDir))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output file name: ");
            var fileName = Console.ReadLine()?.Replace(" ", "");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("file name is null or empty");

            var inputData = InputProductImageData(ref mlContext, newInDir, FileFormat.Jpeg);
            if (inputData == null)
                throw new ArgumentNullException("inputData == null");

            var productImages = mlContext.Data.CreateEnumerable<ProductImage>(inputData, false).ToArray();
            var predictions = ConsumeClassificationModel(ref mlContext, model, productImages);

            Log.Info($"Product Image Inspection");
            for (int i = 0; i < productImages.Length; i++)
            {
                Console.WriteLine($"ImagePath      : {productImages[i].ImagePath:F3}");
                Console.WriteLine($"ImageBytes     : {productImages[i].Image:F3}");
                Console.WriteLine($"ActualLabel    : {productImages[i].Category:F3}");
                Console.WriteLine($"PredictedLabel : {productImages[i].Category:F3}\n");
            }

            OutputProductImageInspection(ourDir, fileName, productImages, predictions, FileFormat.Csv);
        }

        private static ProductImagePrediction[] ConsumeClassificationModel(ref MLContext mlContext, ITransformer model, ProductImage[] productImages)
        {
            var predEngine = mlContext.Model.CreatePredictionEngine<ProductImage, ProductImagePrediction>(model);
            var productImagePredictions = (
                from productImage in productImages
                let prediction = predEngine.Predict(productImage)
                select prediction).ToArray();

            return productImagePredictions;
        }

        private static void SaveClassificationModel(ref MLContext mlContext, ITransformer model, IDataView dataView, string location, string fileName)
        {
            var path = $"{location}\\Model @{fileName} .zip";
            mlContext.Model.Save(model, dataView.Schema, path);
        }

        #endregion MODEL CONSUMPTION
	}
}

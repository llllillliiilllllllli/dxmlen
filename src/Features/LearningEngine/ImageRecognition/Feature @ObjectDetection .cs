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

namespace DxMLEngine.Features.ImageRecognition 
{
	[Feature]
	internal class ObjectDetection
	{		
		[Feature]
		public static void BuildClassificationModel(string inFileModel, string inDirData, string outDir, string fileName)
		{
			var mlContext = new MLContext();
            
            var dataView = InputObjectImageData(ref mlContext, inDirData, FileFormat.Jpeg);
            var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.3);
            var trainData = trainTestData.TrainSet;
            var validData = mlContext.Data.TrainTestSplit(trainTestData.TestSet).TrainSet;
            var testData = mlContext.Data.TrainTestSplit(trainTestData.TestSet).TestSet;

            var model = TrainClassificationModel(ref mlContext, trainData, validData, inFileModel, inDirData);
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
		public static void InspectObjectImage(string inFileModel, string inDirData, string outDir, string fileName)
		{
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFileModel, out _);
			
			var inputData = InputObjectImageData(ref mlContext, inDirData, FileFormat.Jpeg);
			
			var objectImages = mlContext.Data.CreateEnumerable<ObjectImage>(inputData, false).ToArray();
            var predictions = ConsumeClassificationModel(ref mlContext, model, objectImages); 
			
			Log.Info($"Object Detection");
			for (int i = 0; i < objectImages.Length; i++)
			{
				Console.WriteLine($"ImagePath      : {objectImages[i].ImagePath:F3}");
				Console.WriteLine($"ActualLabel    : {objectImages[i].Category:F3}");
				Console.WriteLine($"PredictedLabel : {objectImages[i].Category:F3}\n");
			}
			
			OutputObjectImageDetection(outDir, fileName, objectImages, predictions, FileFormat.Csv);
		}	
		
		#region DATA CONNECTION
		
		private static IDataView? InputObjectImageData(ref MLContext mlContext, string location, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Jpeg)
            {
                var paths = Directory.GetFiles(location, searchPattern: "*", searchOption: SearchOption.AllDirectories);
                
                var images = new List<ObjectImage>();
                foreach (var path in paths) 
                {                    
                    var category = Path.GetFileName(path).Split("-")[0];
                    var image = new ObjectImage();
                    image.ImagePath = path;
                    image.Category = category;

                    images.Add(image);
                }

                var dataView = mlContext.Data.LoadFromEnumerable(images);
                return mlContext.Data.ShuffleRows(dataView);
            }
            return null;
        }
		
		private static void OutputObjectImageDetection(string location, string fileName, ObjectImage[] objectImages, ObjectImagePrediction[] predictions, FileFormat fileFormat)
		{
            if (fileFormat == FileFormat.Csv) 
            {
                var dataFrame = new DataFrame( new List<DataFrameColumn>() 
                {
                    new StringDataFrameColumn("ImagePath"),
                    new StringDataFrameColumn("ActualCategory"),
                    new StringDataFrameColumn("PredictedCategory"),
                });

                                for (int i = 0; i < objectImages.Length; i++)
                {
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("ImagePath", $"\"{objectImages[i].ImagePath}\""),
                        new KeyValuePair<string, object?>("ActualCategory", $"\"{objectImages[i].Category}\""),
                        new KeyValuePair<string, object?>("PredictedCategory", $"\"{objectImages[i].Category}\""),
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

        private static ITransformer TrainClassificationModel(ref MLContext mlContext, IDataView trainData, IDataView validData, string inFileModel, string inDir) 
        {
            var pipeline = mlContext.Transforms.LoadImages(
                inputColumnName: "ImagePath", outputColumnName: "Input", imageFolder: inDir)
                .Append(mlContext.Transforms.ResizeImages(
                    inputColumnName: "Input", outputColumnName: "Input", imageWidth: 224, imageHeight: 224))
                .Append(mlContext.Transforms.ExtractPixels(
                    outputColumnName: "Input", interleavePixelColors: true, offsetImage: 117))
                .Append(mlContext.Model.LoadTensorFlowModel(inFileModel)
                    .ScoreTensorFlowModel(
                        inputColumnNames: new[] { "Input" }, outputColumnNames: new[] { "softmax2_pre_activation" }, 
                        addBatchDimensionInput: true))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(
                    inputColumnName: "Label", outputColumnName: "LabelKey"))
                .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(
                    labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(
                    inputColumnName: null, outputColumnName: "PredictedLabel"))
                .AppendCacheCheckpoint(mlContext);

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

            var inputData = InputObjectImageData(ref mlContext, newInDir, FileFormat.Jpeg);
            if (inputData == null)
                throw new ArgumentNullException("inputData == null");

            var objectImages = mlContext.Data.CreateEnumerable<ObjectImage>(inputData, false).ToArray();
            var predictions = ConsumeClassificationModel(ref mlContext, model, objectImages);

            Log.Info($"Object Detection");
            for (int i = 0; i < objectImages.Length; i++)
            {
                Console.WriteLine($"ImagePath      : {objectImages[i].ImagePath:F3}");
                Console.WriteLine($"ActualLabel    : {objectImages[i].Category:F3}");
                Console.WriteLine($"PredictedLabel : {objectImages[i].Category:F3}\n");
            }

            OutputObjectImageDetection(ourDir, fileName, objectImages, predictions, FileFormat.Csv);
        }

        private static ObjectImagePrediction[] ConsumeClassificationModel(ref MLContext mlContext, ITransformer model, ObjectImage[] objectImages)
        {
            var predEngine = mlContext.Model.CreatePredictionEngine<ObjectImage, ObjectImagePrediction>(model);
            var objectImagePredictions = (
                from objectImage in objectImages
                let prediction = predEngine.Predict(objectImage)
                select prediction).ToArray();

            return objectImagePredictions;
        }

        private static void SaveClassificationModel(ref MLContext mlContext, ITransformer model, IDataView dataView, string location, string fileName)
        {
            var path = $"{location}\\Model @{fileName} .zip";
            mlContext.Model.Save(model, dataView.Schema, path);
        }

        #endregion MODEL CONSUMPTION
	}
}

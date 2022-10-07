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

namespace DxMLEngine.Features.Recognition
{
	[Feature]
	internal class ObjectDetection
	{
        private const string ObjectDetectionGuide =
            "\nInstruction:\n" +
            "\t......................................................................................................\n" +
            "\t......................................................................................................\n" +
            "\t......................................................................................................\n" +
            "\tSource: ..............................................................................................\n" +
            "\tSource: ..............................................................................................\n" +
            "\tSource: ..............................................................................................";

        [Feature]
		public static void BuildClassificationModel(string inFileModel, string inFileTrain, string inFileTest, string inDirImages, string outDir, string fileName)
		{
			var mlContext = new MLContext();
            
            var trainData = InputObjectImageFromFile(ref mlContext, inFileTrain, FileFormat.Tsv);
            var testData = InputObjectImageFromFile(ref mlContext, inFileTest, FileFormat.Tsv);

            var model = TrainClassificationModel(ref mlContext, trainData!, inFileModel, inDirImages);
            var metrics = EvaluateClassificationModel(ref mlContext, model, testData!);
			
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
                SaveClassificationModel(ref mlContext, model, trainData!, outDir, fileName);        
		}
		
		[Feature]
		public static void InspectObjectImage(string inFileModel, string inFileData, string outDir, string fileName)
		{
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFileModel, out _);
			
			var inputData = InputObjectImageFromFile(ref mlContext, inFileData, FileFormat.Tsv);
			
			var objectImages = mlContext.Data.CreateEnumerable<ObjectImage>(inputData, false).ToArray();
            var predictions = ConsumeClassificationModel(ref mlContext, model, objectImages); 
			
			Log.Info($"Object Detection");
			for (int i = 0; i < objectImages.Length; i++)
			{
				Console.WriteLine($"ImagePath      : {objectImages[i].ImagePath:F3}");
				Console.WriteLine($"ActualLabel    : {objectImages[i].Category:F3}");
				Console.WriteLine($"PredictedLabel : {predictions[i].Category:F3}\n");
			}
			
			OutputObjectImageDetection(outDir, fileName, objectImages, predictions, FileFormat.Csv);
		}	
		
		#region DATA CONNECTION
		
		private static IDataView? InputObjectImageFromFile(ref MLContext mlContext, string path, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Tsv)
            {
                var dataView = mlContext.Data.LoadFromTextFile<ObjectImage>(path, hasHeader: false, separatorChar: '\t');
                return dataView;
            }

            if (fileFormat == FileFormat.Jpg)
            {
                var objectImages = new List<ObjectImage>();
                var imagePaths = DataFrame.LoadCsv(path, header: true, separator: ',', encoding: Encoding.UTF8);
                for (int i = 0; i < imagePaths.Rows.Count; i++)
                {
                    var imagePath = Convert.ToString(imagePaths.Columns["ImagePath"][i]);  
                    var category = Convert.ToString(imagePaths.Columns["Category"][i]);  

                    if (imagePath == null) continue;
                    if (!Path.IsPathFullyQualified(imagePath)) continue;
                    if (Path.GetExtension(imagePath) != ".jpg") continue;

                    var image = new ObjectImage();
                    image.ImagePath = imagePath;
                    image.Category = category;

                    objectImages.Add(image);
                }
                return mlContext.Data.LoadFromEnumerable(objectImages);
            }

            if (fileFormat == FileFormat.Jpg)
            {
                var objectImages = new List<ObjectImage>();
                var imagePaths = DataFrame.LoadCsv(path, header: true, separator: ',', encoding: Encoding.UTF8);
                for (int i = 0; i < imagePaths.Rows.Count; i++)
                {
                    var imagePath = Convert.ToString(imagePaths.Columns["ImagePath"][i]);
                    var category = Convert.ToString(imagePaths.Columns["Category"][i]);

                    if (imagePath == null) continue;
                    if (!Path.IsPathFullyQualified(imagePath)) continue;
                    if (Path.GetExtension(imagePath) != ".jpg") continue;

                    var image = new ObjectImage();
                    image.ImagePath = imagePath;
                    image.Category = category;

                    objectImages.Add(image);
                }
                return mlContext.Data.LoadFromEnumerable(objectImages);
            }

            return null;
        }

        private static IDataView? InputObjectImageFromFolder(ref MLContext mlContext, string location, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Jpg)
            {
                var paths = Directory.GetFiles(location, searchPattern: "*", searchOption: SearchOption.AllDirectories);

                var images = new List<ProductImage>();
                foreach (var path in paths)
                {
                    if (Path.GetExtension(path) != ".jpg") continue;

                    var category = Path.GetFileName(path);
                    var image = new ProductImage();
                    image.ImagePath = path;
                    image.Category = category;

                    images.Add(image);
                }

                var dataView = mlContext.Data.LoadFromEnumerable(images);
                return dataView;
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
                        new KeyValuePair<string, object?>("PredictedCategory", $"\"{predictions[i].Category}\""),
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

        private static ITransformer TrainClassificationModel(ref MLContext mlContext, IDataView trainData, string inFileModel, string inDirData) 
        {
            var pipeline = mlContext.Transforms.LoadImages(
                inputColumnName: "ImagePath", outputColumnName: "input", imageFolder: inDirData)
                .Append(mlContext.Transforms.ResizeImages(
                    inputColumnName: "input", outputColumnName: "input", imageWidth: 224, imageHeight: 224))
                .Append(mlContext.Transforms.ExtractPixels(
                    outputColumnName: "input", interleavePixelColors: true, offsetImage: 117))
                .Append(mlContext.Model.LoadTensorFlowModel(inFileModel)
                    .ScoreTensorFlowModel(
                        inputColumnNames: new[] { "input" }, 
                        outputColumnNames: new[] { "softmax2_pre_activation" }, 
                        addBatchDimensionInput: true))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(
                    inputColumnName: "Category", outputColumnName: "Label"))
                .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(
                    labelColumnName: "Label", featureColumnName: "softmax2_pre_activation"))
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
            var inInDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(inInDir))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var outDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(outDir))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output file name: ");
            var fileName = Console.ReadLine()?.Replace(" ", "");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("file name is null or empty");

            var inputData = InputObjectImageFromFile(ref mlContext, inInDir, FileFormat.Jpg);
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

            OutputObjectImageDetection(outDir, fileName, objectImages, predictions, FileFormat.Csv);
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

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Vision;
using Microsoft.Data.Analysis;
using Microsoft.ML.TensorFlow;

using DxMLEngine.Utilities;
using DxMLEngine.Attributes;
using DxMLEngine.Functions;
using DxMLEngine.Objects;
using System.Text.RegularExpressions;
using Tensorflow.Operations.Initializers;
using System.Drawing.Imaging;

namespace DxMLEngine.Features.Recognition
{
	[Feature]
	internal class OxxnObjectDetection
    {
        private const string OxxnObjectDetectionGuide =
            "\nInstruction:\n" +
            "\t......................................................................................................\n" +
            "\t......................................................................................................\n" +
            "\t......................................................................................................\n" +
            "\tSource: ..............................................................................................\n" +
            "\tSource: ..............................................................................................\n" +
            "\tSource: ..............................................................................................";

        [Feature]
		public static void BuildObjectDetectionModel(string inFileModel, string inDirImages, string outDir, string fileName)
		{
			var mlContext = new MLContext();
            
            var trainData = mlContext.Data.LoadFromEnumerable(new List<ImageNet>());
            var testData = InputImageNetFromFolder(ref mlContext, inDirImages, FileFormat.Jpg);

            var model = TrainObjectDetectionModel(ref mlContext, trainData, inFileModel);
            var metrics = EvaluateObjectDetectionModel(ref mlContext, model, testData!);
			
            Log.Info($"Onnx Object Detection Metrics");

            Console.WriteLine($"Label-1     : {metrics[0], 6:F3}");
            Console.WriteLine($"Label-2     : {metrics[1], 6:F3}");
            Console.WriteLine($"Label-3     : {metrics[2], 6:F3}");
            Console.WriteLine($"Label-4     : {metrics[3], 6:F3}");
            Console.WriteLine($"Label-5     : {metrics[4], 6:F3}");
            Console.WriteLine($"Label-6     : {metrics[5], 6:F3}");
            Console.WriteLine($"Label-7     : {metrics[6], 6:F3}");
            Console.WriteLine($"Label-8     : {metrics[7], 6:F3}");
            Console.WriteLine($"Label-9     : {metrics[8], 6:F3}");
            Console.WriteLine($"Label-10    : {metrics[9], 6:F3}");
            Console.WriteLine($"Label-11    : {metrics[10],6:F3}");
            Console.WriteLine($"Label-12    : {metrics[11],6:F3}");
            Console.WriteLine($"Label-13    : {metrics[12],6:F3}");
            Console.WriteLine($"Label-14    : {metrics[13],6:F3}");
            Console.WriteLine($"Label-15    : {metrics[14],6:F3}");
            Console.WriteLine($"Label-16    : {metrics[15],6:F3}");
            Console.WriteLine($"Label-17    : {metrics[16],6:F3}");
            Console.WriteLine($"Label-18    : {metrics[17],6:F3}");
            Console.WriteLine($"Label-19    : {metrics[18],6:F3}");
            Console.WriteLine($"Label-20    : {metrics[19],6:F3}\n");

            Console.Write("\nTry model (Y/N): ");
            if (Console.ReadLine() == "Y")
                TryObjectDetectionModel(ref mlContext, model);		
			
			Console.Write("\nSave model (Y/N): ");
            if (Console.ReadLine() == "Y")
                SaveObjectDetectionModel(ref mlContext, model, trainData!, outDir, fileName);        
		}
		
		[Feature]
		public static void DetectObjectsInImage(string inFileModel, string inDirData, string outDir, string fileName)
		{
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFileModel, out _);
			
			var inputData = InputImageNetFromFolder(ref mlContext, inDirData, FileFormat.Jpg);
			
			var imageNets = mlContext.Data.CreateEnumerable<ImageNet>(inputData, false).ToArray();
            var predictions = ConsumeObjectDetectionModel(ref mlContext, model, imageNets);

            Log.Info($"Onnx Object Recognition");
            for (int i = 0; i < imageNets.Length; i++)
            {
                Console.WriteLine($"ImagePath   : {predictions[i].ImagePath}");
                Console.WriteLine($"ImageLabel  : {predictions[i].Label}");
                Console.WriteLine($"Label-1     : {predictions[i].PredictedLabels![0], 6:F3}");
                Console.WriteLine($"Label-2     : {predictions[i].PredictedLabels![1], 6:F3}");
                Console.WriteLine($"Label-3     : {predictions[i].PredictedLabels![2], 6:F3}");
                Console.WriteLine($"Label-4     : {predictions[i].PredictedLabels![3], 6:F3}");
                Console.WriteLine($"Label-5     : {predictions[i].PredictedLabels![4], 6:F3}");
                Console.WriteLine($"Label-6     : {predictions[i].PredictedLabels![5], 6:F3}");
                Console.WriteLine($"Label-7     : {predictions[i].PredictedLabels![6], 6:F3}");
                Console.WriteLine($"Label-8     : {predictions[i].PredictedLabels![7], 6:F3}");
                Console.WriteLine($"Label-9     : {predictions[i].PredictedLabels![8], 6:F3}");
                Console.WriteLine($"Label-10    : {predictions[i].PredictedLabels![9], 6:F3}");
                Console.WriteLine($"Label-11    : {predictions[i].PredictedLabels![10],6:F3}");
                Console.WriteLine($"Label-12    : {predictions[i].PredictedLabels![11],6:F3}");
                Console.WriteLine($"Label-13    : {predictions[i].PredictedLabels![12],6:F3}");
                Console.WriteLine($"Label-14    : {predictions[i].PredictedLabels![13],6:F3}");
                Console.WriteLine($"Label-15    : {predictions[i].PredictedLabels![14],6:F3}");
                Console.WriteLine($"Label-16    : {predictions[i].PredictedLabels![15],6:F3}");
                Console.WriteLine($"Label-17    : {predictions[i].PredictedLabels![16],6:F3}");
                Console.WriteLine($"Label-18    : {predictions[i].PredictedLabels![17],6:F3}");
                Console.WriteLine($"Label-19    : {predictions[i].PredictedLabels![18],6:F3}");
                Console.WriteLine($"Label-20    : {predictions[i].PredictedLabels![19],6:F3}\n");
            }
            OutputAnnotatedImages(outDir, fileName, predictions, FileFormat.Jpg);
        }	
		
		#region DATA CONNECTION
		
		private static IDataView? InputImageNetFromFile(ref MLContext mlContext, string path, FileFormat fileFormat)
        {
            throw new NotImplementedException();
        }

        private static IDataView? InputImageNetFromFolder(ref MLContext mlContext, string location, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Jpg)
            {
                var paths = Directory.GetFiles(location, searchPattern: "*", searchOption: SearchOption.AllDirectories);

                var images = new List<ImageNet>();
                foreach (var path in paths)
                {
                    if (Path.GetExtension(path) != ".jpg") continue;

                    var label = Regex.Match(Path.GetFileName(path), @"[@][\w\d]+").Value.Replace("@", "");
                    var image = new ImageNet();
                    image.ImagePath = path;
                    image.Label = label;

                    images.Add(image);
                }

                var dataView = mlContext.Data.LoadFromEnumerable(images);
                return dataView;
            }

            return null;
        }

        private static void OutputAnnotatedImages(string location, string fileName, ImageNetPrediction[] predictions, FileFormat fileFormat)
		{
            if (fileFormat == FileFormat.Jpg)
            {
                var images = DrawBoundingBoxes(predictions);
                for (int i = 0; i < images.Length; i++) 
                {
                    var path = $"{location}\\Image @{fileName}{i+1} #-------------- .jpg";
                    images[i].Save(path, ImageFormat.Jpeg);

                    var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
                    File.Move(path, path.Replace("#--------------", $"#{timestamp}"));
                }
            }

            if (fileFormat == FileFormat.Csv) 
            {
                throw new NotImplementedException();
            }			
		}

        #endregion DATA CONNECTION

        #region TRAINING & TESTING

        private static ITransformer TrainObjectDetectionModel(ref MLContext mlContext, IDataView trainData, string inFileModel) 
        {
            var pipeline = mlContext.Transforms
                .LoadImages(inputColumnName: "ImagePath", outputColumnName: "image", imageFolder: "")
                .Append(mlContext.Transforms.ResizeImages(
                    inputColumnName: "image", outputColumnName: "image",
                    imageWidth: ImageNetSpecification.imageWidth,
                    imageHeight: ImageNetSpecification.imageHeight))
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "image"))
                .Append(mlContext.Transforms.ApplyOnnxModel(modelFile: inFileModel,
                    inputColumnNames: new[] { TinyYoloSpecification.InputTensorName },
                    outputColumnNames: new[] { TinyYoloSpecification.OutputTensorName }));

            var model = pipeline.Fit(trainData);
            return model;
        }

        private static float[] EvaluateObjectDetectionModel(ref MLContext mlContext, ITransformer model, IDataView testData)
        {
            var predictions = model.Transform(testData);
            var metrics = predictions.GetColumn<float[]>(TinyYoloSpecification.OutputTensorName).First();

            return metrics;
        }

        #endregion TRAINING & TESTING

        #region MODEL CONSUMPTION

        private static void TryObjectDetectionModel(ref MLContext mlContext, ITransformer model)
        {
            Console.Write("\nEnter input folder path: ");
            var inDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(inDir))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var outDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(outDir))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output file name: ");
            var fileName = Console.ReadLine()?.Replace(" ", "");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("file name is null or empty");

            var inputData = InputImageNetFromFolder(ref mlContext, inDir, FileFormat.Jpg);
            if (inputData == null)
                throw new ArgumentNullException("inputData == null");

            var imageNets = mlContext.Data.CreateEnumerable<ImageNet>(inputData, false).ToArray();
            var predictions = ConsumeObjectDetectionModel(ref mlContext, model, imageNets);

            Log.Info($"Onnx Object Recognition");
            for (int i = 0; i < imageNets.Length; i++)
            {
                Console.WriteLine($"ImagePath   : {predictions[i].ImagePath}");
                Console.WriteLine($"ImageLabel  : {predictions[i].Label}");
                Console.WriteLine($"Label-1     : {predictions[i].PredictedLabels![0], 6:F3}");
                Console.WriteLine($"Label-2     : {predictions[i].PredictedLabels![1], 6:F3}");
                Console.WriteLine($"Label-3     : {predictions[i].PredictedLabels![2], 6:F3}");
                Console.WriteLine($"Label-4     : {predictions[i].PredictedLabels![3], 6:F3}");
                Console.WriteLine($"Label-5     : {predictions[i].PredictedLabels![4], 6:F3}");
                Console.WriteLine($"Label-6     : {predictions[i].PredictedLabels![5], 6:F3}");
                Console.WriteLine($"Label-7     : {predictions[i].PredictedLabels![6], 6:F3}");
                Console.WriteLine($"Label-8     : {predictions[i].PredictedLabels![7], 6:F3}");
                Console.WriteLine($"Label-9     : {predictions[i].PredictedLabels![8], 6:F3}");
                Console.WriteLine($"Label-10    : {predictions[i].PredictedLabels![9], 6:F3}");
                Console.WriteLine($"Label-11    : {predictions[i].PredictedLabels![10],6:F3}");
                Console.WriteLine($"Label-12    : {predictions[i].PredictedLabels![11],6:F3}");
                Console.WriteLine($"Label-13    : {predictions[i].PredictedLabels![12],6:F3}");
                Console.WriteLine($"Label-14    : {predictions[i].PredictedLabels![13],6:F3}");
                Console.WriteLine($"Label-15    : {predictions[i].PredictedLabels![14],6:F3}");
                Console.WriteLine($"Label-16    : {predictions[i].PredictedLabels![15],6:F3}");
                Console.WriteLine($"Label-17    : {predictions[i].PredictedLabels![16],6:F3}");
                Console.WriteLine($"Label-18    : {predictions[i].PredictedLabels![17],6:F3}");
                Console.WriteLine($"Label-19    : {predictions[i].PredictedLabels![18],6:F3}");
                Console.WriteLine($"Label-20    : {predictions[i].PredictedLabels![19],6:F3}\n");
            }

            OutputAnnotatedImages(outDir, fileName, predictions, FileFormat.Jpg);
        }

        private static ImageNetPrediction[] ConsumeObjectDetectionModel(ref MLContext mlContext, ITransformer model, ImageNet[] imageNets)
        {
            var inputDataView = mlContext.Data.LoadFromEnumerable(imageNets);
            var outputDataView = model.Transform(inputDataView);
            var predictions = mlContext.Data.CreateEnumerable<ImageNetPrediction>(outputDataView, false);

            return predictions.ToArray();
        }

        private static void SaveObjectDetectionModel(ref MLContext mlContext, ITransformer model, IDataView dataView, string location, string fileName)
        {
            var path = $"{location}\\Model @{fileName} .zip";
            mlContext.Model.Save(model, dataView.Schema, path);
        }

        #endregion MODEL CONSUMPTION

        #region VISUALIZATION

        private static YoloBoundingBox[] LocateBoundingBoxes(ImageNetPrediction prediction)
        {
            if (prediction.PredictedLabels == null)
                throw new ArgumentNullException("prediction.PredictedLabels == null");

            var boundingBoxes = YoloParser.ParseOutputs(prediction.PredictedLabels);
            var filteredBoxes = YoloParser.FilterOverlappingBoxes(boundingBoxes, 5, 0.5F);

            return filteredBoxes;
        }

        private static Image[] DrawBoundingBoxes(ImageNetPrediction[] predictions)
        {
            var images = new List<Image>();
            for (int i = 0; i < predictions.Length; i++)
            {
                var prediction = predictions[i];

                var boundingBoxes = LocateBoundingBoxes(prediction);

                if (prediction.ImagePath == null) continue;
                var image = Image.FromFile(prediction.ImagePath);

                var originalWidth = image.Width;
                var originalHeight = image.Height;

                foreach (var box in boundingBoxes)
                {
                    var x = (uint)Math.Max(box.Dimensions!.X, 0);
                    var y = (uint)Math.Max(box.Dimensions!.Y, 0);
                    var width = (uint)Math.Min(originalWidth - x, box.Dimensions.Width);
                    var height = (uint)Math.Min(originalHeight - y, box.Dimensions.Height);

                    x = (uint)originalWidth * x / ImageNetSpecification.imageWidth;
                    y = (uint)originalHeight * y / ImageNetSpecification.imageHeight;
                    width = (uint)originalWidth * width / ImageNetSpecification.imageWidth;
                    height = (uint)originalHeight * height / ImageNetSpecification.imageHeight;

                    var text = $"{box.Label} ({Math.Round((decimal)box.Confidence! * 100, 2)}%)";

                    using (var thumbnail = Graphics.FromImage(image))
                    {
                        thumbnail.CompositingQuality = CompositingQuality.HighQuality;
                        thumbnail.SmoothingMode = SmoothingMode.HighQuality;
                        thumbnail.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        var font = new Font("Roboto", 12, FontStyle.Regular);
                        var size = thumbnail.MeasureString(text, font);
                        var fontBrush = new SolidBrush(Color.Black);
                        var atPoint = new Point((int)x, (int)y - (int)size.Height - 1);

                        var boxColor = (Color)Convert.ChangeType(box.Color, typeof(Color))!;
                        var pen = new Pen(boxColor, 3.2f);
                        var colorBrush = new SolidBrush(boxColor);

                        thumbnail.FillRectangle(colorBrush, (int)x, (int)(y - size.Height - 1), (int)size.Width, (int)size.Height);
                        thumbnail.DrawString(text, font, fontBrush, atPoint);
                        thumbnail.DrawRectangle(pen, x, y, width, height);
                    }
                }
                images.Add(image);
            }
            return images.ToArray();
        }

        #endregion VISUALIZATION 
    }
}

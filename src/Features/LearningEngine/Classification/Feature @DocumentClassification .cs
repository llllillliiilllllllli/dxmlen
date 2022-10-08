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
using Microsoft.Data.Analysis;

using DxMLEngine.Attributes;
using DxMLEngine.Utilities;
using DxMLEngine.Objects;

namespace DxMLEngine.Features.Classification
{
    [Feature]
    internal class DocumentClassification
    {
        private const string DocumentClassificationGuide =
            "\nInstruction:\n" +
            "\t......................................................................................................\n" +
            "\t......................................................................................................\n" +
            "\t......................................................................................................\n" +
            "\tSource: ..............................................................................................\n" +
            "\tSource: ..............................................................................................\n" +
            "\tSource: ..............................................................................................";

        [Feature]
        public static void BuildClassificationModel(string inFile, string outDir, string fileName)
        {
            var mlContext = new MLContext(seed: 0);

            var dataView = InputDocumentFromFile(ref mlContext, inFile, FileFormat.Txt);

            var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            var trainData = trainTestData.TrainSet;
            var testData = trainTestData.TestSet;

            var model = TrainClassificationModel(ref mlContext, trainData);
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
        public static void ClassifyDocument(string inFileModel, string inFileData, string outDir, string fileName)
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFileModel, out _);

            var inputData = InputDocumentFromFile(ref mlContext, inFileData, FileFormat.Txt);

            var documents = mlContext.Data.CreateEnumerable<Document>(inputData, false).ToArray();
            var predictions = ConsumeClassificationModel(ref mlContext, model, documents);

            Log.Info($"Document Analysis");
            for (int i = 0; i < predictions.Length; i++)
            {
                Console.WriteLine($"Id               : {predictions[i].Id}");
                Console.WriteLine($"Title            : {predictions[i].Title}");
                Console.WriteLine($"Description      : {predictions[i].Description}");
                Console.WriteLine($"ActualSubject    : {predictions[i].Subject}");
                Console.WriteLine($"PredictedSubject : {predictions[i].Prediction}");
            }

            OutputDocumentClassification(outDir, fileName, predictions, FileFormat.Csv);
        }

        #region DATA CONNECTION

        private static IDataView? InputDocumentFromFile(ref MLContext mlContext, string path, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Txt)
            {
                var dataView = mlContext.Data.LoadFromTextFile<Document>(path, hasHeader: true, separatorChar: '\t');
                return dataView;
            }

            if (fileFormat == FileFormat.Csv)
            {
                throw new NotImplementedException();
            }

            if (fileFormat == FileFormat.Json)
            {
                throw new NotImplementedException();
            }

            return null;
        }

        private static void OutputDocumentClassification(string location, string fileName, DocumentPrediction[] predictions, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Txt)
            {
                throw new NotImplementedException();
            }

            if (fileFormat == FileFormat.Csv)
            {
                var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
                    new StringDataFrameColumn("Id"),
                    new StringDataFrameColumn("Title"),
                    new StringDataFrameColumn("Description"),
                    new StringDataFrameColumn("ActualSubject"),
                    new StringDataFrameColumn("PredictedSubject"),
                });

                for (int i = 0; i < predictions.Length; i++)
                {
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("Id",               $"\"{predictions[i].Id}\""),
                        new KeyValuePair<string, object?>("Title",            $"\"{predictions[i].Title?.Replace("\"", "\"\"")}\""),
                        new KeyValuePair<string, object?>("Description",      $"\"{predictions[i].Description?.Replace("\"", "\"\"")}\""),
                        new KeyValuePair<string, object?>("ActualSubject",    $"\"{predictions[i].Subject}\""),
                        new KeyValuePair<string, object?>("PredictedSubject", $"\"{predictions[i].Prediction}\""),
                    };
                                        
                    dataFrame.Append(dataRow, inPlace: true);
                }

                var path = $"{location}\\Dataset @{fileName} #-------------- .csv";
                DataFrame.WriteCsv(dataFrame, path, header: true, separator: ',', encoding: Encoding.UTF8);

                var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
                File.Move(path, path.Replace("#--------------", $"#{timestamp}"));
            }

            if (fileFormat == FileFormat.Json)
            {
                throw new NotImplementedException();
            }
        }

        #endregion DATA CONNECTION

        #region TRAINING & TESTING

        private static ITransformer TrainClassificationModel(ref MLContext mlContext, IDataView trainData)
        {
            var pipeline = mlContext.Transforms.Conversion
                .MapValueToKey(inputColumnName: "Subject", outputColumnName: "Label")
                .Append(mlContext.Transforms.Text.FeaturizeText(
                    inputColumnName: "Title", outputColumnName: "TitleFeaturized"))
                .Append(mlContext.Transforms.Text.FeaturizeText(
                    inputColumnName: "Description", outputColumnName: "DescriptionFeaturized"))
                .Append(mlContext.Transforms.Concatenate(
                    "Features", "TitleFeaturized", "DescriptionFeaturized"))
                .AppendCacheCheckpoint(mlContext)
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                    labelColumnName: "Label", featureColumnName: "Features"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(
                    inputColumnName: null, outputColumnName: "PredictedLabel"));

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
            var inFile = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(inFile))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var outDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(outDir))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output file name: ");
            var fileName = Console.ReadLine()?.Replace(" ", "");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("file name is null or empty");

            var inputData = InputDocumentFromFile(ref mlContext, inFile, FileFormat.Txt);
            if (inputData == null)
                throw new ArgumentNullException("inputData == null");

            var documents = mlContext.Data.CreateEnumerable<Document>(inputData, false).ToArray();
            var predictions = ConsumeClassificationModel(ref mlContext, model, documents);

            Log.Info($"Document Analysis");
            for (int i = 0; i < documents.Length; i++)
            {
                Console.WriteLine($"Id               : {documents[i].Id}");
                Console.WriteLine($"Title            : {documents[i].Title}");
                Console.WriteLine($"Description      : {documents[i].Description}");
                Console.WriteLine($"ActualSubject    : {documents[i].Subject}");
                Console.WriteLine($"PredictedSubject : {predictions[i].Subject}");
            }

            OutputDocumentClassification(outDir, fileName, predictions, FileFormat.Csv);
        }

        private static DocumentPrediction[] ConsumeClassificationModel(ref MLContext mlContext, ITransformer model, Document[] documents)
        {
            var predEngine = mlContext.Model.CreatePredictionEngine<Document, DocumentPrediction>(model);
            var predictions = (
                from document in documents
                let prediction = predEngine.Predict(document)
                select prediction).ToArray();

            return predictions;
        }

        private static void SaveClassificationModel(ref MLContext mlContext, ITransformer model, IDataView dataView, string location, string fileName)
        {
            var path = $"{location}\\Model @{fileName} .zip";
            mlContext.Model.Save(model, dataView.Schema, path);
        }

        #endregion MODEL CONSUMPTION
    }
}

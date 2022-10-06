using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Data;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.Data.Analysis;

using DxMLEngine.Attributes;
using DxMLEngine.Utilities;
using System.Windows.Forms;

namespace DxMLEngine.Features.DocumentClassification
{
    [Feature]
    internal class DocumentClassification
    {
        #region INSTRUCTION

        private const string DocumentClassificationInstruction =
            "\nInstruction:\n" +
            "\tThis program performs classification on documents by analyzing multiple text fields.\n" +
            "\tText vectors such as titles, abstracts, summaries, descriptions are featurized as inputs.\n" +
            "\tOuputs contain document classified to categories which can be accomplished by verious methods.\n" +
            "\tSdcaMaximumEntropy trains model with coordinate descent method where scores are class propability.\n" +
            "\tMetrics for assessing model effectiveness consist of log loss, micro and macro accuracy.\n" +

            "\tSource: https://learn.microsoft.com/en-us/dotnet/api/microsoft.ml.trainers.sdcamulticlasstrainerbase-1?view=ml-dotnet\n" +
            "\tSource: https://learn.microsoft.com/en-us/dotnet/api/microsoft.ml.data.multiclassclassificationmetrics?view=ml-dotnet\n" +
            "\tSource: https://en.wikipedia.org/wiki/Multiclass_classification\n" +
            "\tSource: https://jmlr.org/papers/volume14/shalev-shwartz13a/shalev-shwartz13a.pdf\n" +
            "\tSource: https://arxiv.org/abs/2008.05756";

        #endregion INSTRUCTION

        [Feature(instruction: DocumentClassificationInstruction)]
        public static void BuildDocumentModel(string inFile, string outDir, string fileName)
        {
            var mlContext = new MLContext(seed: 0);

            var dataView = InputDocumentData(ref mlContext, inFile, FileFormat.Txt);

            var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            var trainData = trainTestData.TrainSet;
            var testData = trainTestData.TestSet;

            var model = TrainDocumentModel(ref mlContext, trainData);
            var metrics = EvaluateDocumentModel(ref mlContext, model, testData);

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

            Console.Write("\nConsume model (Y/N): ");
            if (Console.ReadLine() == "Y")
            {
                Console.Write("\nEnter input file path: ");
                var newInFile = Console.ReadLine()?.Replace("\"", "");

                if (string.IsNullOrEmpty(newInFile))
                    throw new ArgumentNullException("path is null or empty");

                Console.Write("\nEnter output folder path: ");
                var newOutDir = Console.ReadLine()?.Replace("\"", "");

                if (string.IsNullOrEmpty(newOutDir))
                    throw new ArgumentNullException("path is null or empty");

                Console.Write("\nEnter output file name: ");
                var newFileName = Console.ReadLine()?.Replace(" ", "");

                if (string.IsNullOrEmpty(newFileName))
                    throw new ArgumentNullException("file name is null or empty");

                var inputData = InputDocumentData(ref mlContext, newInFile, FileFormat.Txt);
                if (inputData == null)
                    throw new ArgumentNullException("inputData == null");

                var documents = mlContext.Data.CreateEnumerable<Document>(inputData, false).ToArray();
                var predictions = ConsumeDocumentModel(ref mlContext, model, documents);

                Log.Info($"Document Analysis");
                for (int i = 0; i < documents.Length; i++)
                {
                    Console.WriteLine($"Id               : {documents[i].Id}");
                    Console.WriteLine($"Title            : {documents[i].Title}");
                    Console.WriteLine($"Description      : {documents[i].Description}");
                    Console.WriteLine($"ActualSubject    : {documents[i].Subject}");
                    Console.WriteLine($"PredictedSubject : {predictions[i].Subject}");
                }

                OutputDocumentClassification(newOutDir, newFileName, documents, predictions, FileFormat.Csv);
            }

            Console.Write("\nSave model (Y/N): ");
            if (Console.ReadLine() == "Y")
                SaveDocumentModel(ref mlContext, model, dataView!, outDir, fileName);
        }

        [Feature(instruction: DocumentClassificationInstruction)]
        public static void ClassifyDocument(string inFileModel, string inFileData, string outDir, string fileName)
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFileModel, out _);

            var inputData = InputDocumentData(ref mlContext, inFileData, FileFormat.Txt);

            var documents = mlContext.Data.CreateEnumerable<Document>(inputData, false).ToArray();
            var predictions = ConsumeDocumentModel(ref mlContext, model, documents);

            Log.Info($"Document Analysis");
            for (int i = 0; i < documents.Length; i++)
            {
                Console.WriteLine($"Id               : {documents[i].Id}");
                Console.WriteLine($"Title            : {documents[i].Title}");
                Console.WriteLine($"Description      : {documents[i].Description}");
                Console.WriteLine($"ActualSubject    : {documents[i].Subject}");
                Console.WriteLine($"PredictedSubject : {predictions[i].Subject}");
            }

            OutputDocumentClassification(outDir, fileName, documents, predictions, FileFormat.Csv);
        }

        #region DATA CONNECTION

        private static IDataView? InputDocumentData(ref MLContext mlContext, string path, FileFormat fileFormat)
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

        private static void OutputDocumentClassification(string location, string fileName, Document[] documents, DocumentPrediction[] predictions, FileFormat fileFormat)
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

                for (int i = 0; i < documents.Length; i++)
                {
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("Id", $"\"{documents[i].Id}\""),
                        new KeyValuePair<string, object?>("Title", $"\"{documents[i].Title?.Replace("\"", "\"\"")}\""),
                        new KeyValuePair<string, object?>("Description", $"\"{documents[i].Description?.Replace("\"", "\"\"")}\""),
                        new KeyValuePair<string, object?>("ActualSubject", $"\"{documents[i].Subject}\""),
                        new KeyValuePair<string, object?>("PredictedSubject", $"\"{predictions[i].Subject}\""),
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

        private static ITransformer TrainDocumentModel(ref MLContext mlContext, IDataView trainData)
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

        private static MulticlassClassificationMetrics EvaluateDocumentModel(ref MLContext mlContext, ITransformer model, IDataView testData)
        {
            var predictions = model.Transform(testData);
            var metrics = mlContext.MulticlassClassification.Evaluate(predictions);

            return metrics;
        }

        #endregion TRAINING & TESTING

        #region MODEL CONSUMPTION

        private static DocumentPrediction[] ConsumeDocumentModel(ref MLContext mlContext, ITransformer model, Document[] documents)
        {
            var predEngine = mlContext.Model.CreatePredictionEngine<Document, DocumentPrediction>(model);
            var predictions = (
                from document in documents
                let prediction = predEngine.Predict(document)
                select prediction).ToArray();

            return predictions;
        }

        private static void SaveDocumentModel(ref MLContext mlContext, ITransformer model, IDataView dataView, string location, string fileName)
        {
            var path = $"{location}\\Model @{fileName} .zip";
            mlContext.Model.Save(model, dataView.Schema, path);
        }

        #endregion MODEL CONSUMPTION
    }
}

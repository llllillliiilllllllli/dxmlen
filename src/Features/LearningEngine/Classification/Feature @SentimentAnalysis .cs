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
    internal class SentimentAnalysis
    {
        private const string SentimentAnalysisGuide =
            "\nInstruction:\n" +
            "\t......................................................................................................\n" +
            "\t......................................................................................................\n" +
            "\t......................................................................................................\n" +
            "\tSource: ..............................................................................................\n" +
            "\tSource: ..............................................................................................\n" +
            "\tSource: ..............................................................................................";

        [Feature]
        public static void BuildSentimentModel(string inFile, string outDir, string fileName)
        {
            var mlContext = new MLContext();

            var dataView = InputSentimentFromFile(ref mlContext, inFile, FileFormat.Txt);

            var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            var trainData = trainTestData.TrainSet;
            var testData = trainTestData.TestSet;

            var model = TrainSentimentModel(ref mlContext, trainData);
            var metrics = EvaluateSentimentModel(ref mlContext, model, testData);

            Log.Info($"Binary Classification Metrics");

            Console.WriteLine($"Accuracy            : {metrics.Accuracy:F3}");
            Console.WriteLine($"F1Score             : {metrics.F1Score:F3}");
            Console.WriteLine($"AreaUnderRocCurve   : {metrics.AreaUnderRocCurve:F3}");
            Console.WriteLine($"AreaUnderPRCurve    : {metrics.AreaUnderPrecisionRecallCurve:F3}\n");

            Console.WriteLine($"PositivePrecision   : {metrics.PositivePrecision:F3}");
            Console.WriteLine($"PositiveRecall      : {metrics.PositiveRecall:F3}");
            Console.WriteLine($"NegativePrecision   : {metrics.NegativePrecision:F3}");
            Console.WriteLine($"NegativeRecall      : {metrics.NegativeRecall:F3}");
            Console.WriteLine($"\n{metrics.ConfusionMatrix.GetFormattedConfusionTable()}");

            Console.Write("\nTry model (Y/N): ");
            if (Console.ReadLine() == "Y")
                TrySentimentModel(ref mlContext, model);

            Console.Write("\nSave model (Y/N): ");
            if (Console.ReadLine() == "Y")
                SaveSentimentModel(ref mlContext, model, dataView!, outDir, fileName);
        }

        [Feature]
        public static void AnalyzeSentiment(string inFileModel, string inFileData, string outDir, string fileName)
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFileModel, out _);

            var inputData = InputSentimentFromFile(ref mlContext, inFileData, FileFormat.Txt);

            var sentiments = mlContext.Data.CreateEnumerable<Sentiment>(inputData, false).ToArray();
            var predictions = ConsumeSentimentModel(ref mlContext, model, sentiments);

            Log.Info($"Sentiment Analysis");
            for (int i = 0; i < sentiments.Length; i++)
            {
                Console.WriteLine($"Content         : {sentiments[i].Content}");
                Console.WriteLine($"ActualLabel     : {sentiments[i].Label}");
                Console.WriteLine($"PredictedLabel  : {predictions[i].Prediction}");
                Console.WriteLine($"Probability     : {predictions[i].Probability}");
                Console.WriteLine($"Score           : {predictions[i].Score}\n");
            }

            OutputSentimentAnalysis(outDir, fileName, sentiments, predictions, FileFormat.Csv);
        }

        #region DATA CONNECTION

        private static IDataView? InputSentimentFromFile(ref MLContext mlContext, string path, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Txt)
            {
                var dataView = mlContext.Data.LoadFromTextFile<Sentiment>(path, hasHeader: false, separatorChar: '\t');
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

        private static void OutputSentimentAnalysis(string location, string fileName, Sentiment[] sentiments, SentimentPrediction[] predictions, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Txt)
            {
                throw new NotImplementedException();
            }

            if (fileFormat == FileFormat.Csv)
            {
                var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
                    new StringDataFrameColumn("Content"),
                    new StringDataFrameColumn("ActualLabel"),
                    new StringDataFrameColumn("PredictedLabel"),
                    new StringDataFrameColumn("Probability"),
                    new StringDataFrameColumn("Score"),
                });

                for (int i = 0; i < sentiments.Length; i++)
                {
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("Content", $"\"{sentiments[i].Content}\""),
                        new KeyValuePair<string, object?>("ActualLabel", $"\"{sentiments[i].Label}\""),
                        new KeyValuePair<string, object?>("PredictedLabel", $"\"{predictions[i].Prediction}\""),
                        new KeyValuePair<string, object?>("Probability", $"\"{predictions[i].Probability}\""),
                        new KeyValuePair<string, object?>("Score", $"\"{predictions[i].Score}\""),
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

        private static ITransformer TrainSentimentModel(ref MLContext mlContext, IDataView trainData)
        {
            var pipeline = mlContext.Transforms.Text
                .FeaturizeText(inputColumnName: "Content", outputColumnName: "Features")
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: "Label", featureColumnName: "Features"));

            var model = pipeline.Fit(trainData);
            return model;
        }

        private static BinaryClassificationMetrics EvaluateSentimentModel(ref MLContext mlContext, ITransformer model, IDataView testData)
        {
            var predictions = model.Transform(testData);
            var metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label", "Score");

            return metrics;
        }

        #endregion TRAINING & TESTING

        #region MODEL CONSUMPTION

        private static void TrySentimentModel(ref MLContext mlContext, ITransformer model)
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

            var inputData = InputSentimentFromFile(ref mlContext, inFile, FileFormat.Txt);
            if (inputData == null)
                throw new ArgumentNullException("inputData == null");

            var sentiments = mlContext.Data.CreateEnumerable<Sentiment>(inputData, false).ToArray();
            var predictions = ConsumeSentimentModel(ref mlContext, model, sentiments);

            Log.Info($"Sentiment Analysis");
            for (int i = 0; i < sentiments.Length; i++)
            {
                Console.WriteLine($"Content         : {sentiments[i].Content}");
                Console.WriteLine($"ActualLabel     : {sentiments[i].Label}");
                Console.WriteLine($"PredictedLabel  : {predictions[i].Prediction}");
                Console.WriteLine($"Probability     : {predictions[i].Probability}");
                Console.WriteLine($"Score           : {predictions[i].Score}\n");
            }

            OutputSentimentAnalysis(outDir, fileName, sentiments, predictions, FileFormat.Csv);
        }

        private static SentimentPrediction[] ConsumeSentimentModel(ref MLContext mlContext, ITransformer model, Sentiment[] sentiments)
        {
            var predEngine = mlContext.Model.CreatePredictionEngine<Sentiment, SentimentPrediction>(model);
            var predictions = (
                from sentiment in sentiments
                let prediction = predEngine.Predict(sentiment)
                select prediction).ToArray();

            return predictions;
        }

        private static void SaveSentimentModel(ref MLContext mlContext, ITransformer model, IDataView dataView, string location, string fileName)
        {
            var path = $"{location}\\Model @{fileName} .zip";
            mlContext.Model.Save(model, dataView.Schema, path);
        }

        #endregion MODEL CONSUMPTION
    }
}

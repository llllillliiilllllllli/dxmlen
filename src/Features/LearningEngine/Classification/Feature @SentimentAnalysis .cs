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

namespace DxMLEngine.Features.SentimentAnalysis
{
    [Feature]
    internal class SentimentAnalysis
    {
        #region INSTRUCTION

        private const string SentimentAnalysisInstruction =
            "\nInstruction:\n" +
            "\tThis function classfies comments and feedbacks in English by means of sentiment.\n" +
            "\tSentiment of text can be positive labeled as ones and zeros in trainning dataset.\n" +
            "\tThe model is a binary logistic classifier named SdcaLogisticRegressionBinaryTrainer.\n" +
            "\tModel is statistically calibrated to provide probabilities of uncertain predictions.\n" +
            "\tModel predictability is evaluated by precision and retrieval metrics, among others.\n" +

            "\tSource: https://learn.microsoft.com/en-us/dotnet/api/microsoft.ml.trainers.sdcalogisticregressionbinarytrainer?view=ml-dotnet\n" +
            "\tSource: https://en.wikipedia.org/wiki/Binary_classification\n" +
            "\tSource: https://www.ijcai.org/Proceedings/11/Papers/462.pdf\n" +
            "\tSource: https://icml.cc/Conferences/2008/papers/166.pdf\n" +
            "\tSource: https://en.wikipedia.org/wiki/Calibration_(statistics)";

        #endregion INSTRUCTION

        [Feature(instruction: SentimentAnalysisInstruction)]
        public static void BuildSentimentModel(string inFile, string outDir, string fileName)
        {
            var mlContext = new MLContext();

            var dataView = InputSentimentData(ref mlContext, inFile, FileFormat.Txt);

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

                var inputData = InputSentimentData(ref mlContext, newInFile, FileFormat.Txt);
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

                OutputSentimentAnalysis(newOutDir, newFileName, sentiments, predictions, FileFormat.Csv);
            }

            Console.Write("\nSave model (Y/N): ");
            if (Console.ReadLine() == "Y")
                SaveSentimentModel(ref mlContext, model, dataView!, outDir, fileName);
        }

        [Feature(instruction: SentimentAnalysisInstruction)]
        public static void AnalyzeSentiment(string inFileModel, string inFileData, string outDir, string fileName)
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFileModel, out _);

            var inputData = InputSentimentData(ref mlContext, inFileData, FileFormat.Txt);

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

        private static IDataView? InputSentimentData(ref MLContext mlContext, string path, FileFormat fileFormat)
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

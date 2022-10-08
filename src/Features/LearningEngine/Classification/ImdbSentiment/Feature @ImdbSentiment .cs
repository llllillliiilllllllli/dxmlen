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
using Microsoft.ML.Transforms;

using DxMLEngine.Attributes;
using DxMLEngine.Utilities;
using DxMLEngine.Objects;

namespace DxMLEngine.Features.Classification
{
    [Feature]
    internal class ImdbSentiment
    {
        private const string ImdbSentimentGuide =
            "\nInstruction:\n" +
            "\t......................................................................................................\n" +
            "\t......................................................................................................\n" +
            "\t......................................................................................................\n" +
            "\tSource: ..............................................................................................\n" +
            "\tSource: ..............................................................................................\n" +
            "\tSource: ..............................................................................................";

        [Feature]
        public static void BuildSentimentModel(string inFileModel, string inFileData, string inFileLookupMap, string outDir, string fileName)
        {
            var mlContext = new MLContext();

            var dataView = InputSentimentFromFile(ref mlContext, inFileData, FileFormat.Csv);
            var trainData = mlContext.Data.LoadFromEnumerable(new List<Review>());
            var testData = dataView;

            var model = TrainSentimentModel(ref mlContext, trainData, inFileModel, inFileLookupMap);
            var metrics = EvaluateSentimentModel(ref mlContext, model, testData);

            Log.Info($"IMDB Sentiment Model Metrics");

            //Console.WriteLine($"Accuracy            : {metrics.Accuracy:F3}");
            //Console.WriteLine($"F1Score             : {metrics.F1Score:F3}");
            //Console.WriteLine($"AreaUnderRocCurve   : {metrics.AreaUnderRocCurve:F3}");
            //Console.WriteLine($"AreaUnderPRCurve    : {metrics.AreaUnderPrecisionRecallCurve:F3}\n");

            //Console.WriteLine($"PositivePrecision   : {metrics.PositivePrecision:F3}");
            //Console.WriteLine($"PositiveRecall      : {metrics.PositiveRecall:F3}");
            //Console.WriteLine($"NegativePrecision   : {metrics.NegativePrecision:F3}");
            //Console.WriteLine($"NegativeRecall      : {metrics.NegativeRecall:F3}");
            //Console.WriteLine($"\n{metrics.ConfusionMatrix.GetFormattedConfusionTable()}");

            Console.Write("\nTry model (Y/N): ");
            if (Console.ReadLine() == "Y")
                TrySentimentModel(ref mlContext, model);

            Console.Write("\nSave model (Y/N): ");
            if (Console.ReadLine() == "Y")
                SaveSentimentModel(ref mlContext, model, trainData, outDir, fileName);
        }

        [Feature]
        public static void AnalyzeSentiment(string inFileModel, string inFileData, string outDir, string fileName)
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFileModel, out _);

            var inputData = InputSentimentFromFile(ref mlContext, inFileData, FileFormat.Csv);
            var sentiments = mlContext.Data.CreateEnumerable<Review>(inputData, false).ToArray();
            var predictions = ConsumeSentimentModel(ref mlContext, model, sentiments);

            Log.Info($"IMDB Sentiment Analysis");
            for (int i = 0; i < sentiments.Length; i++)
            {
                Console.WriteLine($"Content        : {predictions[i].Content}");
                Console.WriteLine($"ActualLabel    : {predictions[i].Label}");
                Console.WriteLine($"PredictedLabel : {predictions[i].Results![0]}\n");
                Console.WriteLine($"PredictedLabel : {predictions[i].Results![1]}\n");
            }

            OutputSentimentAnalysis(outDir, fileName, predictions, FileFormat.Csv);
        }

        #region DATA CONNECTION

        private static IDataView InputSentimentFromFile(ref MLContext mlContext, string path, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Tsv)
            {
                return mlContext.Data.LoadFromTextFile<Review>(path, hasHeader: true, separatorChar: '\t');
            }

            if (fileFormat == FileFormat.Csv)
            {
                return mlContext.Data.LoadFromTextFile<Review>(path, hasHeader: true, separatorChar: ',');
            }

            if (fileFormat == FileFormat.Json)
            {
                throw new NotImplementedException();
            }

            return mlContext.Data.LoadFromEnumerable(new List<Review>());
        }

        private static void OutputSentimentAnalysis(string location, string fileName, ReviewPrediction[] predictions, FileFormat fileFormat)
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
                    new StringDataFrameColumn("PredictedLabel1"),
                    new StringDataFrameColumn("PredictedLabel2"),
                });

                for (int i = 0; i < predictions.Length; i++)
                {
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("Content",        $"\"{predictions[i].Content}\""),
                        new KeyValuePair<string, object?>("ActualLabel",    $"\"{predictions[i].Label}\""),
                        new KeyValuePair<string, object?>("PredictedLabel1",$"\"{predictions[i].Results![0]}\""),
                        new KeyValuePair<string, object?>("PredictedLabel1",$"\"{predictions[i].Results![1]}\""),
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

        private static ITransformer TrainSentimentModel(ref MLContext mlContext, IDataView trainData, string inFileModel, string inFileLookupMap)
        {
            var tensorFlowModel = mlContext.Model.LoadTensorFlowModel(inFileModel);

            var lookupMap = mlContext.Data.LoadFromTextFile(
                inFileLookupMap, separatorChar: ',', columns: new[]
                {
                    new TextLoader.Column("Word", DataKind.String, 0),
                    new TextLoader.Column("Id", DataKind.Int32, 1),
                });

            var pipeline = mlContext.Transforms.Text
                .TokenizeIntoWords(
                    inputColumnName: "ReviewText", 
                    outputColumnName: "TokenizedWords")
                .Append(mlContext.Transforms.Conversion.MapValue(
                    inputColumnName: "TokenizedWords",
                    outputColumnName: "Features",
                    lookupMap: lookupMap, 
                    keyColumn: lookupMap.Schema["Word"], 
                    valueColumn: lookupMap.Schema["Id"]))
                .Append(mlContext.Transforms.CustomMapping(ResizeFeaturesAction, "Resize"))
                .Append(tensorFlowModel.ScoreTensorFlowModel(
                    inputColumnName: "Features",
                    outputColumnName: "Prediction/Softmax"))
                .Append(mlContext.Transforms.CopyColumns(
                    inputColumnName: "Prediction/Softmax", 
                    outputColumnName: "Prediction"));

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

        private static void ExploreSentimentModel(TensorFlowModel model)
        {
            var schema = model.GetModelSchema();
            var featuresType = (VectorDataViewType)schema["Features"].Type;
            var predictionType = (VectorDataViewType)schema["Prediction/Softmax"].Type;

            Log.Info("TensorFlow Sentiment Model");

            Console.WriteLine($"Features            : {featuresType.ItemType.RawType}");
            Console.WriteLine($"Features Size       : {featuresType.Size}");
            Console.WriteLine($"Prediction/Softmax  : {predictionType.ItemType.RawType}");
            Console.WriteLine($"Prediction Size     : {predictionType.Size}\n");
        }

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

            var inputData = InputSentimentFromFile(ref mlContext, inFile, FileFormat.Csv);
            var sentiments = mlContext.Data.CreateEnumerable<Review>(inputData, false).ToArray();
            var predictions = ConsumeSentimentModel(ref mlContext, model, sentiments);

            Log.Info($"IMDB Sentiment Analysis");
            for (int i = 0; i < sentiments.Length; i++)
            {
                Console.WriteLine($"Content        : {predictions[i].Content}");
                Console.WriteLine($"ActualLabel    : {predictions[i].Label}");
                Console.WriteLine($"PredictedLabel : {predictions[i].Results![0]}\n");
                Console.WriteLine($"PredictedLabel : {predictions[i].Results![1]}\n");
            }

            OutputSentimentAnalysis(outDir, fileName, predictions, FileFormat.Csv);
        }

        private static ReviewPrediction[] ConsumeSentimentModel(ref MLContext mlContext, ITransformer model, Review[] reviews)
        {
            var predEngine = mlContext.Model.CreatePredictionEngine<Review, ReviewPrediction>(model);
            var predictions = (
                from review in reviews
                let prediction = predEngine.Predict(review)
                select prediction).ToArray();

            return predictions;
        }

        private static void SaveSentimentModel(ref MLContext mlContext, ITransformer model, IDataView dataView, string location, string fileName)
        {
            var path = $"{location}\\Model @{fileName} .zip";
            mlContext.Model.Save(model, dataView.Schema, path);
        }

        #endregion MODEL CONSUMPTION

        #region PRE-PROCESSING

        private const int FeatureLength = 600;

        private class VariableLength
        {
            [VectorType]
            public int[]? Features { get; set; }
        }

        private class FixedLength
        {
            [VectorType]
            public int[]? Features { get; set; }
        }

        private static Action<VariableLength, FixedLength> ResizeFeaturesAction = (rhs, lhs) =>
        {
            var features = rhs.Features;
            Array.Resize(ref features, FeatureLength);
            lhs.Features = features;
        };

        #endregion PRE-PROCESSING
    }
}

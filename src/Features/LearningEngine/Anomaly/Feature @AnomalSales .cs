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
using Microsoft.ML.TimeSeries;
using DxMLEngine.Features.UNComtrade;
using DxMLEngine.Features.Classification;
using Tensorflow.Keras.Engine;

namespace DxMLEngine.Features.AnomalyDetection
{
    [Feature]
    internal class AnomalSalesDetection
    {
        private const string AnomalSalesDetectionGuide =
            "\nInstruction:\n" +
            "\t......................................................................................................\n" +
            "\t......................................................................................................\n" +
            "\t......................................................................................................\n" +
            "\tSource: ..............................................................................................\n" +
            "\tSource: ..............................................................................................\n" +
            "\tSource: ..............................................................................................";

        [Feature]
        public static void BuildAnomalyModel(string inFile, string outDir, string fileName)
        {
            var mlContext = new MLContext();

            var dataView = InputSalesFromFile(ref mlContext, inFile, FileFormat.Csv);
            var trainData = mlContext.Data.LoadFromEnumerable(new List<Sales>());
            var testData = dataView;

            var model = TrainAnomalyModel(ref mlContext, trainData, testData);
            var metrics = EvaluateAnomalyModel(ref mlContext, model, testData);

            Console.Write("\nSave metrics (Y/N): ");
            if (Console.ReadLine() == "Y")
                Console.WriteLine($"\n{metrics} saved");

            Console.Write("\nTry model (Y/N): ");
            if (Console.ReadLine() == "Y")
                TryAnomalyModel(ref mlContext, model);

            Console.Write("\nSave model (Y/N): ");
            if (Console.ReadLine() == "Y")
                SaveAnomalyModel(ref mlContext, model, testData, outDir, fileName);
        }

        [Feature]
        public static void DetectAnomalSales(string inFileModel, string inFileData, string outDir, string fileName)
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFileModel, out _);

            var inputData = InputSalesFromFile(ref mlContext, inFileData, FileFormat.Csv);
            if (inputData == null)
                throw new ArgumentNullException("inputData == null");

            var salesData = mlContext.Data.CreateEnumerable<Sales>(inputData, false).ToArray();                      
            var predictions = ConsumeAnomalyModel(ref mlContext, model, salesData);

            Log.Info($"Product Sales Anomaly Detection");
            for (int i = 0; i < predictions.Length; i++)
            {
                Console.WriteLine($"Month      : {predictions[i].Month:F3}");
                Console.WriteLine($"TotalSales : {predictions[i].TotalSales:F3}");
                Console.WriteLine($"Prediction : {predictions[i].Results![0]:F3}");
                Console.WriteLine($"Score      : {predictions[i].Results![1]:F3}");
                Console.WriteLine($"PValue     : {predictions[i].Results![2]:F3}\n");
            }

            OutputSalesDetection(outDir, fileName, predictions, FileFormat.Csv);
        }

        #region DATA CONNECTION

        private static IDataView InputSalesFromFile(ref MLContext mlContext, string path, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Csv)
            {
                var dataView = mlContext.Data.LoadFromTextFile<Sales>(path, hasHeader: true, separatorChar: ',');
                return dataView;
            }
            return mlContext.Data.LoadFromEnumerable(new List<Sales>());
        }

        private static void OutputSalesDetection(string location, string fileName, SalesPrediction[] predictions, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Txt)
            {
                throw new NotImplementedException();
            }

            if (fileFormat == FileFormat.Csv)
            {
                var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
                    new StringDataFrameColumn("Month"),
                    new StringDataFrameColumn("TotalSales"),
                    new StringDataFrameColumn("PredictedLabel"),
                    new StringDataFrameColumn("Score"),
                    new StringDataFrameColumn("PValue"),
                });

                for (int i = 0; i < predictions.Length; i++)
                {
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("Month",          $"\"{predictions[i].Month}\""),
                        new KeyValuePair<string, object?>("TotalSales",     $"\"{predictions[i].TotalSales}\""),
                        new KeyValuePair<string, object?>("PredictedLabel", $"\"{predictions[i].Results![0]}\""),
                        new KeyValuePair<string, object?>("Score",          $"\"{predictions[i].Results![1]}\""),
                        new KeyValuePair<string, object?>("PValue",         $"\"{predictions[i].Results![2]}\""),
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

        private static ITransformer TrainAnomalyModel(ref MLContext mlContext, IDataView trainData, IDataView testData)
        {
            var numObservations = mlContext.Data.CreateEnumerable<Sales>(testData, false).Count();

            var pipeline = mlContext.Transforms
                .DetectIidSpike(
                    inputColumnName: "TotalSales", 
                    outputColumnName: "Results", 
                    confidence: 95.0, 
                    pvalueHistoryLength: numObservations / 12);

            var model = pipeline.Fit(trainData);
            return model;
        }

        private static AnomalyDetectionMetrics EvaluateAnomalyModel(ref MLContext mlContext, ITransformer model, IDataView testData)
        {
            var outputDataView = model.Transform(testData);

            var predictions = mlContext.Data.CreateEnumerable<SalesPrediction>(outputDataView, false);
            outputDataView = mlContext.Data.LoadFromEnumerable(predictions);

            var metrics = mlContext.AnomalyDetection.Evaluate(outputDataView, "Label", "Score", "PredictedLabel");

            Log.Info($"Anomaly Detection Metrics");

            Console.WriteLine($"AreaUnderRocCurve   : {metrics.AreaUnderRocCurve:F3}");
            Console.WriteLine($"RateAtFalsePositive : {metrics.DetectionRateAtFalsePositiveCount:F3}");

            return metrics;
        }

        #endregion TRAINING & TESTING

        #region MODEL CONSUMPTION

        private static void TryAnomalyModel(ref MLContext mlContext, ITransformer model)
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

            var inputData = InputSalesFromFile(ref mlContext, inFile, FileFormat.Csv);
            if (inputData == null)
                throw new ArgumentNullException("inputData == null");

            var salesData = mlContext.Data.CreateEnumerable<Sales>(inputData, false).ToArray();
            var predictions = ConsumeAnomalyModel(ref mlContext, model, salesData);

            Log.Info($"Product Sales Anomaly Detection");
            for (int i = 0; i < predictions.Length; i++)
            {
                Console.WriteLine($"Month      : {predictions[i].Month:F3}");
                Console.WriteLine($"TotalSales : {predictions[i].TotalSales:F3}");
                Console.WriteLine($"Prediction : {predictions[i].Results![0]:F3}");
                Console.WriteLine($"Score      : {predictions[i].Results![1]:F3}");
                Console.WriteLine($"PValue     : {predictions[i].Results![2]:F3}\n");
            }

            OutputSalesDetection(outDir, fileName, predictions, FileFormat.Csv);
        }

        private static SalesPrediction[] ConsumeAnomalyModel(ref MLContext mlContext, ITransformer model, Sales[] salesData)
        {
            var inputData = mlContext.Data.LoadFromEnumerable(salesData);
            var predictions = mlContext.Data.CreateEnumerable<SalesPrediction>(model.Transform(inputData), false);

            return predictions.ToArray();
        }

        private static void SaveAnomalyModel(ref MLContext mlContext, ITransformer model, IDataView dataView, string location, string fileName)
        {
            var path = $"{location}\\Model @{fileName} .zip";
            mlContext.Model.Save(model, dataView.Schema, path);
        }

        #endregion MODEL CONSUMPTION
    }
}

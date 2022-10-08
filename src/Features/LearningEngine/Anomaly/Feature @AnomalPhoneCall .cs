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

namespace DxMLEngine.Features.AnomalyDetection
{
    [Feature]
    internal class AnomalPhoneCallDetection
    {
        private const string AnomalPhoneCallDetectionGuide =
            "\nInstruction:\n" +
            "\t......................................................................................................\n" +
            "\t......................................................................................................\n" +
            "\t......................................................................................................\n" +
            "\tSource: ..............................................................................................\n" +
            "\tSource: ..............................................................................................\n" +
            "\tSource: ..............................................................................................";

        [Feature]
        public static void BuildAnomalyModel(string inFile)
        {
            var mlContext = new MLContext();

            Console.WriteLine("\nAnomaly detection by SR-CNN model is a built-in function.");

            var dataView = InputPhoneCallFromFile(ref mlContext, inFile, FileFormat.Csv);
            var metrics = EvaluateAnomalyModel(ref mlContext, dataView!);

            Console.Write("\nTry model (Y/N): ");
            if (Console.ReadLine() == "Y")
                TryAnomalyModel(ref mlContext);
        }

        [Feature]
        public static void DetectAnomalPhoneCalls(string inFile, string outDir, string fileName)
        {
            var mlContext = new MLContext();
            
            var inputData = InputPhoneCallFromFile(ref mlContext, inFile, FileFormat.Csv);
            if (inputData == null)
                throw new ArgumentNullException("inputData == null");

            var phoneCalls = mlContext.Data.CreateEnumerable<PhoneCall>(inputData, false).ToArray();
            var predictions = ConsumeAnomalyModel(ref mlContext, phoneCalls);

            Log.Info($"Phone Calls Anomaly Detection");
            for (int i = 0; i < predictions.Length; i++)
            {
                Console.WriteLine($"Timestamp     : {predictions[i].Timestamp:F3}");
                Console.WriteLine($"PhoneCalls    : {predictions[i].PhoneCalls:F3}");
                Console.WriteLine($"IsAnomaly     : {predictions[i].Results![0]:F3}");
                Console.WriteLine($"AnomalyScore  : {predictions[i].Results![1]:F3}");
                Console.WriteLine($"Magnitude     : {predictions[i].Results![2]:F3}");
                Console.WriteLine($"ExpectedValue : {predictions[i].Results![3]:F3}");
                Console.WriteLine($"BoundaryUnits : {predictions[i].Results![4]:F3}");
                Console.WriteLine($"UpperBoundary : {predictions[i].Results![5]:F3}");
                Console.WriteLine($"LowerBoundary : {predictions[i].Results![6]:F3}\n");
            }

            OutputPhoneCallDetection(outDir, fileName, predictions, FileFormat.Csv);
        }

        #region DATA CONNECTION

        private static IDataView? InputPhoneCallFromFile(ref MLContext mlContext, string path, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Csv)
            {
                var dataView = mlContext.Data.LoadFromTextFile<PhoneCall>(path, hasHeader: true, separatorChar: ',');
                return dataView;
            }
            return null;
        }

        private static void OutputPhoneCallDetection(string location, string fileName, PhoneCallPrediction[] predictions, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Txt)
            {
                throw new NotImplementedException();
            }

            if (fileFormat == FileFormat.Csv)
            {
                var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
                    new StringDataFrameColumn("Timestamp"),
                    new StringDataFrameColumn("PhoneCalls"),
                    new StringDataFrameColumn("IsAnomaly"),
                    new StringDataFrameColumn("AnomalyScore"),
                    new StringDataFrameColumn("Magnitude"),
                    new StringDataFrameColumn("ExpectedValue"),
                    new StringDataFrameColumn("BoundaryUnits"),
                    new StringDataFrameColumn("UpperBoundary"),
                    new StringDataFrameColumn("LowerBoundary"),
                });

                for (int i = 0; i < predictions.Length; i++)
                {
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("Timestamp",      $"\"{predictions[i].Timestamp}\""),
                        new KeyValuePair<string, object?>("PhoneCalls",     $"\"{predictions[i].PhoneCalls}\""),
                        new KeyValuePair<string, object?>("IsAnomaly",      $"\"{predictions[i].Results![0]}\""),
                        new KeyValuePair<string, object?>("AnomalyScore",   $"\"{predictions[i].Results![1]}\""),
                        new KeyValuePair<string, object?>("Magnitude",      $"\"{predictions[i].Results![2]}\""),
                        new KeyValuePair<string, object?>("ExpectedValue",  $"\"{predictions[i].Results![3]}\""),
                        new KeyValuePair<string, object?>("BoundaryUnits",  $"\"{predictions[i].Results![4]}\""),
                        new KeyValuePair<string, object?>("UpperBoundary",  $"\"{predictions[i].Results![5]}\""),
                        new KeyValuePair<string, object?>("LowerBoundary",  $"\"{predictions[i].Results![6]}\""),
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

        private static ITransformer TrainAnomalyModel(ref MLContext mlContext, IDataView trainData)
        {
            throw new NotImplementedException();
        }

        private static AnomalyDetectionMetrics EvaluateAnomalyModel(ref MLContext mlContext, IDataView testData)
        {
            var period = mlContext.AnomalyDetection
                .DetectSeasonality(testData, "Value");

            var outputDataView = mlContext.AnomalyDetection
                .DetectEntireAnomalyBySrCnn(testData,
                    options: new SrCnnEntireAnomalyDetectorOptions()
                    {
                        Threshold = 0.3,
                        Sensitivity = 64.0,
                        DetectMode = SrCnnDetectMode.AnomalyAndMargin,
                        Period = period,
                    },
                    inputColumnName: "Value",
                    outputColumnName: "Results");

            var predictions = mlContext.Data.CreateEnumerable<PhoneCallPrediction>(outputDataView, false);
            outputDataView = mlContext.Data.LoadFromEnumerable(predictions);

            var metrics = mlContext.AnomalyDetection.Evaluate(outputDataView, "Label", "Score", "PredictedLabel");

            Log.Info($"Anomaly Detection Metrics");

            Console.WriteLine($"AreaUnderRocCurve   : {metrics.AreaUnderRocCurve:F3}");
            Console.WriteLine($"RateAtFalsePositive : {metrics.DetectionRateAtFalsePositiveCount:F3}");

            return metrics;
        }

        #endregion TRAINING & TESTING

        #region MODEL CONSUMPTION

        private static void TryAnomalyModel(ref MLContext mlContext)
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

            var inputData = InputPhoneCallFromFile(ref mlContext, inFile, FileFormat.Csv);
            if (inputData == null)
                throw new ArgumentNullException("inputData == null");

            var phoneCalls = mlContext.Data.CreateEnumerable<PhoneCall>(inputData, false).ToArray();
            var predictions = ConsumeAnomalyModel(ref mlContext, phoneCalls);

            Log.Info($"Phone Calls Anomaly Detection");
            for (int i = 0; i < predictions.Length; i++)
            {
                Console.WriteLine($"Timestamp     : {predictions[i].Timestamp:F3}");
                Console.WriteLine($"PhoneCalls    : {predictions[i].PhoneCalls:F3}");
                Console.WriteLine($"IsAnomaly     : {predictions[i].Results![0]:F3}");
                Console.WriteLine($"AnomalyScore  : {predictions[i].Results![1]:F3}");
                Console.WriteLine($"Magnitude     : {predictions[i].Results![2]:F3}");
                Console.WriteLine($"ExpectedValue : {predictions[i].Results![3]:F3}");
                Console.WriteLine($"BoundaryUnits : {predictions[i].Results![4]:F3}");
                Console.WriteLine($"UpperBoundary : {predictions[i].Results![5]:F3}");
                Console.WriteLine($"LowerBoundary : {predictions[i].Results![6]:F3}\n");
            }

            OutputPhoneCallDetection(outDir, fileName, predictions, FileFormat.Csv);
        }

        private static PhoneCallPrediction[] ConsumeAnomalyModel(ref MLContext mlContext, PhoneCall[] phoneCalls)
        {
            var inputDataView = mlContext.Data.LoadFromEnumerable(phoneCalls);
            var period = mlContext.AnomalyDetection
                .DetectSeasonality(inputDataView, inputColumnName: "Value");

            var outputDataView = mlContext.AnomalyDetection
                .DetectEntireAnomalyBySrCnn(inputDataView,
                    options: new SrCnnEntireAnomalyDetectorOptions()
                    {
                        Threshold = 0.3,
                        Sensitivity = 64.0,
                        DetectMode = SrCnnDetectMode.AnomalyAndMargin,
                        Period = period,
                    },
                    inputColumnName: "Value",
                    outputColumnName: "Results");

            var predictions = mlContext.Data.CreateEnumerable<PhoneCallPrediction>(outputDataView, false);
            return predictions.ToArray();
        }

        private static void SaveAnomalyModel(ref MLContext mlContext, ITransformer model, IDataView dataView, string location, string fileName)
        {
            throw new NotImplementedException(); 
        }

        #endregion MODEL CONSUMPTION
    }
}

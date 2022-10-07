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

namespace DxMLEngine.Features.Forecasting
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
        public static void BuildAnomalyModel(string inFile, string outDir, string fileName)
        {
            var mlContext = new MLContext();            
            var dataView = InputPhoneCallFromFile(ref mlContext, inFile, FileFormat.Csv);

            var metrics = EvaluateAnomalyModel(ref mlContext, dataView!);

            Log.Info($"Anomaly Detection Metrics");

            Console.WriteLine($"AreaUnderRocCurve   : {metrics.AreaUnderRocCurve:F3}");
            Console.WriteLine($"RateAtFalsePositive : {metrics.DetectionRateAtFalsePositiveCount:F3}");

            Console.Write("\nTry model (Y/N): ");
            if (Console.ReadLine() == "Y")
                TryAnomalyModel(ref mlContext);
        }

        [Feature]
        public static void DetectPhoneCalls(string inFile, string outDir, string fileName)
        {
            var mlContext = new MLContext();
            
            var inputData = InputPhoneCallFromFile(ref mlContext, inFile, FileFormat.Csv);
            if (inputData == null)
                throw new ArgumentNullException("inputData == null");

            var phoneCalls = mlContext.Data.CreateEnumerable<PhoneCall>(inputData, false).ToArray();
            var predictions = ConsumeAnomalyModel(ref mlContext, phoneCalls);

            Log.Info($"Taxi Fare Forecast");
            for (int i = 0; i < phoneCalls.Length; i++)
            {
                Console.WriteLine($"Timestamp     : {phoneCalls[i].Timestamp:F3}");
                Console.WriteLine($"PhoneCalls    : {phoneCalls[i].PhoneCalls:F3}");
                Console.WriteLine($"IsAnomaly     : {predictions[i].Prediction?[0]:F3}");
                Console.WriteLine($"AnomalyScore  : {predictions[i].Prediction?[1]:F3}");
                Console.WriteLine($"Magnitude     : {predictions[i].Prediction?[2]:F3}");
                Console.WriteLine($"ExpectedValue : {predictions[i].Prediction?[3]:F3}");
                Console.WriteLine($"BoundaryUnits : {predictions[i].Prediction?[4]:F3}");
                Console.WriteLine($"UpperBoundary : {predictions[i].Prediction?[5]:F3}");
                Console.WriteLine($"LowerBoundary : {predictions[i].Prediction?[6]:F3}\n");
            }

            OutputPhoneCallDetection(outDir, fileName, phoneCalls, predictions, FileFormat.Csv);
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

        private static void OutputPhoneCallDetection(string location, string fileName, PhoneCall[] phoneCalls, PhoneCallPrediction[] predictions, FileFormat fileFormat)
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

                for (int i = 0; i < phoneCalls.Length; i++)
                {
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("Timestamp", $"\"{phoneCalls[i].Timestamp}\""),
                        new KeyValuePair<string, object?>("PhoneCalls", $"\"{phoneCalls[i].PhoneCalls}\""),
                        new KeyValuePair<string, object?>("IsAnomaly", $"\"{predictions[i].Prediction![0]}\""),
                        new KeyValuePair<string, object?>("AnomalyScore", $"\"{predictions[i].Prediction![1]}\""),
                        new KeyValuePair<string, object?>("Magnitude", $"\"{predictions[i].Prediction![2]}\""),
                        new KeyValuePair<string, object?>("ExpectedValue", $"\"{predictions[i].Prediction![3]}\""),
                        new KeyValuePair<string, object?>("BoundaryUnits", $"\"{predictions[i].Prediction![4]}\""),
                        new KeyValuePair<string, object?>("UpperBoundary", $"\"{predictions[i].Prediction![5]}\""),
                        new KeyValuePair<string, object?>("LowerBoundary", $"\"{predictions[i].Prediction![6]}\""),
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
                .DetectSeasonality(testData, nameof(PhoneCall.PhoneCalls));

            var predictions = mlContext.AnomalyDetection
                .DetectEntireAnomalyBySrCnn(testData,
                    options: new SrCnnEntireAnomalyDetectorOptions()
                    {
                        Threshold = 0.3,
                        Sensitivity = 64.0,
                        DetectMode = SrCnnDetectMode.AnomalyAndMargin,
                        Period = period,
                    },
                    inputColumnName: nameof(PhoneCall.PhoneCalls),
                    outputColumnName: nameof(PhoneCallPrediction.Prediction));

            var metrics = mlContext.AnomalyDetection.Evaluate(predictions, nameof(PhoneCall.PhoneCalls));
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

            Log.Info($"Taxi Fare Forecast");
            for (int i = 0; i < phoneCalls.Length; i++)
            {
                Console.WriteLine($"Timestamp     : {phoneCalls[i].Timestamp:F3}");
                Console.WriteLine($"PhoneCalls    : {phoneCalls[i].PhoneCalls:F3}");
                Console.WriteLine($"IsAnomaly     : {predictions[i].Prediction?[0]:F3}");
                Console.WriteLine($"AnomalyScore  : {predictions[i].Prediction?[1]:F3}");
                Console.WriteLine($"Magnitude     : {predictions[i].Prediction?[2]:F3}");
                Console.WriteLine($"ExpectedValue : {predictions[i].Prediction?[3]:F3}");
                Console.WriteLine($"BoundaryUnits : {predictions[i].Prediction?[4]:F3}");
                Console.WriteLine($"UpperBoundary : {predictions[i].Prediction?[5]:F3}");
                Console.WriteLine($"LowerBoundary : {predictions[i].Prediction?[6]:F3}\n");
            }

            OutputPhoneCallDetection(outDir, fileName, phoneCalls, predictions, FileFormat.Csv);
        }

        private static PhoneCallPrediction[] ConsumeAnomalyModel(ref MLContext mlContext, PhoneCall[] phoneCalls)
        {
            var inputDataView = mlContext.Data.LoadFromEnumerable(phoneCalls);
            var period = mlContext.AnomalyDetection
                .DetectSeasonality(inputDataView, nameof(PhoneCall.PhoneCalls));

            var outputDataView = mlContext.AnomalyDetection
                .DetectEntireAnomalyBySrCnn(inputDataView,
                    options: new SrCnnEntireAnomalyDetectorOptions()
                    {
                        Threshold = 0.3,
                        Sensitivity = 64.0,
                        DetectMode = SrCnnDetectMode.AnomalyAndMargin,
                        Period = period,
                    },
                    inputColumnName: nameof(PhoneCall.PhoneCalls),
                    outputColumnName: nameof(PhoneCallPrediction.Prediction));

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

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

namespace DxMLEngine.Features.Forecasting
{
    [Feature]
    internal class TaxiFareForecast
    {
        #region INSTRUCTION

        private const string TaxiFareModelInstruction =
            "\nInstruction:\n" +
            "\tThis feature build a regression model based on machine learning algorithms to predict taxi fares.\n" +
            "\tFare amount is predicted using vendor, rate, passenger count, payment, trip time and distance.\n" +
            "\tThe model is trained on FastTreeRegression an efficient implementation of the MART gradient boosting algorithm.\n" +
            "\tResults are then evaluated using common regression metrics MSE, MAE, RMSE, and R-Squared\n" +

            "\tSource: https://learn.microsoft.com/en-us/dotnet/api/microsoft.ml.trainers.fasttree.fasttreeregressiontrainer?view=ml-dotnet\n" +
            "\tSource: https://en.wikipedia.org/wiki/Gradient_boosting\n" +
            "\tSource: https://arxiv.org/abs/1505.01866\n" +
            "\tSource: https://jerryfriedman.su.domains/ftp/trebst.pdf";

        #endregion INSTRUCTION

        [Feature(instruction: TaxiFareModelInstruction)]
        public static void BuildTaxiFareModel(string inFile, string outDir, string fileName)
        {
            var mlContext = new MLContext();
            
            var dataView = InputTaxiFareData(ref mlContext, inFile, FileFormat.Csv);
            var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            var trainData = trainTestData.TrainSet;
            var testData = trainTestData.TestSet;

            var model = TrainTaxiFareModel(ref mlContext, trainData);
            var metrics = EvaluateTaxiFareModel(ref mlContext, model, testData);

            Log.Info($"Regression Metrics");

            Console.WriteLine($"MSE  : {metrics.MeanSquaredError:F3}");
            Console.WriteLine($"MAE  : {metrics.MeanAbsoluteError:F3}");
            Console.WriteLine($"RMSE : {metrics.RootMeanSquaredError:F3}");
            Console.WriteLine($"R-Sq : {metrics.RSquared:F3}");

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

                var inputData = InputTaxiFareData(ref mlContext, newInFile, FileFormat.Csv);
                if (inputData == null)
                    throw new ArgumentNullException("inputData == null");

                var taxiFares = mlContext.Data.CreateEnumerable<TaxiFare>(inputData, false).ToArray();
                var predictions = ConsumeTaxiFareModel(ref mlContext, model, taxiFares);

                Log.Info($"Taxi Fare Forecast");
                for (int i = 0; i < taxiFares.Length; i++)
                {
                    Console.WriteLine($"VendorId        : {taxiFares[i].VendorId:F3}");
                    Console.WriteLine($"RateCode        : {taxiFares[i].RateCode:F3}");
                    Console.WriteLine($"PassengerCount  : {taxiFares[i].PassengerCount:F3}");
                    Console.WriteLine($"TripTime        : {taxiFares[i].TripTime:F3}");
                    Console.WriteLine($"TripDistance    : {taxiFares[i].TripDistance:F3}");
                    Console.WriteLine($"PaymentType     : {taxiFares[i].PaymentType:F3}");
                    Console.WriteLine($"ActualFare      : {taxiFares[i].FareAmount:F3}");
                    Console.WriteLine($"PredictedFare   : {predictions[i].FareAmount:F3}\n");
                }

                OutputTaxiFareForecast(newOutDir, newFileName, taxiFares, predictions, FileFormat.Csv);
            }

            Console.Write("\nSave model (Y/N): ");
            if (Console.ReadLine() == "Y")
                SaveTaxiFareModel(ref mlContext, model, dataView!, outDir, fileName);
        }

        [Feature(instruction: TaxiFareModelInstruction)]
        public static void ForecastTaxiFare(string inFileModel, string inFileData, string outDir, string fileName)
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFileModel, out _);

            var inputData = InputTaxiFareData(ref mlContext, inFileData, FileFormat.Csv);

            var taxiFares = mlContext.Data.CreateEnumerable<TaxiFare>(inputData, false).ToArray();
            var predictions = ConsumeTaxiFareModel(ref mlContext, model, taxiFares);

            Log.Info($"Taxi Fare Forecast");

            for (int i = 0; i < taxiFares.Length; i++)
            {
                Console.WriteLine($"VendorId        : {taxiFares[i].VendorId:F3}");
                Console.WriteLine($"RateCode        : {taxiFares[i].RateCode:F3}");
                Console.WriteLine($"PassengerCount  : {taxiFares[i].PassengerCount:F3}");
                Console.WriteLine($"TripTime        : {taxiFares[i].TripTime:F3}");
                Console.WriteLine($"TripDistance    : {taxiFares[i].TripDistance:F3}");
                Console.WriteLine($"PaymentType     : {taxiFares[i].PaymentType:F3}");
                Console.WriteLine($"ActualFare      : {taxiFares[i].FareAmount:F3}");
                Console.WriteLine($"PredictedFare   : {predictions[i].FareAmount:F3}\n");
            }

            OutputTaxiFareForecast(outDir, fileName, taxiFares, predictions, FileFormat.Csv);
        }

        #region DATA CONNECTION

        private static IDataView? InputTaxiFareData(ref MLContext mlContext, string path, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Csv)
            {
                var dataView = mlContext.Data.LoadFromTextFile<TaxiFare>(path, hasHeader: true, separatorChar: ',');
                return dataView;
            }
            return null;
        }

        private static void OutputTaxiFareForecast(string location, string fileName, TaxiFare[] taxiFares, TaxiFarePrediction[] predictions, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Txt)
            {
                throw new NotImplementedException();
            }

            if (fileFormat == FileFormat.Csv)
            {
                var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
                    new StringDataFrameColumn("VendorId"),
                    new StringDataFrameColumn("RateCode"),
                    new StringDataFrameColumn("PassengerCount"),
                    new StringDataFrameColumn("TripTime"),
                    new StringDataFrameColumn("TripDistance"),
                    new StringDataFrameColumn("PaymentType"),
                    new StringDataFrameColumn("ActualFareAmount"),
                    new StringDataFrameColumn("PredictedFareAmount"),
                });

                for (int i = 0; i < taxiFares.Length; i++)
                {
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("VendorId", $"\"{taxiFares[i].VendorId}\""),
                        new KeyValuePair<string, object?>("RateCode", $"\"{taxiFares[i].RateCode}\""),
                        new KeyValuePair<string, object?>("PassengerCount", $"\"{taxiFares[i].PassengerCount}\""),
                        new KeyValuePair<string, object?>("TripTime", $"\"{taxiFares[i].TripTime}\""),
                        new KeyValuePair<string, object?>("TripDistance", $"\"{taxiFares[i].TripDistance}\""),
                        new KeyValuePair<string, object?>("PaymentType", $"\"{taxiFares[i].PaymentType}\""),
                        new KeyValuePair<string, object?>("ActualFareAmount", $"\"{taxiFares[i].FareAmount}\""),
                        new KeyValuePair<string, object?>("PredictedFareAmount", $"\"{predictions[i].FareAmount}\""),
                    };
                                        
                    Console.WriteLine($"Actual    : {taxiFares[i].FareAmount}");
                    Console.WriteLine($"Predicted : {predictions[i].FareAmount}");
                    Console.WriteLine();
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

        private static ITransformer TrainTaxiFareModel(ref MLContext mlContext, IDataView trainData)
        {
            var pipeline = mlContext.Transforms
                .CopyColumns(inputColumnName: "FareAmount", outputColumnName: "Label")
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "VendorIdEncoded", inputColumnName: "VendorId"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "RateCodeEncoded", inputColumnName: "RateCode"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PaymentTypeEncoded", inputColumnName: "PaymentType"))
                .Append(mlContext.Transforms.Concatenate("Features", "VendorIdEncoded", "RateCodeEncoded", "PassengerCount", "TripDistance", "PaymentTypeEncoded"))
                .Append(mlContext.Regression.Trainers.FastTree());

            var model = pipeline.Fit(trainData);
            return model;
        }

        private static RegressionMetrics EvaluateTaxiFareModel(ref MLContext mlContext, ITransformer model, IDataView testData)
        {
            var predictions = model.Transform(testData);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            return metrics;
        }

        #endregion TRAINING & TESTING

        #region MODEL CONSUMPTION

        private static TaxiFarePrediction[] ConsumeTaxiFareModel(ref MLContext mlContext, ITransformer model, TaxiFare[] taxiFares)
        {
            var predEngine = mlContext.Model.CreatePredictionEngine<TaxiFare, TaxiFarePrediction>(model);
            var taxiFarePredictions = (
                from taxiFare in taxiFares
                let prediction = predEngine.Predict(taxiFare)
                select prediction).ToArray();

            return taxiFarePredictions;
        }

        private static void SaveTaxiFareModel(ref MLContext mlContext, ITransformer model, IDataView dataView, string location, string fileName)
        {
            var path = $"{location}\\Model @{fileName} .zip";
            mlContext.Model.Save(model, dataView.Schema, path);
        }

        #endregion MODEL CONSUMPTION
    }
}

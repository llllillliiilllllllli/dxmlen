using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;

using DxMLEngine.Attributes;
using static Microsoft.ML.ForecastingCatalog;
using Microsoft.Data.Analysis;
using DxMLEngine.Features.Amazon;
using DxMLEngine.Utilities;
using System.Windows.Forms;
using DxMLEngine.Features.UNComtrade;
using System.Reflection;
using System.Data;

namespace DxMLEngine.Features.Forecasting
{
    [Feature]
    internal class Forecasting
    {
        public static void BuildBikeRentalModel()
        {
            ////
            Console.Write("\nEnter input file path: ");
            var inFile = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(inFile))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var outDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(outDir))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output file name: ");
            var outFile = Console.ReadLine()?.Replace(" ", "");

            if (string.IsNullOrEmpty(outFile))
                throw new ArgumentNullException("file name is null or empty");

            ////
            var mlContext = new MLContext();

            var dataset = InputBikeRentalData(ref mlContext, inFile, FileFormat.Mdf);
            var trainData = mlContext.Data.FilterRowsByColumn(dataset, "Year", upperBound: 1);
            var testData = mlContext.Data.FilterRowsByColumn(dataset, "Year", lowerBound: 1);

            var model = TrainModel(ref mlContext, trainData);
            EvaluateModel(ref mlContext, model, testData);

            Console.Write("\nSave results (Y/N): ");
            if (Console.ReadLine() == "Y")
            {
                var predictions = model.Transform(testData);
                var inputData = mlContext.Data.CreateEnumerable<BikeRental>(testData, true).ToArray();
                var outputData = mlContext.Data.CreateEnumerable<BikeRentalPrediction>(predictions, true).ToArray();
                OutputResults(outDir, outFile, inputData, outputData);
            }

            Console.Write("\nSave model (Y/N): ");
            if (Console.ReadLine() == "Y") 
                OutputModel(ref mlContext, model, outDir, outFile);
        }

        public static void ForecastBikeRental()
        {
            Log.Info("ForecastBikeRental");

            ////
            Console.Write("\nEnter input file model: ");
            var inFile = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(inFile))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var outDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(outDir))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output file name: ");
            var outFile = Console.ReadLine()?.Replace(" ", "");

            if (string.IsNullOrEmpty(outFile))
                throw new ArgumentNullException("file name is null or empty");

            Console.Write("\nEnter time horizon: ");
            var horizon = Console.ReadLine();

            if (string.IsNullOrEmpty(horizon))
                throw new ArgumentNullException("horizon is null or empty");

            Console.Write("\nEnter confidence level: ");
            var confidence = Console.ReadLine();

            if (string.IsNullOrEmpty(horizon))
                throw new ArgumentNullException("confidence is null or empty");

            ////
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFile, out _);

            var predEngine = model.CreateTimeSeriesEngine<BikeRental, BikeRentalPrediction>(mlContext);
            var outputData = predEngine.Predict(Convert.ToInt32(horizon), Convert.ToSingle(confidence));

            Console.WriteLine($"\nBike Rental Forecast");
            Console.WriteLine($"=====================");
            for (int i = 0; i < outputData.PredictedRentals!.Length; i++)
            {
                Console.WriteLine($"Prediction : {outputData.PredictedRentals![i]:F3}");
                Console.WriteLine($"Lowerbound : {outputData.LowerBound![i]:F3}");
                Console.WriteLine($"Upperbound : {outputData.UpperBound![i]:F3}");
                Console.WriteLine($"");
            }

            OutputBikeRentalForecast(outDir, outFile, outputData, FileFormat.Txt);
        }

        #region DATA CONNECTION

        private static IDataView? InputBikeRentalData(ref MLContext mlContext, string path, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Mdf)
            {
                var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={path};Integrated Security=True;Connect Timeout=30;";
                var dbLoader = mlContext.Data.CreateDatabaseLoader<BikeRental>();
                var sqlQuery = "SELECT RentalDate, CAST(Year as REAL) as Year, CAST(TotalRentals as REAL) as TotalRentals FROM Rentals";
                var dbSource = new DatabaseSource(SqlClientFactory.Instance, connectionString, sqlQuery);

                var dataView = dbLoader.Load(dbSource);
                return dataView;
            }

            if (fileFormat == FileFormat.Csv)
            {
                throw new NotImplementedException();
            }

            return null;
        }

        private static void OutputBikeRentalForecast(string location, string fileName, BikeRentalPrediction outputData, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Txt)
            {
                var lines = new List<string>();
                for (int i = 0; i < outputData.PredictedRentals!.Length; i++)
                {
                    lines.Add($"Prediction : {outputData.PredictedRentals![i]:F3}");
                    lines.Add($"Lowerbound : {outputData.LowerBound![i]:F3}");
                    lines.Add($"Upperbound : {outputData.UpperBound![i]:F3}");
                    lines.Add($"\n");
                }

                var path = $"{location}\\Datadoc @{fileName} #-------------- .txt";
                File.WriteAllLines(path, lines, encoding: Encoding.UTF8);

                var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
                File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
            }

            if (fileFormat == FileFormat.Csv)
            {
                throw new NotFiniteNumberException();
            }

            if (fileFormat == FileFormat.Json)
            {
                throw new NotFiniteNumberException();
            }
        }

        #endregion DATA CONNECTION

        #region TRAINING & TESTING

        private static ITransformer TrainModel(ref MLContext mlContext, IDataView trainData)
        {
            var pipeline = mlContext.Forecasting.ForecastBySsa(
                inputColumnName: "TotalRentals",
                outputColumnName: "PredictedRentals",
                windowSize: 7,
                seriesLength: 30,
                trainSize: 365,
                horizon: 7,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "LowerBound",
                confidenceUpperBoundColumn: "UpperBound");

            var model = pipeline.Fit(trainData);
            
            return model;
        }

        private static void EvaluateModel(ref MLContext mlContext, ITransformer model, IDataView testData)
        {
            Log.Info($"EvaluateModel");

            var predictions = model.Transform(testData);

            var actualValues = (
                from item in mlContext.Data.CreateEnumerable<BikeRental>(predictions, true)
                select item.TotalRentals).ToArray();

            var predictedValues = (
                from item in mlContext.Data.CreateEnumerable<BikeRentalPrediction>(predictions, true)
                select item.PredictedRentals![0]).ToArray();

            var metrics = actualValues.Zip(predictedValues, (actualValue, predictedValue) => actualValue - predictedValue);

            var MSE = metrics.Average(error => Math.Pow(Convert.ToDouble(error), 2));
            var MAE = metrics.Average(error => Math.Abs(Convert.ToDouble(error)));
            var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(Convert.ToDouble(error), 2)));

            Console.WriteLine($"MSE  : {MSE:F3}");
            Console.WriteLine($"MAE  : {MAE:F3}");
            Console.WriteLine($"RMSE : {RMSE:F3}");
        }

        private static void OutputResults(string location, string fileName, BikeRental[] inputData, BikeRentalPrediction[] outputData)
        {
            Log.Info("OutputBikeRentalForecast");

            var dataFrame = new DataFrame(new List<DataFrameColumn>()
            {
                new StringDataFrameColumn("ActualRental"),
                new StringDataFrameColumn("PredictedRental"),
                new StringDataFrameColumn("LowerBound"),
                new StringDataFrameColumn("UpperBound"),
            });

            for (int i = 0; i < inputData.Length; i++)
            {
                var actualRental = inputData[i].TotalRentals;
                var predictedRental = outputData[i].PredictedRentals![0];
                var lowerBound = outputData[i].LowerBound![0];
                var upperBound = outputData[i].UpperBound![0];
                var dataRow = new List<KeyValuePair<string, object?>>()
                {
                    new KeyValuePair<string, object?>("ActualRental", actualRental),
                    new KeyValuePair<string, object?>("PredictedRental", predictedRental),
                    new KeyValuePair<string, object?>("LowerBound", lowerBound),
                    new KeyValuePair<string, object?>("UpperBound", upperBound),
                };

                Console.WriteLine($"Actual : {actualRental:F3} | Predicted : {predictedRental:F3}");
                dataFrame.Append(dataRow, inPlace: true);
            }

            var path = $"{location}\\Dataset @{fileName} #-------------- .csv";
            DataFrame.WriteCsv(dataFrame, path, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputModel(ref MLContext mlContext, ITransformer model, string location, string fileName)
        {
            Log.Info($"OutputModel");

            var predEngine = model.CreateTimeSeriesEngine<BikeRental, BikeRentalPrediction>(mlContext);
            predEngine.CheckPoint(mlContext, $"{location}\\Model @{fileName} .zip");
        }

        #endregion TRAINING & TESTING

        public static void ForecastTaxiFare()
        {
            throw new NotFiniteNumberException();
            throw new NotFiniteNumberException();
            throw new NotFiniteNumberException();
            throw new NotFiniteNumberException();
            throw new NotFiniteNumberException();
        }
    }
}

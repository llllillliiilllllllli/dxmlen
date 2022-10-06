using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.IO;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using Microsoft.Data.Analysis;

using DxMLEngine.Attributes;
using DxMLEngine.Utilities;

namespace DxMLEngine.Features.Forecasting
{
    [Feature]
    internal class BikeRentalForecast
    {
        #region INSTRUCTION

        private const string BikeRentalModelInstruction =
            "\nInstruction:\n" +
            "\tThis model forecast demand for bike rental for upcoming time periods.\n" +
            "\tPredicted rental is based on at least two year of historical rental data.\n" +
            "\tSingularSpectrumAnalysis decomposes univariate time series into principle components\n" +
            "\tThese components include trends, noise, seasonality, and other influential factors.\n" +
            "\tThe model can predict rental according to time range and confidence levels\n" +

            "\tSource: https://learn.microsoft.com/en-us/dotnet/api/microsoft.ml.transforms.timeseries.ssaforecastingestimator?view=ml-dotnet\n" +
            "\tSource: https://ssa.cf.ac.uk/zhigljavsky/pdfs/SSA/SSA_encyclopedia.pdf";

        #endregion INSTRUCTION

        [Feature(instruction: BikeRentalModelInstruction)]
        public static void BuildBikeRentalModel(string inFile, string outDir, string fileName)
        {
            var mlContext = new MLContext();

            var dataset = InputBikeRentalData(ref mlContext, inFile, FileFormat.Mdf);
            var trainData = mlContext.Data.FilterRowsByColumn(dataset, "Year", upperBound: 1);
            var testData = mlContext.Data.FilterRowsByColumn(dataset, "Year", lowerBound: 1);

            var model = TrainBikeRentalModel(ref mlContext, trainData);
            var metrics = EvaluateBikeRentalModel(ref mlContext, model, testData);
            
            Log.Info($"Regression Metrics");

            Console.WriteLine($"MSE  : {metrics.Item1:F3}");
            Console.WriteLine($"MAE  : {metrics.Item2:F3}");
            Console.WriteLine($"RMSE : {metrics.Item3:F3}");
            Console.WriteLine($"R-Sq : {metrics.Item4:F3}");
            Console.WriteLine($"Loss : {metrics.Item5:F3}");

            Console.Write("\nConsume model (Y/N): ");
            if (Console.ReadLine() == "Y")
            {
                Console.Write("\nEnter output folder path: ");
                var newOutDir = Console.ReadLine()?.Replace("\"", "");

                if (string.IsNullOrEmpty(newOutDir))
                    throw new ArgumentNullException("path is null or empty");

                Console.Write("\nEnter output file name: ");
                var newFileName = Console.ReadLine()?.Replace(" ", "");

                if (string.IsNullOrEmpty(newFileName))
                    throw new ArgumentNullException("file name is null or empty");

                Console.Write("\nEnter horizon: ");
                var inputHorizon = Console.ReadLine();
                int? horizon = null;
                if (!string.IsNullOrEmpty(inputHorizon))
                    horizon = Convert.ToInt32(inputHorizon);

                Console.Write("\nEnter confidence level: ");
                var inputConfidenceLevel = Console.ReadLine();
                float? confidenceLevel = null;
                if (!string.IsNullOrEmpty(inputConfidenceLevel))
                    confidenceLevel = Convert.ToSingle(inputConfidenceLevel);

                var predictions = ConsumeBikeRentalModel(ref mlContext, model, horizon, confidenceLevel);

                Log.Info($"Bike Rental Forecast");
                for (int i = 0; i < predictions.PredictedRentals?.Length; i++)
                {
                    Console.WriteLine($"Horizon         : {horizon}");
                    Console.WriteLine($"Confidence      : {confidenceLevel}");
                    Console.WriteLine($"TimePeriod      : {i + 1}");
                    Console.WriteLine($"PredictedRental : {predictions.PredictedRentals[i]:F3}\n");
                }

                OutputBikeRentalForecast(newOutDir, newFileName, horizon, confidenceLevel, predictions, FileFormat.Csv);
            }

            Console.Write("\nSave model (Y/N): ");
            if (Console.ReadLine() == "Y")
                SaveBikeRentalModel(ref mlContext, model, outDir, fileName);
        }

        [Feature(instruction: BikeRentalModelInstruction)]
        public static void ForecastBikeRental(string inFileModel, string outDir, string fileName, int horizon, float confidenceLevel)
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFileModel, out _);

            var predictions = ConsumeBikeRentalModel(ref mlContext, model, horizon, confidenceLevel);

            Log.Info($"Bike Rental Forecast");
            for (int i = 0; i < predictions.PredictedRentals?.Length; i++)
            {
                Console.WriteLine($"Horizon         : {horizon}");
                Console.WriteLine($"Confidence      : {confidenceLevel}");
                Console.WriteLine($"TimePeriod      : {i + 1}");
                Console.WriteLine($"PredictedRental : {predictions.PredictedRentals[i]:F3}\n");
            }

            OutputBikeRentalForecast(outDir, fileName, horizon, confidenceLevel, predictions, FileFormat.Csv);
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

        private static void OutputBikeRentalForecast(string location, string fileName, int? horizon, float? confidenceLevel, BikeRentalPrediction prediction, FileFormat fileFormat)
        {
            if (prediction.PredictedRentals == null)
                throw new ArgumentNullException("prediction.PredictedRentals == null");

            if (fileFormat == FileFormat.Txt)
            {
                var lines = new List<string>();
                for (int i = 0; i < prediction.PredictedRentals.Length; i++)
                {
                    lines.Add($"Horizon    : {horizon}");
                    lines.Add($"Confidence : {confidenceLevel}");
                    lines.Add($"Period     : {i + 1}");
                    lines.Add($"Prediction : {prediction.PredictedRentals![i]:F3}");
                    lines.Add($"Lowerbound : {prediction.LowerBound![i]:F3}");
                    lines.Add($"Upperbound : {prediction.UpperBound![i]:F3}");
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

        private static ITransformer TrainBikeRentalModel(ref MLContext mlContext, IDataView trainData)
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

        private static (double?, double?, double?, double?, double?) EvaluateBikeRentalModel(ref MLContext mlContext, ITransformer model, IDataView testData)
        {
            var predictions = model.Transform(testData);

            var actualValues = (
                from item in mlContext.Data.CreateEnumerable<BikeRental>(predictions, false)
                select item.TotalRentals).ToArray();

            var predictedValues = (
                from item in mlContext.Data.CreateEnumerable<BikeRentalPrediction>(predictions, false)
                select item.PredictedRentals![0]).ToArray();

            var metrics = actualValues.Zip(predictedValues, (actualValue, predictedValue) => actualValue - predictedValue);

            var MSE = metrics.Average(error => Math.Pow(Convert.ToDouble(error), 2));
            var MAE = metrics.Average(error => Math.Abs(Convert.ToDouble(error)));
            var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(Convert.ToDouble(error), 2)));

            var norminator = actualValues.Zip(predictedValues, (x, y) => (x - actualValues.Average()) * (y - actualValues.Average())).Sum();
            var denominator = Math.Sqrt(actualValues.Sum(x => Math.Pow(x - actualValues.Average(), 2)) * predictedValues.Sum(y => Math.Pow(y - predictedValues.Average(), 2)));
            var RSQ = norminator / denominator;
            double? lossFunction = null; 

            return (MSE, MAE, RMSE, RSQ, lossFunction);
        }

        #endregion TRAINING & TESTING

        #region MODEL CONSUMPTION

        private static BikeRentalPrediction ConsumeBikeRentalModel(ref MLContext mlContext, ITransformer model, int? horizon, float? confidenceLevel)
        {
            var predEngine = model.CreateTimeSeriesEngine<BikeRental, BikeRentalPrediction>(mlContext);
            var prediction = predEngine.Predict(horizon, confidenceLevel);
            return prediction;
        }

        private static void SaveBikeRentalModel(ref MLContext mlContext, ITransformer model, string location, string fileName)
        {
            var predEngine = model.CreateTimeSeriesEngine<BikeRental, BikeRentalPrediction>(mlContext);
            predEngine.CheckPoint(mlContext, $"{location}\\Model @{fileName} .zip");
        }

        #endregion MODEL CONSUMPTION
    }
}

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
using Microsoft.ML.Trainers;
using Microsoft.Data.Analysis;

using DxMLEngine.Attributes;
using DxMLEngine.Utilities;
using DxMLEngine.Objects;
using Tensorflow.Keras.Engine;


namespace DxMLEngine.Features.Recommendation
{
    [Feature]
    internal class MovieRecommendation
    {
        [Feature]
        public static void BuildRecommendationModel(string inFile, string outDir, string fileName)
        {
            var mlContext = new MLContext(seed: 0);

            var dataView = InputMovieRatingData(ref mlContext, inFile, FileFormat.Csv);

            var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            var trainData = trainTestData.TrainSet;
            var testData = trainTestData.TestSet;

            var model = TrainRecommendationModel(ref mlContext, trainData);
            var metrics = EvaluateRecommendationModel(ref mlContext, model, testData);

            Log.Info($"Regression Metrics");

            Console.WriteLine($"MSE  : {metrics.MeanSquaredError:F3}");
            Console.WriteLine($"MAE  : {metrics.MeanAbsoluteError:F3}");
            Console.WriteLine($"RMSE : {metrics.RootMeanSquaredError:F3}");
            Console.WriteLine($"R-Sq : {metrics.RSquared:F3}");

            Console.Write("\nTry model (Y/N): ");
            if (Console.ReadLine() == "Y")
                TryRecommendationModel(ref mlContext, model);

            Console.Write("\nSave model (Y/N): ");
            if (Console.ReadLine() == "Y")
                SaveRecommendationModel(ref mlContext, model, dataView!, outDir, fileName);
        }

        [Feature]
        public static void RecommendMovieToUser(string inFileModel, string inFileData, string outDir, string fileName)
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFileModel, out _);

            var inputData = InputMovieRatingData(ref mlContext, inFileData, FileFormat.Csv);

            var movieRatings = mlContext.Data.CreateEnumerable<MovieRating>(inputData, false).ToArray();
            var predictions = ConsumeRecommendationModel(ref mlContext, model, movieRatings);

            Log.Info($"Movie Recommendation");
            for (int i = 0; i < movieRatings.Length; i++)
            {
                Console.WriteLine($"UserId          : {movieRatings[i].UserId}");
                Console.WriteLine($"MovieId         : {movieRatings[i].MovieId}");
                Console.WriteLine($"ActualRating    : {movieRatings[i].Rating}");
                Console.WriteLine($"PredictedRating : {predictions[i].Rating}\n");
            }

            OutputMovieRecommendation(outDir, fileName, movieRatings, predictions, FileFormat.Csv);
        }

        #region DATA CONNECTION

        private static IDataView? InputMovieRatingData(ref MLContext mlContext, string path, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Txt)
            {
                throw new NotImplementedException();
            }

            if (fileFormat == FileFormat.Csv)
            {
                var dataView = mlContext.Data.LoadFromTextFile<MovieRating>(path, hasHeader: true, separatorChar: ',');
                return dataView;
            }

            if (fileFormat == FileFormat.Json)
            {
                throw new NotImplementedException();
            }

            return null;
        }

        private static void OutputMovieRecommendation(string location, string fileName, MovieRating[] movieRatings, MovieRatingPrediction[] predictions, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Txt)
            {
                throw new NotImplementedException();
            }

            if (fileFormat == FileFormat.Csv)
            {
                var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
                    new StringDataFrameColumn("UserId"),
                    new StringDataFrameColumn("MovieId"),
                    new StringDataFrameColumn("ActualRating"),
                    new StringDataFrameColumn("PredictedRating"),
                });

                for (int i = 0; i < movieRatings.Length; i++)
                {
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("UserId", $"\"{movieRatings[i].UserId}\""),
                        new KeyValuePair<string, object?>("MovieId", $"\"{movieRatings[i].MovieId}\""),
                        new KeyValuePair<string, object?>("ActualRating", $"\"{movieRatings[i].Rating}\""),
                        new KeyValuePair<string, object?>("PredictedRating", $"\"{predictions[i].Rating}\""),
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

        private static ITransformer TrainRecommendationModel(ref MLContext mlContext, IDataView trainData)
        {
            var pipeline = mlContext.Transforms.CopyColumns(inputColumnName: "Rating", outputColumnName: "Label")
                .Append(mlContext.Transforms.Conversion
                    .MapValueToKey(inputColumnName: "UserId", outputColumnName: "UserIdEncoded"))
                .Append(mlContext.Transforms.Conversion
                    .MapValueToKey(inputColumnName: "MovieId", outputColumnName: "MovieIdEncoded"))
                .Append(mlContext.Recommendation().Trainers.MatrixFactorization(
                    new MatrixFactorizationTrainer.Options
                    {
                        MatrixColumnIndexColumnName = "UserIdEncoded",
                        MatrixRowIndexColumnName = "MovieIdEncoded",
                        LabelColumnName = "Label",
                        NumberOfIterations = 20,
                        ApproximationRank = 100
                    }));

            var model = pipeline.Fit(trainData);
            return model;
        }

        private static RegressionMetrics EvaluateRecommendationModel(ref MLContext mlContext, ITransformer model, IDataView testData)
        {
            var predictions = model.Transform(testData);
            var metrics = mlContext.Recommendation().Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");

            return metrics;
        }

        #endregion TRAINING & TESTING

        #region MODEL CONSUMPTION

        private static void TryRecommendationModel(ref MLContext mlContext, ITransformer model)
        {
            Console.Write("\nEnter input file path: ");
            var inFile = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(inFile))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var ourDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(ourDir))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output file name: ");
            var fileName = Console.ReadLine()?.Replace(" ", "");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("file name is null or empty");

            var inputData = InputMovieRatingData(ref mlContext, inFile, FileFormat.Csv);
            if (inputData == null)
                throw new ArgumentNullException("inputData == null");

            var movieRatings = mlContext.Data.CreateEnumerable<MovieRating>(inputData, false).ToArray();
            var predictions = ConsumeRecommendationModel(ref mlContext, model, movieRatings);

            Log.Info($"Movie Recommendation");
            for (int i = 0; i < movieRatings.Length; i++)
            {
                Console.WriteLine($"UserId          : {movieRatings[i].UserId}");
                Console.WriteLine($"MovieId         : {movieRatings[i].MovieId}");
                Console.WriteLine($"ActualRating    : {movieRatings[i].Rating}");
                Console.WriteLine($"PredictedRating : {predictions[i].Rating}\n");
            }

            OutputMovieRecommendation(ourDir, fileName, movieRatings, predictions, FileFormat.Csv);
        }

        private static MovieRatingPrediction[] ConsumeRecommendationModel(ref MLContext mlContext, ITransformer model, MovieRating[] movieRatings)
        {
            var predEngine = mlContext.Model.CreatePredictionEngine<MovieRating, MovieRatingPrediction>(model);
            var predictions = (
                from movieRating in movieRatings
                let prediction = predEngine.Predict(movieRating)
                select prediction).ToArray();

            return predictions;
        }

        private static void SaveRecommendationModel(ref MLContext mlContext, ITransformer model, IDataView dataView, string location, string fileName)
        {
            var path = $"{location}\\Model @{fileName} .zip";
            mlContext.Model.Save(model, dataView.Schema, path);
        }

        #endregion MODEL CONSUMPTION
    }
}

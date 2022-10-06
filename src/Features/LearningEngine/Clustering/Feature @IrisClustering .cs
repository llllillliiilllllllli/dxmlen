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
using DxMLEngine.Features.ClusterAnalysis;

namespace DxMLEngine.Features.IrisCluster
{
    [Feature]
    internal class IrisCluster
    {
        #region INSTRUCTION

        private const string IrisClusterInstruction =
            "\nInstruction:\n" +
            "\tThis model partitions observed data instances (iris features) into different clusters (iris species).\n" +
            "\tAlthough labels are given in training data, the learning method assumes groups are unknown (unsupervised).\n" +
            "\tKMeanClustering model starts forming distinct groups by minimizing within-cluster distances.\n" +
            "\tDistances and the average of distances among observations are also the model's evaluating metrics.\n\n" +
            
            "\tSource: https://learn.microsoft.com/en-us/dotnet/api/microsoft.ml.trainers.kmeanstrainer?view=ml-dotnet\n" +
            "\tSource: https://en.wikipedia.org/wiki/K-means_clustering\n" +
            "\tSource: https://www.microsoft.com/en-us/research/wp-content/uploads/2016/02/ding15.pdf\n" +
            "\tSource: https://github.com/dotnet/machinelearning/blob/main/test/data/iris.data";

        #endregion INSTRUCTION

        [Feature(instruction: IrisClusterInstruction)]
        public static void BuildClusterModel(string inFile, string outDir, string fileName)
        {
            var mlContext = new MLContext(seed: 0);

            var dataView = InputIrisData(ref mlContext, inFile, FileFormat.Csv);

            var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            var trainData = trainTestData.TrainSet;
            var testData = trainTestData.TestSet;

            var model = TrainClusterModel(ref mlContext, trainData);
            var metrics = EvaluateClusterModel(ref mlContext, model, testData);

            Log.Info($"Clustering Analysis Metrics");
            Console.WriteLine($"DaviesBouldinIndex   : {metrics.DaviesBouldinIndex:F3}");
            Console.WriteLine($"AverageDistance      : {metrics.AverageDistance:F3}");
            Console.WriteLine($"NorMutualInformation : {metrics.NormalizedMutualInformation:F3}\n");

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

                var inputData = InputIrisData(ref mlContext, newInFile, FileFormat.Csv);
                if (inputData == null)
                    throw new ArgumentNullException("inputData == null");

                var irisData = mlContext.Data.CreateEnumerable<Iris>(inputData, false).ToArray();
                var predictions = ConsumeClusterModel(ref mlContext, model, irisData);

                Log.Info($"Iris Cluster Analysis");
                for (int i = 0; i < irisData.Length; i++)
                {
                    Console.WriteLine($"SepalLength     : {irisData[i].SepalLength}");
                    Console.WriteLine($"SepalWidth      : {irisData[i].SepalWidth}");
                    Console.WriteLine($"PetalLength     : {irisData[i].PetalLength}");
                    Console.WriteLine($"PetalWidth      : {irisData[i].PetalWidth}");
                    Console.WriteLine($"ActualCluster   : {irisData[i].Species}");
                    Console.WriteLine($"PredictedCluster: {predictions[i].PredictedSpecies}");
                    Console.WriteLine($"AverageDistance : {predictions[i].Distances?.Average()}\n");
                }

                OutputIrisCluster(newOutDir, newFileName, irisData, predictions, FileFormat.Csv);
            }

            Console.Write("\nSave model (Y/N): ");
            if (Console.ReadLine() == "Y")
                SaveClusterModel(ref mlContext, model, dataView!, outDir, fileName);
        }

        [Feature(instruction: IrisClusterInstruction)]
        public static void AnalyzeCluster(string inFileModel, string inFileData, string outDir, string fileName)
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(inFileModel, out _);

            var inputData = InputIrisData(ref mlContext, inFileData, FileFormat.Csv);

            var irisData = mlContext.Data.CreateEnumerable<Iris>(inputData, false).ToArray();
            var predictions = ConsumeClusterModel(ref mlContext, model, irisData);

            Log.Info($"Iris Cluster Analysis");
            for (int i = 0; i < irisData.Length; i++)
            {
                Console.WriteLine($"SepalLength     : {irisData[i].SepalLength}");
                Console.WriteLine($"SepalWidth      : {irisData[i].SepalWidth}");
                Console.WriteLine($"PetalLength     : {irisData[i].PetalLength}");
                Console.WriteLine($"PetalWidth      : {irisData[i].PetalWidth}");
                Console.WriteLine($"ActualCluster   : {irisData[i].Species}");
                Console.WriteLine($"PredictedCluster: {predictions[i].PredictedSpecies}");
                Console.WriteLine($"AverageDistance : {predictions[i].Distances?.Average()}\n");
            }

            OutputIrisCluster(outDir, fileName, irisData, predictions, FileFormat.Csv);
        }

        #region DATA CONNECTION

        private static IDataView? InputIrisData(ref MLContext mlContext, string path, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Txt)
            {
                throw new NotImplementedException();
            }

            if (fileFormat == FileFormat.Csv)
            {
                var dataView = mlContext.Data.LoadFromTextFile<Iris>(path, hasHeader: false, separatorChar: ',');
                return dataView;
            }

            if (fileFormat == FileFormat.Tsv)
            {
                throw new NotImplementedException();
            }

            if (fileFormat == FileFormat.Json)
            {
                throw new NotImplementedException();
            }

            return null;
        }

        private static void OutputIrisCluster(string location, string fileName, Iris[] irisData, IrisPrediction[] predictions, FileFormat fileFormat)
        {
            if (fileFormat == FileFormat.Txt)
            {
                throw new NotImplementedException();
            }

            if (fileFormat == FileFormat.Csv)
            {
                var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
                    new StringDataFrameColumn("SepalLength"),
                    new StringDataFrameColumn("SepalWidth"),
                    new StringDataFrameColumn("PetalLength"),
                    new StringDataFrameColumn("PetalWidth"),
                    new StringDataFrameColumn("ActualCluster"),
                    new StringDataFrameColumn("PredictedCluster"),
                    new StringDataFrameColumn("AverageDistance"),
                });

                for (int i = 0; i < irisData.Length; i++)
                {
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("SepalLength", $"\"{irisData[i].SepalLength}\""),
                        new KeyValuePair<string, object?>("SepalWidth", $"\"{irisData[i].SepalWidth}\""),
                        new KeyValuePair<string, object?>("PetalLength", $"\"{irisData[i].PetalLength}\""),
                        new KeyValuePair<string, object?>("PetalWidth", $"\"{irisData[i].PetalWidth}\""),
                        new KeyValuePair<string, object?>("ActualCluster", $"\"{irisData[i].Species}\""),
                        new KeyValuePair<string, object?>("PredictedCluster", $"\"{predictions[i].PredictedSpecies}\""),
                        new KeyValuePair<string, object?>("AverageDistance", $"\"{predictions[i].Distances?.Average()}\""),
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

        private static ITransformer TrainClusterModel(ref MLContext mlContext, IDataView trainData)
        {
            var pipeline = mlContext.Transforms
                .Concatenate("Features", "SepalLength", "SepalWidth", "PetalLength", "PetalWidth")
                .Append(mlContext.Clustering.Trainers.KMeans("Features", numberOfClusters: 3));

            var model = pipeline.Fit(trainData);
            return model;
        }

        private static ClusteringMetrics EvaluateClusterModel(ref MLContext mlContext, ITransformer model, IDataView testData)
        {
            var predictions = model.Transform(testData);
            var metrics = mlContext.Clustering.Evaluate(predictions);

            return metrics;
        }

        #endregion TRAINING & TESTING

        #region MODEL CONSUMPTION

        private static IrisPrediction[] ConsumeClusterModel(ref MLContext mlContext, ITransformer model, Iris[] irisData)
        {
            var predEngine = mlContext.Model.CreatePredictionEngine<Iris, IrisPrediction>(model);
            var predictions = (
                from iris in irisData
                let prediction = predEngine.Predict(iris)
                select prediction).ToArray();

            return predictions;
        }

        private static void SaveClusterModel(ref MLContext mlContext, ITransformer model, IDataView dataView, string location, string fileName)
        {
            var path = $"{location}\\Model @{fileName} .zip";
            mlContext.Model.Save(model, dataView.Schema, path);
        }

        #endregion MODEL CONSUMPTION
    }
}

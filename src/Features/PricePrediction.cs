using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ML;
using Microsoft.ML.Data;

using MLEngine.Entity;

namespace MLEngine.Features
{
    internal class PricePrediction
    {
        public static void PredictHousingPrice()
        {
            var mlContext = new MLContext();

            Console.Write("Enter input file: ");
            var i_fil = Console.ReadLine();

            var options = new TextLoader.Options();
            options.Separators = new char[1] { ',' };
            options.HasHeader = true; 
            options.TrimWhitespace = true;

            var housingPriceData = mlContext.Data.LoadFromTextFile<HousingPriceData>(i_fil, options);
            var trainTestData = mlContext.Data.TrainTestSplit(housingPriceData, testFraction: 0.2);

            var pipeline = mlContext.Transforms
                .Concatenate("Features", new[] { "Size" })
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Price", maximumNumberOfIterations: 100));

            var model = pipeline.Fit(trainTestData.TrainSet);

            var predictions = model.Transform(trainTestData.TestSet);

            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Price");

            Console.WriteLine($"R-Squared: {metrics.RSquared:0.##}");
            Console.WriteLine($"RMS Error: {metrics.RootMeanSquaredError:0.##}");
        }
    }
}

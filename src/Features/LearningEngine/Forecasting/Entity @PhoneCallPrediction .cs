using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ML.Data;

namespace DxMLEngine.Features.Forecasting
{
    public class PhoneCallPrediction
    {
        /// <summary>
        /// Prediction:
        /// - isAnomaly
        /// - anomalyScore
        /// - magnitude
        /// - expectedValue
        /// - boundaryUnits
        /// - upperBoundary
        /// - lowerBoundary
        /// </summary>

        [VectorType(7), ColumnName("PredictedLabel")]
        public double[]? Prediction { get; set; }
    }
}

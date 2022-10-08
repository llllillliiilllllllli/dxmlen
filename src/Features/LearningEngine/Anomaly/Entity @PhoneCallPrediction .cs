using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ML.Data;

namespace DxMLEngine.Features.AnomalyDetection
{
    public class PhoneCallPrediction : PhoneCall
    {
        /// <summary>
        /// Results:
        /// 0 isAnomaly
        /// 1 anomalyScore
        /// 2 magnitude
        /// 3 expectedValue
        /// 4 boundaryUnits
        /// 5 upperBoundary
        /// 6 lowerBoundary
        /// </summary>

        [ColumnName("PredictedLabel")]
        public bool Prediction { get { return Convert.ToBoolean(Results![0]); } }

        [ColumnName("Score")]
        public float Score { get { return Convert.ToSingle(Results![1]); } }

        [ColumnName("Magnitude")]
        public float Magnitude { get { return Convert.ToSingle(Results![2]); } }

        [ColumnName("ExpectedValue")]
        public float ExpectedValue { get { return Convert.ToSingle(Results![3]); } }

        [VectorType(7), ColumnName("Results")]
        public double[]? Results { set; get; }
    }
}

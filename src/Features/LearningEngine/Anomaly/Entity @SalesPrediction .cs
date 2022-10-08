using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ML.Data;

namespace DxMLEngine.Features.AnomalyDetection
{
    public class SalesPrediction : Sales
    {
        /// <summary>
        /// Results:
        /// 0 alert
        /// 1 score
        /// 2 p-value
        /// </summary>

        [ColumnName("PredictedLabel")]
        public bool Prediction { get { return Convert.ToBoolean(Results![0]); } }

        [ColumnName("Score")]
        public float Score { get { return Convert.ToSingle(Results![1]); } }

        [ColumnName("PValue")]
        public float PValue { get { return Convert.ToSingle(Results![2]); } }

        [VectorType(3), ColumnName("Results")]
        public double[]? Results { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ML.Data;

namespace DxMLEngine.Features.Forecasting
{
    public class BikeRentalPrediction
    {
        public float[]? PredictedRentals { set; get; }
        public float[]? LowerBound { set; get; }
        public float[]? UpperBound { set; get; }
    }
}

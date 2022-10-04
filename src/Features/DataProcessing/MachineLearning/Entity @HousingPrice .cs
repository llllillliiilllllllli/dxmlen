using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.MachineLearning
{
    internal class HousingPriceData
    {
        [LoadColumn(0)]
        public float Size { get; set; }
        [LoadColumn(1)]
        public float Price { get; set; }
    }

    public class HousingPricePrediction
    {
        [ColumnName("Score")]
        public float Price { get; set; }
    }
}

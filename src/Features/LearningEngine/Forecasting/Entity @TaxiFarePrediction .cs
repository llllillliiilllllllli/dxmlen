using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ML.Data;

namespace DxMLEngine.Features.Forecasting
{
    public class TaxiFarePrediction : TaxiFare
    {
        [ColumnName("Score")]
        public float Score;
    }
}

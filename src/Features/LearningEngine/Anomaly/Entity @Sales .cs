using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ML.Data;

namespace DxMLEngine.Features.AnomalyDetection
{
    public class Sales
    {
        [LoadColumn(0), ColumnName("Month")]
        public string? Month;

        [LoadColumn(1), ColumnName("TotalSales")]
        public float TotalSales;

        [LoadColumn(2), ColumnName("Label")]
        public float Label;
    }
}

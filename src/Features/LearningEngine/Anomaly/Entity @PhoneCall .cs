using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ML.Data;

namespace DxMLEngine.Features.AnomalyDetection
{
    public class PhoneCall
    {
        [LoadColumn(0), ColumnName("Timestamp")]
        public string? Timestamp { set; get; }

        [LoadColumn(1), ColumnName("Value")]
        public double PhoneCalls { set; get; }

        [LoadColumn(2), ColumnName("Label")]
        public float Label { set; get; }
    }
}

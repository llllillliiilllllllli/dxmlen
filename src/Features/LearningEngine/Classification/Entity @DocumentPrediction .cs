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

namespace DxMLEngine.Features.Classification
{
    public class DocumentPrediction
    {
        [ColumnName("PredictedLabel")]
        public string? Subject { set; get; }

        [ColumnName("Score")]
        public float[]? Scores { set; get; }
    }
}

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
    public class Review
    {
        [LoadColumn(0), ColumnName("Content")]
        public string? Content { set; get; }

        [LoadColumn(1), ColumnName("Label")]
        public bool Label { set; get; }
    }
}

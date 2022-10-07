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
    public class Document
    {
        [LoadColumn(0), ColumnName("ID")]
        public string? Id { set; get; }

        [LoadColumn(1), ColumnName("Subject")]
        public string? Subject { set; get; }

        [LoadColumn(2), ColumnName("Title")]
        public string? Title { set; get; }

        [LoadColumn(3), ColumnName("Description")]
        public string? Description { set; get; }
    }
}

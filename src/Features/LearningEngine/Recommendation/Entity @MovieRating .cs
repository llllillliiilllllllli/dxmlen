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

namespace DxMLEngine.Features.Recommendation
{
    public class MovieRating
    {
        [LoadColumn(0), ColumnName("UserId")]
        public float UserId { set; get; }

        [LoadColumn(1), ColumnName("MovieId")]
        public float MovieId { set; get; }

        [LoadColumn(2), ColumnName("Rating")]
        public float Rating { set; get; }

        [LoadColumn(3), ColumnName("Timestamp")]
        public float Timestamp { set; get; }
    }
}

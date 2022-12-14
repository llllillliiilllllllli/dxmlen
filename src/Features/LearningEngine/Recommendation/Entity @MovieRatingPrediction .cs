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
    public class MovieRatingPrediction : MovieRating
    {
        [ColumnName("Label")]
        public float Prediction { set; get; }

        [ColumnName("Score")]
        public float Score { set; get; }
    }
}

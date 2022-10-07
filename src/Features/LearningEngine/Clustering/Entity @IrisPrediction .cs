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

namespace DxMLEngine.Features.Clustering
{
    public class IrisPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedSpecies;

        [ColumnName("Score")]
        public float[]? Distances;
    }
}

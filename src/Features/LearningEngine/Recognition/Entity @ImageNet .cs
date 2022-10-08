using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Reflection;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Vision;
using Microsoft.Data.Analysis;
using Microsoft.ML.TensorFlow;

namespace DxMLEngine.Features.Recognition
{
    public class ImageNet
    {
        [LoadColumn(0), ColumnName("ImagePath")]
        public string? ImagePath;

        [LoadColumn(1), ColumnName("Label")]
        public string? Label;
    }    
}

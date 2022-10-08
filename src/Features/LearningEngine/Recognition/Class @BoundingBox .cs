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
    public class YoloBoundingBox
    {
        public string? Label { get; set; }

        public float? Confidence { get; set; }

        public Color? Color { get; set; }

        public Dimensions? Dimensions { get; set; }

        public RectangleF? Rectangle
        {
            get 
            {
                if (Dimensions != null)
                    return new RectangleF(
                        Dimensions.X, Dimensions.Y, Dimensions.Width, Dimensions.Height);
                else 
                    return null; 
            }
        }
    }
}

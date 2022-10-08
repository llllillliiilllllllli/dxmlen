using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Data;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.Data.Analysis;

using DxMLEngine.Attributes;
using DxMLEngine.Utilities;

namespace DxMLEngine.Features.Recognition
{
	public class ProductImagePrediction : ProductImage
	{		
		[ColumnName("PredictedLabel")]
		public string? Prediction { set; get; }
	}
}

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

namespace DxMlEngine.Features.ProductInspection 
{
	public class ProductImagePrediction
	{
		[ColumnName("ImagePath")]
		public string? ImagePath { set; get; }
		
		[ColumnName("PredictedLabel")]
		public string? ImagePath { set; get; }
	}
}

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

namespace DxMlEngine.Features.ProductInspection 
{
	public class ProductImage
	{
		[ColumnName("Image")]
		public byte[]? Image { set; get; }
		
		[ColumnName("ImagePath")]
		public string? ImagePath { set; get; }
		
		[ColumnName("Category")]
		public string? Category { set; get; }	
	}
}

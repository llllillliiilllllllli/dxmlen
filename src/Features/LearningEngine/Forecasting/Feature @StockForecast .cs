using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;

using DxMLEngine.Attributes;
using static Microsoft.ML.ForecastingCatalog;
using Microsoft.Data.Analysis;
using DxMLEngine.Features.Amazon;
using DxMLEngine.Utilities;
using System.Windows.Forms;
using DxMLEngine.Features.UNComtrade;
using System.Reflection;
using System.Data;
using System.IO;

namespace DxMLEngine.Features.Forecasting
{
    [Feature]
    internal class StockForecast
    {
 
    }
}

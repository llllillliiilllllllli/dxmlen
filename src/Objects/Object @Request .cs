using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Objects
{
    internal class Request
    {
        public string? BaseUrl { set; get; }
        public Dictionary<string, string>? Params { set; get; }
    }
}

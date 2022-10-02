using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Objects
{
    internal class WebElement
    {
        internal string? Name { set; get; }
        internal string? Value { set; get; }
        internal string? XPath { set; get; }
        internal string? CSS { set; get; }
    }
}

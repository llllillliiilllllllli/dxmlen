using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Objects
{
    internal class Webpage
    {
        internal string? Title { set; get; }
        internal string? Url { set; get; }
        internal string? PageText { set; get; }
        internal string? PageSource { set; get; }
        internal WebElement[]? Elements { set; get; }
    }
}

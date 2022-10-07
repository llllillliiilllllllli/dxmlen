using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DxMLEngine.Features.GooglePatents
{
    internal enum SearchBy
    {
        Keyword = 0,
        ClassCode = 1,
        PatentCode = 2,
    }
}

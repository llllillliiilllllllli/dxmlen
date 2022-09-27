using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

using DxMLEngine.Functions;
using DxMLEngine.Attributes;
using DxMLEngine.Features.GooglePatents;
using System.Windows.Forms;

namespace DxMLEngine.Features
{
    [Feature]
    internal class Sandbox
    {
        public static void DoNothing()
        {
        }
    }
}

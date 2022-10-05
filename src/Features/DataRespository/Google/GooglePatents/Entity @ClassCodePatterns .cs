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
    internal class ClassCodePatterns
    {
        public const string A = @"[A][\d\w]+[/][\d]+";
        public const string B = @"[B][\d\w]+[/][\d]+";
        public const string C = @"[C][\d\w]+[/][\d]+";
        public const string D = @"[D][\d\w]+[/][\d]+";
        public const string E = @"[E][\d\w]+[/][\d]+";
        public const string F = @"[F][\d\w]+[/][\d]+";
        public const string G = @"[G][\d\w]+[/][\d]+";
        public const string H = @"[H][\d\w]+[/][\d]+";
        public const string Y = @"[Y][\d\w]+[/][\d]+";
    }
}

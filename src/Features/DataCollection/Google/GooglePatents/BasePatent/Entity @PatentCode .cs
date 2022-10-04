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
    internal class PatentCode
    {
        public string? Code { set; get; }
        public string? Keyword { set; get; }
        public string? ClassCode { set; get; }
        public string? Before { set; get; }
        public string? After { set; get; }
        public string? Inventor { set; get; }
        public string? Assignee { set; get; }
        public string? Country { set; get; }
        public string? Language { set; get; }
        public string? Status { set; get; }
        public string? Type { set; get; }
        public string? Litigation { set; get; }
    }
}           

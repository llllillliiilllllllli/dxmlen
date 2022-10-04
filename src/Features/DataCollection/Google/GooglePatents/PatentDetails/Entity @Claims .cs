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
    public class Claims
    {
        public string? Content { set; get; }
        public string? Language { set; get; }

        public Claims(string? content, string? language)
        {
            this.Content = content;
            this.Language = language;
        }   
    }
}

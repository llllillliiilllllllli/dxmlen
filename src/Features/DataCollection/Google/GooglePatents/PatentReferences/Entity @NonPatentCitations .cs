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
    internal class NonPatentCitations
    {
        internal class Citation
        {
            public string Title { set; get; }

            public Citation(string title)
                => this.Title = title;
        }

        public Citation[] Citations;

        public NonPatentCitations(Citation[] citations)
            => this.Citations = citations;
    }
}

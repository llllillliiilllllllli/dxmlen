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
    internal class PatentSummary
    {
        public string AbstractText { set; get; }
        public int NumberOfImage { set; get; }
        public Dictionary<string, string> Classifications { set; get; }  

        public PatentSummary(string abstractText, int numberOfImage, Dictionary<string, string> classifications)
        {
            this.AbstractText = abstractText;
            this.NumberOfImage = numberOfImage;
            this.Classifications = classifications;
        }
    }
}

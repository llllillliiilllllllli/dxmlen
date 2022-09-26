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
    internal class Concepts
    {
        internal class Concept
        {
            public string Name { set; get; }
            public string Image { set; get; }
            public string Sections { set; get; }
            public int Count { set; get; }
            public float QueryMatch { set; get; }

            public Concept(string name, string image, string sections, int count, float queryMatch)
            {
                this.Name = name;
                this.Image = image;
                this.Sections = sections;
                this.Count = count;
                this.QueryMatch = queryMatch;
            }
        }

        public Concepts[] Keywords;

        public Concepts(Concepts[] keywords)
            => this.Keywords = keywords;
    }
}

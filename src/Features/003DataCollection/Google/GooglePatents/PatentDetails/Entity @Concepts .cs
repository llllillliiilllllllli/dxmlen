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
            public string? Id { set; get; }
            public string? Name { set; get; }
            public string? Domain { set; get; }
            public string? ImageSmall { set; get; }
            public string? ImageLarge { set; get; }
            public string? Smiles { set; get; }
            public string? IchiKey { set; get; }
            public float? Similarity { set; get; }
            public string[]? Sections { set; get; }
            public int? Count { set; get; }
            
            public Concept() { }
            public Concept(string id, string name, string domain, string imageSmall, string imageLarge, 
                string smiles, string ichiKey, float similarity, string[] sections, int count)
            {
                this.Id = id;
                this.Name = name;
                this.Domain = domain;
                this.ImageSmall = imageSmall;
                this.ImageLarge = imageLarge;
                this.Smiles = smiles;
                this.IchiKey = ichiKey;
                this.Similarity = similarity;
                this.Sections = sections;
                this.Count = count;
            }
        }

        public Concept[]? KeyConcepts { set; get; }

        public Concepts() { }
        public Concepts(Concept[] keyConcepts)
            => this.KeyConcepts = keyConcepts;
    }
}

using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.OxfordDictionary
{
    public class Inflections
    {
        public string? id { get; set; }
        public Metadata? metadata { get; set; }
        public Result[]? results { get; set; }

        public class Metadata
        {
            public string? operation { get; set; }
            public string? provider { get; set; }
            public string? schema { get; set; }
        }

        public class Result
        {
            public string? id { get; set; }
            public string? language { get; set; }
            public LexicalEntry[]? lexicalEntries { get; set; }
            public string? text { get; set; }

            public class LexicalEntry
            {
                public Inflection[]? inflections { get; set; }
                public string? language { get; set; }
                public LexicalCategory? lexicalCategory { get; set; }
                public GrammaticalFeature[]? grammaticalFeatures { get; set; }

                public class Inflection
                {
                    public GrammaticalFeature[]? grammaticalFeatures { get; set; }
                    public string? inflectedForm { get; set; }
                }

                public class LexicalCategory
                {
                    public string? id { get; set; }
                    public string? text { get; set; }
                }

                public class GrammaticalFeature
                {
                    public string? id { get; set; }
                    public string? text { get; set; }
                    public string? type { get; set; }
                }
            }
        }
    }
}

using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.OxfordDictionary
{
    public class Lemmas
    {
        public Metadata? metadata { get; set; }
        public Result[]? results { get; set; }

        public class Metadata
        {
            public string? provider { get; set; }
        }

        public class Result
        {
            public string? id { get; set; }
            public string? language { get; set; }
            public Lexicalentry[]? lexicalEntries { get; set; }
            public string? word { get; set; }
        }

        public class Lexicalentry
        {
            public Inflectionof[]? inflectionOf { get; set; }
            public string? language { get; set; }
            public Lexicalcategory? lexicalCategory { get; set; }
            public string? text { get; set; }
        }

        public class Lexicalcategory
        {
            public string? id { get; set; }
            public string? text { get; set; }
        }

        public class Inflectionof
        {
            public string? id { get; set; }
            public string? text { get; set; }
        }
    }
}

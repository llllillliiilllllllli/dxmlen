using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.OxfordDictionary
{
    public class Search
    {
        public Metadata? metadata { get; set; }
        public Result[]? results { get; set; }

        public class Metadata
        {
            public string? limit { get; set; }
            public string? offset { get; set; }
            public string? operation { get; set; }
            public string? provider { get; set; }
            public string? schema { get; set; }
            public string? sourceLanguage { get; set; }
            public string? total { get; set; }
        }

        public class Result
        {
            public string? id { get; set; }
            public string? label { get; set; }
            public string? matchString { get; set; }
            public string? matchType { get; set; }
            public string? region { get; set; }
            public float? score { get; set; }
            public string? word { get; set; }
        }
    }
}

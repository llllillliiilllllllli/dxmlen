using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.OxfordDictionary
{
    public class Theaurus
    {
        public string? id { get; set; }
        public Metadata? metadata { get; set; }
        public Result[]? results { get; set; }
        public string? word { get; set; }

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
            public string? type { get; set; }
            public string? word { get; set; }

            public class LexicalEntry
            {
                public Entry[]? entries { get; set; }
                public string? language { get; set; }
                public LexicalCategory? lexicalCategory { get; set; }
                public string? text { get; set; }

                public class Entry
                {
                    public Sens[]? senses { get; set; }

                    public class Sens
                    {
                        public Antonym[]? antonyms { get; set; }
                        public Example[]? examples { get; set; }
                        public string? id { get; set; }
                        public Subsens[]? subsenses { get; set; }
                        public Synonym[]? synonyms { get; set; }

                        public class Antonym
                        {
                            public string? language { get; set; }
                            public string? text { get; set; }
                        }

                        public class Example
                        {
                            public string? text { get; set; }
                        }

                        public class Subsens
                        {
                            public string? id { get; set; }
                            public Synonym[]? synonyms { get; set; }
                            public Register[]? registers { get; set; }

                            public class Register
                            {
                                public string? id { get; set; }
                                public string? text { get; set; }
                            }
                        }

                        public class Synonym
                        {
                            public string? language { get; set; }
                            public string? text { get; set; }
                        }
                    }
                }

                public class LexicalCategory
                {
                    public string? id { get; set; }
                    public string? text { get; set; }
                }
            }
        }
    }
}

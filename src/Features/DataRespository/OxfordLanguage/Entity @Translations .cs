using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.OxfordDictionary
{
    public class Translations
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
                    public Pronunciation[]? pronunciations { get; set; }
                    public Sens[]? senses { get; set; }
                    public Variantform[]? variantForms { get; set; }

                    public class Pronunciation
                    {
                        public string? audioFile { get; set; }
                        public string?[]? dialects { get; set; }
                        public string? phoneticNotation { get; set; }
                        public string? phoneticSpelling { get; set; }
                    }

                    public class Sens
                    {
                        public DatasetCrosslink[]? datasetCrossLinks { get; set; }
                        public string? id { get; set; }
                        public Note[]? notes { get; set; }
                        public Translation[]? translations { get; set; }

                        public class DatasetCrosslink
                        {
                            public string? entry_id { get; set; }
                            public string? language { get; set; }
                            public string? sense_id { get; set; }
                        }

                        public class Note
                        {
                            public string? text { get; set; }
                            public string? type { get; set; }
                        }

                        public class Translation
                        {
                            public string? language { get; set; }
                            public string? text { get; set; }
                            public Register[]? registers { get; set; }
                            public Note[]? notes { get; set; }
                            public Region[]? regions { get; set; }

                            public class Register
                            {
                                public string? id { get; set; }
                                public string? text { get; set; }
                            }

                            public class Region
                            {
                                public string? id { get; set; }
                                public string? text { get; set; }
                            }
                        }
                    }

                    public class Variantform
                    {
                        public string? text { get; set; }
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

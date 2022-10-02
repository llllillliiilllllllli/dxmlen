using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.OxfordDictionary
{
    public class Words
    {
        public Metadata? metadata { get; set; }
        public string? query { get; set; }
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
            public string? type { get; set; }
            public string? word { get; set; }

            public class LexicalEntry
            {
                public Entry[]? entries { get; set; }
                public string? language { get; set; }
                public LexicalCategory? lexicalCategory { get; set; }
                public string? text { get; set; }
                public Derivative[]? derivatives { get; set; }
                public Phras[]? phrases { get; set; }

                public class Entry
                {
                    public GrammaticalFeature[]? grammaticalFeatures { get; set; }
                    public Inflection[]? inflections { get; set; }
                    public Pronunciation[]? pronunciations { get; set; }
                    public Sens[]? senses { get; set; }
                    public string?[]? etymologies { get; set; }
                    public Note[]? notes { get; set; }

                    public class GrammaticalFeature
                    {
                        public string? id { get; set; }
                        public string? text { get; set; }
                        public string? type { get; set; }
                    }

                    public class Pronunciation
                    {
                        public string? audioFile { get; set; }
                        public string?[]? dialects { get; set; }
                        public string? phoneticNotation { get; set; }
                        public string? phoneticSpelling { get; set; }
                    }

                    public class Inflection
                    {
                        public string? inflectedForm { get; set; }
                        public GrammaticalFeature[]? grammaticalFeatures { get; set; }
                        public Pronunciation[]? pronunciations { get; set; }
                    }

                    public class Sens
                    {
                        public string?[]? definitions { get; set; }
                        public DomainClass[]? domainClasses { get; set; }
                        public Example[]? examples { get; set; }
                        public string? id { get; set; }
                        public SemanticClass[]? semanticClasses { get; set; }
                        public string?[]? shortDefinitions { get; set; }
                        public Subsens[]? subsenses { get; set; }
                        public Synonym[]? synonyms { get; set; }
                        public ThesaurusLink[]? thesaurusLinks { get; set; }

                        public class DomainClass
                        {
                            public string? id { get; set; }
                            public string? text { get; set; }
                        }

                        public class Example
                        {
                            public string? text { get; set; }
                            public Note[]? notes { get; set; }
                        }

                        public class SemanticClass
                        {
                            public string? id { get; set; }
                            public string? text { get; set; }
                        }

                        public class Subsens
                        {
                            public string?[]? definitions { get; set; }
                            public DomainClass[]? domainClasses { get; set; }
                            public Example[]? examples { get; set; }
                            public string? id { get; set; }
                            public Note[]? notes { get; set; }
                            public string?[]? shortDefinitions { get; set; }
                        }

                        public class Synonym
                        {
                            public string? language { get; set; }
                            public string? text { get; set; }
                        }

                        public class ThesaurusLink
                        {
                            public string? entry_id { get; set; }
                            public string? sense_id { get; set; }
                        }
                    }

                    public class Note
                    {
                        public string? text { get; set; }
                        public string? type { get; set; }
                    }
                }

                public class LexicalCategory
                {
                    public string? id { get; set; }
                    public string? text { get; set; }
                }

                public class Derivative
                {
                    public string? id { get; set; }
                    public string? text { get; set; }
                }

                public class Phras
                {
                    public string? id { get; set; }
                    public string? text { get; set; }
                }
            }
        }
    }
}

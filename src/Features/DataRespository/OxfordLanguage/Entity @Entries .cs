using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.OxfordDictionary
{
    public class Entries
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
            public Lexicalentry[]? lexicalEntries { get; set; }
            public string? type { get; set; }
            public string? word { get; set; }

            public class Lexicalentry
            {
                public Entry[]? entries { get; set; }
                public string? language { get; set; }
                public Lexicalcategory? lexicalCategory { get; set; }
                public Phras[]? phrases { get; set; }
                public string? text { get; set; }

                public class Entry
                {
                    public string?[]? etymologies { get; set; }
                    public string? homographNumber { get; set; }
                    public Pronunciation[]? pronunciations { get; set; }
                    public Sens[]? senses { get; set; }
                    public Grammaticalfeature[]? grammaticalFeatures { get; set; }

                    public class Pronunciation
                    {
                        public string?[]? dialects { get; set; }
                        public string? phoneticNotation { get; set; }
                        public string? phoneticSpelling { get; set; }
                        public string? audioFile { get; set; }
                    }

                    public class Sens
                    {
                        public string?[]? definitions { get; set; }
                        public DomainClass[]? domainClasses { get; set; }
                        public Example[]? examples { get; set; }
                        public string? id { get; set; }
                        public SemanticClass[]? semanticClasses { get; set; }
                        public string?[]? shortDefinitions { get; set; }
                        public Register[]? registers { get; set; }
                        public Subsens[]? subsenses { get; set; }
                        public Synonym[]? synonyms { get; set; }
                        public Thesauruslink[]? thesaurusLinks { get; set; }
                        public Region[]? regions { get; set; }

                        public class DomainClass
                        {
                            public string? id { get; set; }
                            public string? text { get; set; }
                        }

                        public class Example
                        {
                            public string? text { get; set; }
                            public Register[]? registers { get; set; }
                        }

                        public class SemanticClass
                        {
                            public string? id { get; set; }
                            public string? text { get; set; }
                        }

                        public class Register
                        {
                            public string? id { get; set; }
                            public string? text { get; set; }
                        }

                        public class Subsens
                        {
                            public string?[]? definitions { get; set; }
                            public DomainClass[]? domainClasses { get; set; }
                            public string? id { get; set; }
                            public SemanticClass[]? semanticClasses { get; set; }
                            public string?[]? shortDefinitions { get; set; }
                            public Domain[]? domains { get; set; }
                            public Example[]? examples { get; set; }
                            public Register[]? registers { get; set; }
                            public Region[]? regions { get; set; }
                            public Note[]? notes { get; set; }
                        }

                        public class Synonym
                        {
                            public string? language { get; set; }
                            public string? text { get; set; }
                        }

                        public class Thesauruslink
                        {
                            public string? entry_id { get; set; }
                            public string? sense_id { get; set; }
                        }

                        public class Region
                        {
                            public string? id { get; set; }
                            public string? text { get; set; }
                        }

                        public class Note
                        {
                            public string? text { get; set; }
                            public string? type { get; set; }
                        }

                        public class Domain
                        {
                            public string? id { get; set; }
                            public string? text { get; set; }
                        }
                    }

                    public class Grammaticalfeature
                    {
                        public string? id { get; set; }
                        public string? text { get; set; }
                        public string? type { get; set; }
                    }
                }

                public class Lexicalcategory
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

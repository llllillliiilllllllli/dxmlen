using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DxMLEngine.Features.ScienceDirect
{
    public class ScienceDirectSearch
    {
        [JsonPropertyName("search-results")]
        public SearchResult? searchResult { get; set; }

        public class SearchResult
        {
            [JsonPropertyName("opensearch:totalResults")]
            public string? opensearchtotalResults { get; set; }
            
            [JsonPropertyName("opensearch:startIndex")]
            public string? opensearchstartIndex { get; set; }
            
            [JsonPropertyName("opensearch:itemsPerPage")]
            public string? opensearchitemsPerPage { get; set; }
            
            [JsonPropertyName("opensearch:Query")]
            public OpensearchQuery? opensearchQuery { get; set; }

            [JsonPropertyName("link")]
            public Link[]? link { get; set; }

            [JsonPropertyName("entry")]
            public Entry[]? entry { get; set; }

            public class OpensearchQuery
            {
                [JsonPropertyName("@role")]
                public string? role { get; set; }
                [JsonPropertyName("@searchTerms")]
                public string? searchTerms { get; set; }
                [JsonPropertyName("@startPage")]
                public string? startPage { get; set; }
            }

            public class Link
            {
                [JsonPropertyName("@_fa")]
                public string? _fa { get; set; }
                [JsonPropertyName("@ref")]
                public string? _ref { get; set; }
                [JsonPropertyName("@href")]
                public string? href { get; set; }
                [JsonPropertyName("@type")]
                public string? type { get; set; }
            }

            public class Entry
            {
                [JsonPropertyName("@_fa")]
                public string? _fa { get; set; }
                [JsonPropertyName("load-date")]
                public DateTime? loaddate { get; set; }
                [JsonPropertyName("link")]
                public Link[]? link { get; set; }
                [JsonPropertyName("dc:identifier")]
                public string? dcidentifier { get; set; }
                [JsonPropertyName("prism:url")]
                public string? prismurl { get; set; }
                [JsonPropertyName("dc:title")]
                public string? dctitle { get; set; }
                [JsonPropertyName("dc:creator")]
                public string? dccreator { get; set; }
                [JsonPropertyName("prism:publicationName")]
                public string? prismpublicationName { get; set; }
                [JsonPropertyName("prism:volume")]
                public string? prismvolume { get; set; }
                [JsonPropertyName("prism:coverDate")]
                public string? prismcoverDate { get; set; }
                [JsonPropertyName("prism:startingPage")]
                public string? prismstartingPage { get; set; }
                [JsonPropertyName("prism:endingPage")]
                public string? prismendingPage { get; set; }
                [JsonPropertyName("prism:doi")]
                public string? prismdoi { get; set; }
                [JsonPropertyName("prism:openaccess")]
                public bool openaccess { get; set; }
                [JsonPropertyName("prism:pii")]
                public string? pii { get; set; }
                [JsonPropertyName("authors")]
                public AuthorList? authorList { get; set; }
            }

            public class AuthorList
            {
                [JsonPropertyName("author")]
                public object? authors { get; set; }
            }
        }
    }
}

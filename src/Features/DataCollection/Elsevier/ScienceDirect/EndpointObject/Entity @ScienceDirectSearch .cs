using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static DxMLEngine.Features.ScienceDirect.ScienceDirectSearch.SearchResult;

namespace DxMLEngine.Features.ScienceDirect
{
    public class ScienceDirectSearch
    {
        [JsonPropertyName("search-results")]
        public SearchResult? Result { get; set; }

        public class SearchResult
        {
            [JsonPropertyName("opensearch:totalResults")]
            public string? OpenSearchTotalResults { get; set; }
            
            [JsonPropertyName("opensearch:startIndex")]
            public string? OpenSearchStartIndex { get; set; }
            
            [JsonPropertyName("opensearch:itemsPerPage")]
            public string? OpenSearchItemsPerPage { get; set; }
            
            [JsonPropertyName("opensearch:Query")]
            public OpensearchQuery? OpenSearchQuery { get; set; }

            [JsonPropertyName("link")]
            public Link[]? Link { get; set; }

            [JsonPropertyName("entry")]
            public Entry[]? Entry { get; set; }
        }

        public class OpensearchQuery
        {
            [JsonPropertyName("@role")]
            public string? Role { get; set; }
            [JsonPropertyName("@searchTerms")]
            public string? SearchTerms { get; set; }
            [JsonPropertyName("@startPage")]
            public string? StartPage { get; set; }
        }

        public class Link
        {
            [JsonPropertyName("@_fa")]
            public string? _fa { get; set; }
            [JsonPropertyName("@ref")]
            public string? Ref { get; set; }
            [JsonPropertyName("@href")]
            public string? Href { get; set; }
            [JsonPropertyName("@type")]
            public string? Type { get; set; }
        }

        public class Entry
        {
            [JsonPropertyName("@_fa")]
            public string? _fa { get; set; }
            [JsonPropertyName("load-date")]
            public DateTime? Loaddate { get; set; }
            [JsonPropertyName("link")]
            public Link[]? Link { get; set; }
            [JsonPropertyName("dc:identifier")]
            public string? Identifier { get; set; }
            [JsonPropertyName("prism:url")]
            public string? Url { get; set; }
            [JsonPropertyName("dc:title")]
            public string? Title { get; set; }
            [JsonPropertyName("dc:creator")]
            public string? Creator { get; set; }
            [JsonPropertyName("prism:publicationName")]
            public string? PublicationName { get; set; }
            [JsonPropertyName("prism:volume")]
            public string? Volume { get; set; }
            [JsonPropertyName("prism:coverDate")]
            public string? CoverDate { get; set; }
            [JsonPropertyName("prism:startingPage")]
            public string? StartingPage { get; set; }
            [JsonPropertyName("prism:endingPage")]
            public string? EndingPage { get; set; }
            [JsonPropertyName("prism:doi")]
            public string? Doi { get; set; }
            [JsonPropertyName("prism:openaccess")]
            public bool OpenAccess { get; set; }
            [JsonPropertyName("prism:pii")]
            public string? Pii { get; set; }
            [JsonPropertyName("authors")]
            public AuthorList? AuthorList { get; set; }
        }

        public class AuthorList
        {
            [JsonPropertyName("author")]
            public object? Authors { get; set; }
        }
    }
}

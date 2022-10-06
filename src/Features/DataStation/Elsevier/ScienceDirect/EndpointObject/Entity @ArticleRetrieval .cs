using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DxMLEngine.Features.ScienceDirect
{
    public class ArticleRetrieval
    {
        [JsonPropertyName("full-text-retrieval-response")]
        public FullResponse? ResponseText { get; set; }

        public class FullResponse
        {
            [JsonPropertyName("scopus-eid")]
            public string? ScopusEid { get; set; }
            [JsonPropertyName("originalText")]
            public string? OriginalText { get; set; }
            [JsonPropertyName("scopus-id")]
            public string? ScopusId { get; set; }
            [JsonPropertyName("pubmed-id")]
            public string? PubmedId { get; set; }
            [JsonPropertyName("coredata")]
            public CoreData? CoreData { get; set; }
            [JsonPropertyName("objects")]
            public Objects? Objects { get; set; }
            [JsonPropertyName("link")]
            public Link? Link { get; set; }
        }

        public class CoreData
        {
            [JsonPropertyName("eid")]
            public string? Eid { get; set; }
            [JsonPropertyName("dc:description")]
            public string? Description { get; set; }
            [JsonPropertyName("openArchiveArticle")]
            public string? OpenArchiveArticle { get; set; }
            [JsonPropertyName("prism:coverDate")]
            public string? CoverDate { get; set; }
            [JsonPropertyName("openaccessUserLicense")]
            public string? OpenAccessUserLicense { get; set; }
            [JsonPropertyName("prism:aggregationType")]
            public string? AggregationType { get; set; }
            [JsonPropertyName("prism:url")]
            public string? Url { get; set; }
            [JsonPropertyName("dc:creator")]
            public Creator[]? CreatorList { get; set; }
            [JsonPropertyName("link")]
            public Link[]? Link { get; set; }
            [JsonPropertyName("dc:format")]
            public string? Format { get; set; }
            [JsonPropertyName("openaccessType")]
            public string? OpenAccessType { get; set; }
            [JsonPropertyName("pii")]
            public string? Pii { get; set; }
            [JsonPropertyName("prism:volume")]
            public string? Volume { get; set; }
            [JsonPropertyName("prism:publisher")]
            public string? Publisher { get; set; }
            [JsonPropertyName("dc:title")]
            public string? Title { get; set; }
            [JsonPropertyName("prism:copyright")]
            public string? Copyright { get; set; }
            [JsonPropertyName("openaccess")]
            public string? OpenAccess { get; set; }
            [JsonPropertyName("prism:issn")]
            public string? Issn { get; set; }
            [JsonPropertyName("dcterms:subject")]
            public Subject[]? Subjects { get; set; }
            [JsonPropertyName("openaccessArticle")]
            public string? OpenAccessArticle { get; set; }
            [JsonPropertyName("prism:publicationName")]
            public string? PublicationName { get; set; }
            [JsonPropertyName("openaccessSponsorType")]
            public string? OpenAccessSponsorType { get; set; }
            [JsonPropertyName("prism:pageRange")]
            public string? PageRange { get; set; }
            [JsonPropertyName("prism:endingPage")]
            public string? EndingPage { get; set; }
            [JsonPropertyName("pubType")]
            public string? PubType { get; set; }
            [JsonPropertyName("prism:coverDisplayDate")]
            public string? CoverDisplayDate { get; set; }
            [JsonPropertyName("prism:doi")]
            public string? Doi { get; set; }
            [JsonPropertyName("prism:startingPage")]
            public string? StartingPage { get; set; }
            [JsonPropertyName("dc:identifier")]
            public string? Identifier { get; set; }
            [JsonPropertyName("openaccessSponsorName")]
            public string? OpenAccessSponsorName { get; set; }
        }

        public class Creator
        {
            [JsonPropertyName("@_fa")]
            public string? _fa { get; set; }
            [JsonPropertyName("$")]
            public string? Name { get; set; }
        }

        public class Subject
        {
            [JsonPropertyName("@_fa")]
            public string? _fa { get; set; }
            [JsonPropertyName("$")]
            public string? Term { get; set; }
        }

        public class Objects
        {
            [JsonPropertyName("objects")]
            public Object[]? objects { get; set; }
        }

        public class Object
        {
            [JsonPropertyName("@category")]
            public string? Category { get; set; }
            [JsonPropertyName("@height")]
            public string? Height { get; set; }
            [JsonPropertyName("@width")]
            public string? Width { get; set; }
            [JsonPropertyName("@_fa")]
            public string? _fa { get; set; }
            [JsonPropertyName("$")]
            public string? _ { get; set; }
            [JsonPropertyName("@multimediatype")]
            public string? MultimediaType { get; set; }
            [JsonPropertyName("@type")]
            public string? Type { get; set; }
            [JsonPropertyName("@size")]
            public string? Size { get; set; }
            [JsonPropertyName("@ref")]
            public string? Ref { get; set; }
            [JsonPropertyName("@mimetype")]
            public string? MimeType { get; set; }
        }

        public class Link
        {
            [JsonPropertyName("@_fa")]
            public string? _fa { get; set; }
            [JsonPropertyName("@ref")]
            public string? rel { get; set; }
            [JsonPropertyName("@href")]
            public string? href { get; set; }
        }
    }
}
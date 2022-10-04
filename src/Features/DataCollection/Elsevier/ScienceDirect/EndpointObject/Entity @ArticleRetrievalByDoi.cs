using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.ScienceDirect
{
    public class ArticleRetrievalByDoi
    {
        public FullTextRetrievalResponse? fulltextretrievalresponse { get; set; }

        public class FullTextRetrievalResponse
        {
            public string? scopuseid { get; set; }
            public string? originalText { get; set; }
            public string? scopusid { get; set; }
            public string? pubmedid { get; set; }
            public Coredata? coredata { get; set; }
            public Objects? objects { get; set; }
            public Link? link { get; set; }
        }

        public class Coredata
        {
            public string? eid { get; set; }
            public string? dcdescription { get; set; }
            public string? openArchiveArticle { get; set; }
            public string? prismcoverDate { get; set; }
            public string? openaccessUserLicense { get; set; }
            public string? prismaggregationType { get; set; }
            public string? prismurl { get; set; }
            public DcCreator[]? dccreator { get; set; }
            public Link[]? link { get; set; }
            public string? dcformat { get; set; }
            public string? openaccessType { get; set; }
            public string? pii { get; set; }
            public string? prismvolume { get; set; }
            public string? prismpublisher { get; set; }
            public string? dctitle { get; set; }
            public string? prismcopyright { get; set; }
            public string? openaccess { get; set; }
            public string? prismissn { get; set; }
            public DctermsSubject[]? dctermssubject { get; set; }
            public string? openaccessArticle { get; set; }
            public string? prismpublicationName { get; set; }
            public string? openaccessSponsorType { get; set; }
            public string? prismpageRange { get; set; }
            public string? prismendingPage { get; set; }
            public string? pubType { get; set; }
            public string? prismcoverDisplayDate { get; set; }
            public string? prismdoi { get; set; }
            public string? prismstartingPage { get; set; }
            public string? dcidentifier { get; set; }
            public string? openaccessSponsorName { get; set; }

            public class DcCreator
            {
                public string? _fa { get; set; }
                public string? _ { get; set; }
            }

            public class DctermsSubject
            {
                public string? _fa { get; set; }
                public string? _ { get; set; }
            }
        }

        public class Objects
        {
            public Object[]? _object { get; set; }

            public class Object
            {
                public string? category { get; set; }
                public string? height { get; set; }
                public string? width { get; set; }
                public string? _fa { get; set; }
                public string? _ { get; set; }
                public string? multimediatype { get; set; }
                public string? type { get; set; }
                public string? size { get; set; }
                public string? _ref { get; set; }
                public string? mimetype { get; set; }
            }
        }

        public class Link
        {
            public string? _fa { get; set; }
            public string? rel { get; set; }
            public string? href { get; set; }
        }
    }
}

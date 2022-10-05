using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.ScienceDirect
{
    public class Request
    {
        public string? Id { set; get; }

        private const string API_KEY = "3fb59b48b0bad7a65c3777993e503fc1";
        private const string EP_SEARCH_SCIENCEDIRECT = "https://api.elsevier.com/content/search/sciencedirect?{parameters}";    // params: &query={query}&apiKey={apiKey}&httpAccept={httpAccept}
        private const string EP_RETRIEVAL_ARTICLE_DOI = "https://api.elsevier.com/content/article/doi/{doi}?{parameters}";      // params: &apiKey={apiKey}&httpAccept={httpAccept}
        private const string EP_RETRIEVAL_ARTICLE_EID = "https://api.elsevier.com/content/article/eid/{eid}?{parameters}";      // params: &apiKey={apiKey}&httpAccept={httpAccept}
        private const string EP_RETRIEVAL_ARTICLE_PII = "https://api.elsevier.com/content/article/pii/{pii}?{parameters}";      // params: &apiKey={apiKey}&httpAccept={httpAccept}
        private const string EP_RETRIEVAL_ARTICLE_PUBMED_ID = "https://api.elsevier.com/content/article/pubmed_id/{pubmed_id}?{parameters}";        // params: &apiKey={apiKey}&httpAccept={httpAccept}
        private const string EP_RETRIEVAL_ARTICLE_ENTITLEMENT_DOI = "https://api.elsevier.com/content/article/entitlement/doi/{doi}?{parameters}";  // params: &apiKey={apiKey}&httpAccept={httpAccept}
        private const string EP_RETRIEVAL_ARTICLE_ENTITLEMENT_EID = "https://api.elsevier.com/content/article/entitlement/eid/{eid}?{parameters}";  // params: &apiKey={apiKey}&httpAccept={httpAccept}
        private const string EP_RETRIEVAL_ARTICLE_ENTITLEMENT_PII = "https://api.elsevier.com/content/article/entitlement/pii/{pii}?{parameters}";  // params: &apiKey={apiKey}&httpAccept={httpAccept}
        private const string EP_RETRIEVAL_ARTICLE_ENTITLEMENT_PUBMED_ID = "https://api.elsevier.com/content/article/entitlement/pubmed_id/{pubmed_id}?{parameters}";    // params: &apiKey={apiKey}&httpAccept={httpAccept}
        private const string EP_RETRIEVAL_ARTICLE_ENTITLEMENT_SCOPUS_ID = "https://api.elsevier.com/content/article/entitlement/scopus_id/{scopus_id}?{parameters}";    // params: &apiKey={apiKey}&httpAccept={httpAccept}
        private const string EP_RETRIEVAL_OBJECT_DOI = "";
        private const string EP_RETRIEVAL_OBJECT_DOI_REF = "";
        private const string EP_RETRIEVAL_OBJECT_DOI_REF_HIGH = "";
        private const string EP_RETRIEVAL_OBJECT_DOI_REF_STANDARD = "";
        private const string EP_RETRIEVAL_OBJECT_DOI_REF_THUMBNAIL = "";
        private const string EP_RETRIEVAL_OBJECT_EID = "";
        private const string EP_RETRIEVAL_OBJECT_PII = "";
        private const string EP_RETRIEVAL_OBJECT_PII_REF = "";
        private const string EP_RETRIEVAL_OBJECT_PII_REF_HIGH = "";
        private const string EP_RETRIEVAL_OBJECT_PII_REF_STANDARD = "";
        private const string EP_RETRIEVAL_OBJECT_PII_REF_THUMBNAIL = "";
        private const string EP_RETRIEVAL_OBJECT_PUBMED_ID = "";
        private const string EP_RETRIEVAL_OBJECT_PUBMED_ID_REF = "";
        private const string EP_RETRIEVAL_OBJECT_PUBMED_ID_REF_HIGH = "";
        private const string EP_RETRIEVAL_OBJECT_PUBMED_ID_REF_STANDARD = "";
        private const string EP_RETRIEVAL_OBJECT_PUBMED_ID_REF_THUMBNAIL = "";
        private const string EP_RETRIEVAL_OBJECT_SCOPUS_ID = "";
        private const string EP_RETRIEVAL_OBJECT_SCOPUS_ID_REF = "";
        private const string EP_RETRIEVAL_OBJECT_SCOPUS_ID_REF_HIGH = "";
        private const string EP_RETRIEVAL_OBJECT_SCOPUS_ID_REF_STANDARD = "";
        private const string EP_RETRIEVAL_OBJECT_SCOPUS_ID_REF_THUMBNAIL = "";
        private const string EP_METADATA_ARTICLE = "https://api.elsevier.com/content/metadata/article?{parameters}";    // params: &query={query}&apiKey={apiKey}&httpAccept={httpAccept}

        public string BaseEndpoint { get { return "https://api.elsevier.com"; } }
        public string ScienceDirectSearchUrl { get { return ConfigureScienceDirectSearchUrl(); } }
        public string ArticleRetrievalUrl { get { return ConfigureArticleRetrievalUrl(); } }

        internal string? Query { set; get; }
        internal string? ApiKey { set; get; }
        internal string? Doi { set; get; }
        internal string? Eid { set; get; }
        internal string? Pii { set; get; }
        internal string? PubmedId { set; get; }
        internal string? ScopusId { set; get; }
        internal string? HttpAccept { set; get; }

        internal RetrieveBy? RetrieveBy { set; get; }

        private string ConfigureScienceDirectSearchUrl()
        {
            var parameters = "";
            if (Query != null) parameters += $"&query={Query.Replace(" ", "%20")}";
            if (ApiKey != null) parameters += $"&apiKey={ApiKey}";
            if (HttpAccept != null) parameters += $"&httpAccept={HttpAccept}";

            return EP_SEARCH_SCIENCEDIRECT.Replace("{parameters}", parameters).Replace("?&", "?");
        }

        private string ConfigureArticleRetrievalUrl()
        {
            var parameters = "";
            if (ApiKey != null) parameters += $"&apiKey={ApiKey}";
            if (HttpAccept != null) parameters += $"&httpAccept={HttpAccept}";

            switch (RetrieveBy)
            {
                case Features.ScienceDirect.RetrieveBy.Doi:
                    return EP_RETRIEVAL_ARTICLE_DOI
                        .Replace("{doi}", Doi)
                        .Replace("{parameters}", parameters)
                        .Replace("?&", "?");

                case Features.ScienceDirect.RetrieveBy.Eid:
                    return EP_RETRIEVAL_ARTICLE_EID
                        .Replace("{eid}", Eid)
                        .Replace("{parameters}", parameters)
                        .Replace("?&", "?");

                case Features.ScienceDirect.RetrieveBy.Pii:
                    return EP_RETRIEVAL_ARTICLE_PII
                        .Replace("{pii}", Pii)
                        .Replace("{parameters}", parameters)
                        .Replace("?&", "?");

                case Features.ScienceDirect.RetrieveBy.PubMedId:
                    return EP_RETRIEVAL_ARTICLE_PUBMED_ID
                        .Replace("{pubmed_id}", PubmedId)
                        .Replace("{parameters}", parameters)
                        .Replace("?&", "?");

                default:
                    return "";
            }
        }
    }
}

/// ====================================================================================
/// ScienceDirect API by Elsevier
/// ScienceDirect is Elsevier's premier scientific platform for millions of articles.
/// ScienceDirect API shows peer-reviewed full-text content from scholarly publications.
/// Non-commercial use of the API is free of charge in accordance to Elsevier policies.
/// 
/// The API provides three basic functions: search, metadata, retrieval.
/// Data from ScienceDirect API can be used for several purposes:
/// . IRs and CRIS systems (including VIVO)
/// . Text mining across published contents
/// . Federated search using search engines
/// . Obtained and process article info
/// . Obtained and process journal info
/// 
/// Source: https://dev.elsevier.com/sd_apis.html
/// Source: https://dev.elsevier.com/sd_apis_use_cases.html
/// Source: https://dev.elsevier.com/sciencedirect.html
/// Source: https://dev.elsevier.com/sd_api_spec.html
/// ====================================================================================

using DxMLEngine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace DxMLEngine.Features.ScienceDirect
{
    internal class Endpoint
    {
        public string? Id { set; get; }

        #region REQUEST

        private const string API_KEY = "3fb59b48b0bad7a65c3777993e503fc1";

        private const string EP_SEARCH_SCIENCEDIRECT = "https://api.elsevier.com/content/search/sciencedirect?{parameters}";    // params: &query={query}&apiKey={apiKey}&httpAccept={httpAccept}
        private const string EP_RETRIEVAL_ARTICLE_DOI = "https://api.elsevier.com/content/article/doi/{doi}?{parameters}";      // params: &apiKey={apiKey}&httpAccept={httpAccept}
        private const string EP_RETRIEVAL_ARTICLE_EID = "https://api.elsevier.com/content/article/eid/{eid}?{parameters}";      // params: &apiKey={apiKey}&httpAccept={httpAccept}
        private const string EP_RETRIEVAL_ARTICLE_PII = "https://api.elsevier.com/content/article/pii/{pii}?{parameters}";      // params: &apiKey={apiKey}&httpAccept={httpAccept}
        private const string EP_RETRIEVAL_ARTICLE_PUDMED_ID = "https://api.elsevier.com/content/article/pubmed_id/{pubmed_id}?{parameters}";        // params: &apiKey={apiKey}&httpAccept={httpAccept}
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
        public string ScienceDirectSearchEndpoint { get { return ConfigureScienceDirectSearchEndpoint(); } }
        public string ArticleRetrievalByDoiEndpoint { get { return ConfigureArticleRetrievalByDoiEndpoint(); } }
        public string ArticleRetrievalByEidEndpoint { get { return ConfigureArticleRetrievalByEidEndpoint(); } }
        public string ArticleRetrievalByPiiEndpoint { get { return ConfigureArticleRetrievalByPiiEndpoint(); } }
        public string ArticleRetrievalByPubmedIdEndpoint { get { return ConfigureArticleRetrievalByPubmedIdEndpoint(); } }
        
        internal string? Query { set; get; }
        internal string? ApiKey { set; get; }
        internal string? Doi { set; get; }
        internal string? Eid { set; get; }
        internal string? Pii { set; get; }
        internal string? PubmedId { set; get; }
        internal string? ScopusId { set; get; }
        internal string? HttpAccept { set; get; }

        private string ConfigureScienceDirectSearchEndpoint()
        {
            var parameters = "";
            if (Query != null) parameters += $"&query={Query.Replace(" ", "%20")}";
            if (ApiKey != null) parameters += $"&apiKey={ApiKey}";
            if (HttpAccept != null) parameters += $"&httpAccept={HttpAccept}";
            
            return EP_SEARCH_SCIENCEDIRECT.Replace("{parameters}", parameters).Replace("?&", "?");
        }

        private string ConfigureArticleRetrievalByDoiEndpoint()
        {
            return "";
        }

        private string ConfigureArticleRetrievalByEidEndpoint()
        {
            return "";
        }

        private string ConfigureArticleRetrievalByPiiEndpoint()
        {
            return "";
        }

        private string ConfigureArticleRetrievalByPubmedIdEndpoint()
        {
            return "";
        }

        #endregion REQUEST

        #region RESPONSE

        //public string? Response;

        public string? Response { set; get; }

        public ScienceDirectSearch? ScienceDirectSearch { get { return Deserialize<ScienceDirectSearch>(); } }
        
        public ArticleRetrievalByDoi? ArticleRetrievalByDoi { get { return Deserialize<ArticleRetrievalByDoi>(); } }

        private T? Deserialize<T>() where T : class
        {
            if (Response == null) return null;
            var options = new JsonSerializerOptions() { WriteIndented = true };
            return JsonSerializer.Deserialize<T>(Response, options);
        }

        #endregion RESPONSE
    }
}

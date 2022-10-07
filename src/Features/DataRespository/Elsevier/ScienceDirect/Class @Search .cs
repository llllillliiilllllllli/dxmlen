using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DxMLEngine.Features.ScienceDirect
{
    internal class Search
    {
        public string? Id { set; get; }

        #region REQUEST 

        private const string URL_SEARCH_PAGE = "https://www.sciencedirect.com/search?qs={qs}";
        private const string URL_ARTICLE_PAGE = "https://www.sciencedirect.com/science/article/abs/pii/{pii}";
        private const string URL_JOURNAL_PAGE = "https://www.sciencedirect.com/journal/{journal}";
        private const string URL_AUTHOR_PAGE = "https://www.scopus.com/authid/detail.uri?authorId={authorId}";

        public string SearchUrl { get { return ConfigureSearchUrl(); } }
        public string ArticleUrl { get { return ConfigureArticleUrl(); } }
        public string JournalUrl { get { return ConfigureJournalUrl(); } }
        public string AuthorUrl { get { return ConfigureAuthorUrl(); } }

        public string? Query { set; get; }
        public string? Pii { set; get; }
        public string? Journal { set; get; }
        public string? AuthorId { set; get; }

        private string ConfigureSearchUrl()
        {
            return "";
        }
        
        private string ConfigureArticleUrl()
        {
            return "";
        }
        
        private string ConfigureJournalUrl()
        {
            return "";
        }
        
        private string ConfigureAuthorUrl()
        {
            return "";
        }

        #endregion REQUEST 

        #region RESPONSE

        public string? PageText { set; get; }
        public string? PageSource { set; get; }

        #endregion RESPONSE
    }
}

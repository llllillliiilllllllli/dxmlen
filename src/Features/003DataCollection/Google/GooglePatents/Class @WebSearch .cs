using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DxMLEngine.Features.GooglePatents
{
    internal class WebSearch
    {
        public string? Id { set; get; }

        #region REQUEST

        public string Domain { get { return "https://patents.google.com/"; } }
        public string SearchUrl { get { return ConfigureSearchUrl(); } }
        public string PatentUrl { get { return ConfigurePatentUrl(); } }
        
        private const string URL_SEARCH_PAGE = "https://patents.google.com/?{parameters}";
        private const string URL_PATENT_PAGE = "https://patents.google.com/patent/{patentCode}";

        internal string? Keyword { set; get; }
        internal string? ClassCode { set; get; }
        internal string? PatentCode { set; get; }

        internal string? Before { set; get; }
        internal string? After { set; get; }
        internal string? Inventor { set; get; }
        internal string? Assignee { set; get; }

        internal string? Country { set; get; }
        internal string? Language { set; get; }
        internal string? Status { set; get; }
        internal string? Type { set; get; }
        internal string? Litigation { set; get; }

        internal string? PageNumber { set; get; }

        internal SearchBy? SearchBy { set; get; }
        internal int? NumberOfPages { set; get; }

        private string ConfigureSearchUrl()
        {
            /// ====================================================================================
            /// configure search url based on given domain, query, and parameters
            /// 
            /// >>> param:  SearchBy    # search by option to configure search url
            ///             
            /// >>> funct:  1       # update search query using selected search by method
            /// >>> funct:  2       # update other parameters based on properties of search instance
            /// >>> funct:  3       # assign arguments to parameters in search url where not null
            /// ====================================================================================

            ////0
            var parameters = "";
            switch (SearchBy)
            {
                case Features.GooglePatents.SearchBy.PatentCode:
                    parameters += $"&q={PatentCode}";
                    break;
                case Features.GooglePatents.SearchBy.ClassCode:
                    parameters += $"&q={ClassCode}";
                    break;                
                case Features.GooglePatents.SearchBy.Keyword:
                    parameters += $"&q={Keyword?.Replace(" ", "+")}";
                    break;                
                default:
                    parameters += $"";
                    break;
            }

            ////1
            if (Before != null) parameters += $"&before={Before}";
            if (After != null) parameters += $"&after={After}";
            if (Inventor != null) parameters += $"&inventor={Inventor}";
            if (Assignee != null) parameters += $"&assignee={Assignee}";
            if (Country != null) parameters += $"&country={Country}";
            if (Language != null) parameters += $"&language={Language}";
            if (Status != null) parameters += $"&status={Status}";
            if (Type != null) parameters += $"&type={Type}";
            if (Litigation != null) parameters += $"&litigation={Litigation}";
            if (PageNumber != null) parameters += $"&page={PageNumber}";

            return URL_SEARCH_PAGE.Replace("{parameters}", parameters).Replace("?&", "?");
        }

        private string ConfigurePatentUrl()
        {
            return URL_PATENT_PAGE.Replace("{patentCode}", PatentCode);
        }

        #endregion REQUEST

        #region RESPONSE

        public string? PageText { set; get; }
        public string? PageSource { set; get; }

        #endregion RESPONSE
    }
}

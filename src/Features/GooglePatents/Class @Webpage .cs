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
    internal class Webpage
    {
        public string Domain { get { return "https://patents.google.com/"; } }
        public string SearchUrl { get { return ConfigureSearchUrl(); } }
        public string PatentUrl { get { return ConfigurePatentUrl(); } }
        
        public string? PageText { set; get; }
        public string? PageSource { set; get; }

        private const string URL_SEARCH_PAGE = "https://patents.google.com/?{parameters}";
        private const string URL_PATENT_PAGE = "https://patents.google.com/patent/{patentCode}";

        internal Dictionary<string, string?> Parameters { set; get; }

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

        internal SearchBy searchBy { set; get; }

        public Webpage() 
        {
            Parameters = new Dictionary<string, string?>()
            {
                {"&q=", null },
                {"&before=", Before },
                {"&after=", After },
                {"&inventor=", Inventor },
                {"&assignee=", Assignee },
                {"&country=", Country },
                {"&language=", Language },
                {"&status=", Status },
                {"&type=", Type },
                {"&litigation=", Litigation },
                {"&page=", PageNumber },
            };
        }

        private string ConfigureSearchUrl()
        {
            /// ====================================================================================
            /// configure search url based on given domain, query, and parameters
            /// 
            /// >>> param:  SearchBy    # search by option to configure search url
            ///             
            /// >>> funct:  1       # update search query using selected search by method
            /// >>> funct:  2       # update other parameters based on properties of webpage instance
            /// >>> funct:  3       # assign arguments to parameters in search url where not null
            /// ====================================================================================

            ////0
            switch (searchBy)
            {
                case SearchBy.Keyword:
                    Parameters["&q="] = Keyword;
                    break;                
                case SearchBy.ClassCode:
                    Parameters["&q="] = ClassCode;
                    break;                
                case SearchBy.PatentCode:
                    Parameters["&q="] = PatentCode;
                    break;
            }

            ////1
            Parameters["&before="] = Before;
            Parameters["&after="] = After;
            Parameters["&inventor="] = Inventor;
            Parameters["&assignee="] = Assignee;
            Parameters["&country="] = Country;
            Parameters["&language="] = Language;
            Parameters["&status="] = Status;
            Parameters["&type="] = Type;
            Parameters["&litigation="] = Litigation;
            Parameters["&page="] = PageNumber;

            ////2
            var paramters = "";
            foreach (var key in Parameters.Keys)
                if (Parameters[key] != null)
                    paramters += $"{key}{Parameters[key]}";

            return URL_SEARCH_PAGE.Replace("{parameters}", paramters).Replace("?&", "?");
        }

        private string ConfigurePatentUrl()
        {
            return URL_PATENT_PAGE.Replace("{patentCode}", PatentCode);
        } 
    }
}

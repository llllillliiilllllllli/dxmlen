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
    internal class SearchUrl
    {
        internal string? Domain { set; get; }
        internal string? QueryKey { set; get; }
        internal string? QueryValue { set; get; }
        internal Dictionary<string, string?>? Parameters { set; get; }
        internal string? Pagination { set; get; }

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

        public SearchUrl() 
        {
            this.Domain = "https://patents.google.com/";
            this.QueryKey = "?q=";
            this.QueryValue = null;
            this.Parameters = new Dictionary<string, string?>()
            {
                {"&before=", Before },
                {"&after=", After },
                {"&inventor=", Inventor },
                {"&assignee=", Assignee },
                {"&country=", Country },
                {"&language=", Language },
                {"&status=", Status },
                {"&type=", Type },
                {"&litigation=", Litigation },
            };

            this.Pagination = "&page={page}";
        }

        public string ConfigureSearchUrl(SearchBy searchBy)
        {
            /// ====================================================================================
            /// configure search url based on given domain, query, and parameters
            /// 
            /// >>> param:  SearchBy                    # search by option to configure search url
            /// >>> param:  Dictionary<string, string?> # parameter key-value pairs for searching
            ///             
            /// >>> funct:  0       # if search by keyword: https://patents.google.com/?q={keyword}
            /// >>> funct:  1       # if search by class code: https://patents.google.com/?q={classCode}
            /// >>> funct:  2       # if search by patent code: https://patents.google.com/?q={patentCode}
            /// >>> funct:  3       # assign arguments to parameters in search url if arguments not null
            /// >>> funct:  4       # return complete url with pagination pattern for iterating pages
            /// ====================================================================================
            ////0
            var url = "";

            ////1
            switch (searchBy)
            {
                case SearchBy.Keyword:
                    QueryValue = Keyword;
                    url += $"{Domain}{QueryKey}{QueryValue}";
                    break;                
                case SearchBy.ClassCode:
                    QueryValue = ClassCode;
                    url += $"{Domain}{QueryKey}{QueryValue}";
                    break;                
                case SearchBy.PatentCode:
                    QueryValue = PatentCode;
                    url += $"{Domain}{QueryKey}{PatentCode}";
                    break;
            }

            ////2
            Parameters = new Dictionary<string, string?>()
            {
                {"&before=", Before },
                {"&after=", After },
                {"&inventor=", Inventor },
                {"&assignee=", Assignee },
                {"&country=", Country },
                {"&language=", Language },
                {"&status=", Status },
                {"&type=", Type },
                {"&litigation=", Litigation },
            };

            ////3
            foreach (var key in Parameters.Keys)
                if (Parameters[key] != null)
                    url += $"{key}{Parameters[key]}";

            return url + Pagination;
        }        
    }
}

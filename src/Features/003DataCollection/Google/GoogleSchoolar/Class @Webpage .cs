using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DxMLEngine.Attributes;

namespace DxMLEngine.Features.GoogleScholar
{
    internal class Webpage
    {
        public string? Id { set; get; }

        #region Request

        private const string URL_SEARCH_PAGE = "https://scholar.google.com/scholar?{parameters}";
        
        public string SearchUrl { get { return ConfigureSearchUrl(); } }
                
        public string? Query { set; get; } 
        public string? Page { set; get; } 
        public string? Language { set; get; } 
        public string? FromYear { set; get; } 
        public string? ToYear { set; get; } 
        public bool? Reviewed { set; get; }

        #endregion Request

        #region Response

        public string? PageText { set; get; }
        public string? PageSource { set; get; }
        
        #endregion Response

        public Webpage()
        {

        }

        private string ConfigureSearchUrl()
        {
            string parameters = "";
            if (Query != null) parameters += $"&q={Query.Replace(" ", "+")}";
            if (Page != null) parameters += $"&start={Page}";
            if (Language != null) parameters += $"&hl={Language}";
            if (FromYear != null) parameters += $"&as_ylo={FromYear}";
            if (ToYear != null) parameters += $"&as_yhi={ToYear}";
            if (Reviewed == true) parameters += $"&as_rr=1";
            else parameters += $"&as_rr=0";

            return URL_SEARCH_PAGE.Replace("{parameters}", parameters).Replace("?&", "?");
        }
    }
}

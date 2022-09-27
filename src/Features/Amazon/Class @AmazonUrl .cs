using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.Amazon
{
    internal class AmazonUrl
    {
        public const string URL_SEARCH_PAGE = "https://www.amazon.com/s?k={keyword}";
        public const string URL_PRODUCT_PAGE = "https://www.amazon.com/dp/{asin}";

        public string? Keyword { set; get; }
        public string? ASIN { set; get; }
        public string? Page { set; get; }

        public AmazonUrl()
        {

        }

        public string ConfigureSearchUrl()
        {
            var url = "";
            if (Keyword != null) url += URL_SEARCH_PAGE.Replace("{keyword}", Keyword);
            if (Page != null) url += $"&page={Page}";

            return url;
        }

        public string ConfigureProductUrl(string page)
        {
            return URL_SEARCH_PAGE.Replace("{asin}", ASIN);
        }
    }
}

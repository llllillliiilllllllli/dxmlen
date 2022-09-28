using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.Amazon
{
    internal class Webpage
    {
        public const string URL_SEARCH_PAGE = "https://www.amazon.com/s?k={keyword}";
        public const string URL_PRODUCT_PAGE = "https://www.amazon.com/dp/{asin}";
        public const string URL_REVIEW_PAGE = "https://www.amazon.com/product-reviews/{asin}";

        public string? Keyword { set; get; }
        public string? Asin { set; get; }

        public Dictionary<object, object?> Parameters { set; get; }
        public string? FilterByStar { set; get; }
        public string? SearchPage { set; get; }
        public string? ReviewPage { set; get; }

        public Webpage()
        {
            Parameters = new Dictionary<object, object?>()
            {
                { "/filterByStar", null },
                { "&page=", null },
                { "?pageNumber=", null },
            };
        }

        public string ConfigureSearchUrl()
        {
            Parameters["&page="] = SearchPage;

            var keyword = Keyword != null ? Keyword.Replace(" ", "+").Replace("&", "%26") : null;
            var url = URL_SEARCH_PAGE.Replace("{keyword}", keyword);

            if (Parameters["&page="] != null) url += $"&page={Parameters["&page="]}";            

            return url;
        }

        public string ConfigureProductUrl()
        {
            return URL_PRODUCT_PAGE.Replace("{asin}", Asin);
        }

        public string ConfigureReviewUrl()
        {
            Parameters["/filterByStar"] = FilterByStar;
            Parameters["?pageNumber="] = ReviewPage;

            var url = URL_REVIEW_PAGE.Replace("{asin}", Asin);
            if (Parameters["/filterByStar"] != null) url += $"/filterByStar={Parameters["/filterByStar"]}";
            if (Parameters["?pageNumber="] != null) url += $"?pageNumber={Parameters["?pageNumber="]}";

            return url;
        }
    }
}

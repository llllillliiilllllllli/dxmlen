using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.Amazon
{
    internal class Webpage
    {
        public string Domain { get { return "https://patents.google.com/"; } }
        public string SearchUrl { get { return ConfigureSearchUrl(); } }
        public string DetailUrl { get { return ConfigureDetailUrl(); } }
        public string ReviewUrl { get { return ConfigureReviewUrl(); } }

        public string? PageText { set; get; }
        public string? PageSource { set; get; }

        private const string URL_SEARCH_PAGE = "https://www.amazon.com/s{parameters}";
        private const string URL_PRODUCT_PAGE = "https://www.amazon.com/dp/{asin}";
        private const string URL_REVIEW_PAGE = "https://www.amazon.com/product-reviews/{asin}/{parameters}";

        internal Dictionary<object, object?> SearchParameters { set; get; }
        internal Dictionary<object, object?> ReviewParameters { set; get; }
        
        internal string? Keyword { set; get; }
        internal string? Asin { set; get; }
        internal string? FilterByStar { set; get; }

        internal string? SearchPageNumber { set; get; }
        internal string? ReviewPageNumber { set; get; }

        internal PageLayout PageLayout { set; get; }

        public Webpage()
        {
            SearchParameters = new Dictionary<object, object?>()
            {
                { "?k=", Keyword },
                { "&page=", SearchPageNumber },
            };

            ReviewParameters = new Dictionary<object, object?>()
            {
                { "&filterByStar=", FilterByStar },
                { "&pageNumber=", ReviewPageNumber },
            };
        }

        public string ConfigureSearchUrl()
        {
            SearchParameters["?k="] = Keyword != null ? Keyword.Replace(" ", "+").Replace("&", "%26") : null;
            SearchParameters["&page="] = SearchPageNumber;

            var paramters = "";
            foreach (var key in SearchParameters.Keys)
                if (SearchParameters[key] != null)
                    paramters += $"{key}{SearchParameters[key]}";

            return URL_SEARCH_PAGE.Replace("{parameters}", paramters);
        }

        public string ConfigureDetailUrl()
        {
            return URL_PRODUCT_PAGE.Replace("{asin}", Asin);
        }

        public string ConfigureReviewUrl()
        {
            ReviewParameters["&filterByStar="] = FilterByStar;
            ReviewParameters["&pageNumber="] = ReviewPageNumber;

            var parameters = "";
            foreach (var key in ReviewParameters.Keys)
                if (ReviewParameters[key] != null)
                    parameters += $"{key}{ReviewParameters[key]}";

            return URL_REVIEW_PAGE.Replace("{asin}", Asin).Replace("{parameters}", parameters);
        }
    }
}

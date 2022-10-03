using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.Amazon
{
    internal class WebSearch
    {
        public string? Id { set; get; }

        #region REQUEST

        public string SearchUrl { get { return ConfigureSearchUrl(); } }
        public string DetailUrl { get { return ConfigureDetailUrl(); } }
        public string ReviewUrl { get { return ConfigureReviewUrl(); } }

        private const string URL_SEARCH_PAGE = "https://www.amazon.com/s{parameters}";
        private const string URL_PRODUCT_PAGE = "https://www.amazon.com/dp/{asin}";
        private const string URL_REVIEW_PAGE = "https://www.amazon.com/product-reviews/{asin}/{parameters}";
        
        internal string? Keyword { set; get; }
        internal string? Asin { set; get; }
        internal string? FilterByStar { set; get; }
        internal int? NumberOfPages { set; get; }

        internal string? SearchPageNumber { set; get; }
        internal string? ReviewPageNumber { set; get; }

        internal PageLayout PageLayout { set; get; }

        public string ConfigureSearchUrl()
        {
            var parameters = "";
            if (Keyword != null) parameters += $"?k={Keyword.Replace(" ", "+").Replace("&", "%26")}";
            if (SearchPageNumber != null) parameters += $"&page={SearchPageNumber}";

            return URL_SEARCH_PAGE.Replace("{parameters}", parameters);
        }

        public string ConfigureDetailUrl()
        {
            return URL_PRODUCT_PAGE.Replace("{asin}", Asin);
        }

        public string ConfigureReviewUrl()
        {
            var parameters = "";
            if (FilterByStar != null) parameters += $"&filterByStar={FilterByStar}";
            if (ReviewPageNumber != null) parameters += $"&pageNumber={ReviewPageNumber}";

            return URL_REVIEW_PAGE.Replace("{asin}", Asin).Replace("{parameters}", parameters);
        }

        #endregion REQUEST

        #region RESPONSE

        public string? PageText { set; get; }
        public string? PageSource { set; get; }

        #endregion RESPONSE
    }
}

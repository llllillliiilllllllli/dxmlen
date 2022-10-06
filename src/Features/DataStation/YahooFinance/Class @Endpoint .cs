using DxMLEngine.Features.GooglePatents;
using DxMLEngine.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DxMLEngine.Features.YahooFinance
{
    internal class Endpoint
    {
        public string? Id { set; get; }

        #region REQUEST 

        public string BaseEndpoint { get { return "https://query1.finance.yahoo.com"; } }
        public string SearchEndpoint { get { return ConfigureSearchEndpoint(); } }
        public string QuotesEndpoint { get { return ConfigureQuotesEndpoint(); } }
        public string OptionsEndpoint { get { return ConfigureOptionsEndpoint(); } }
        public string DownloadEndpoint { get { return ConfigureDownloadEndpoint(); } }
        public string HistoryEndpoint { get { return ConfigureHistoryEndpoint(); } }
        public string SummaryEndpoint { get { return ConfigureSummaryEndpoint(); } }        

        private const string URL_BASE = "https://finance.yahoo.com";
        private const string URL_QUOTE = "https://finance.yahoo.com/quote/{symbol}";
        private const string URL_SUMMARY = "https://finance.yahoo.com/quote/{symbol}?p={symbol}";
        private const string URL_CHART = "https://finance.yahoo.com/quote/{symbol}/chart?p={symbol}";
        private const string URL_CONVERSATION = "https://finance.yahoo.com/quote/{symbol}/community?p={symbol}";
        private const string URL_STATISTICS = "https://finance.yahoo.com/quote/{symbol}/key-statistics?p={symbol}";
        private const string URL_HISTORY = "https://finance.yahoo.com/quote/{symbol}/history?p={symbol}";
        private const string URL_PROFILE = "https://finance.yahoo.com/quote/{symbol}/profile?p={symbol}";
        private const string URL_FINANCIALS = "https://finance.yahoo.com/quote/{symbol}/financials?p={symbol}";
        private const string URL_ANALYSIS = "https://finance.yahoo.com/quote/{symbol}/analysis?p={symbol}";
        private const string URL_OPTIONS = "https://finance.yahoo.com/quote/{symbol}/options?p={symbol}" ;
        private const string URL_HOLDERS = "https://finance.yahoo.com/quote/{symbol}/holders?p={symbol}";
        private const string URL_SUSTAINABILITY = "https://finance.yahoo.com/quote/{symbol}/sustainability?p={symbol}";

        private const string EP_SEARCH = "https://query1.finance.yahoo.com/v1/finance/search?{parameters}";
        private const string EP_QUOTES = "https://query1.finance.yahoo.com/v6/finance/quote?{parameters}";
        private const string EP_QUOTES_ALT = "https://query1.finance.yahoo.com/v7/finance/quote?{parameters}";
        private const string EP_OPTIONS = "https://query1.finance.yahoo.com/v7/finance/options/{parameters}";
        private const string EP_DOWNLOAD = "https://query1.finance.yahoo.com/v7/finance/download/{parameters}";
        private const string EP_HISTORY = "https://query1.finance.yahoo.com/v8/finance/chart/{parameters}";
        private const string EP_SUMMARY = "https://query1.finance.yahoo.com/v10/finance/quoteSummary/{parameters}";

        public string? Query { set; get; }
        public string? Symbol { set; get; }
        public string[]? Symbols { set; get; }
        public string[]? Modules { set; get; }

        public string? Interval { set; get; }
        public string? Range { set; get; }
        public string? Period1 { set; get; }
        public string? Period2 { set; get; }
        public bool? Close { set; get; }
        public bool? Events { set; get; }
        public bool? Prepost { set; get; }

        public string ConfigureSearchEndpoint()
        {
            var parameters = "";
            if (Query != null) parameters += $"&q={Query.Replace(" ", "%20")}";
            return EP_SEARCH.Replace("{parameters}", parameters).Replace("?&", "?");
        }        
        
        public string ConfigureQuotesEndpoint()
        {
            var parameters = "";
            if (Symbols != null) parameters += $"&symbols={string.Join(",", Symbols)}";
            return EP_QUOTES.Replace("{parameters}", parameters).Replace("?&", "?");
        }

        public string ConfigureOptionsEndpoint()
        {
            var parameters = "";
            if (Symbol != null) parameters += $"{Symbol}";
            return EP_OPTIONS.Replace("{parameters}", parameters).Replace("?&", "?");
        }

        public string ConfigureDownloadEndpoint()
        {
            throw new NotImplementedException();
        }

        public string ConfigureHistoryEndpoint()
        {
            var parameters = "";
            if (Symbol != null) parameters += $"{Symbol}?";

            if (Interval != null) parameters += $"&interval={Interval}";
            if (Range != null) parameters += $"&range={Range}";
            else
            {
                if (Period1 != null) parameters += $"&period1={Period1}";
                if (Period2 != null) parameters += $"&period2={Period2}";
            }

            if (Close == true) parameters += $"&close=adjusted";
            else parameters += $"&close=unadjusted";
            if (Events == true) parameters += $"&events=div%7Csplit";
            if (Prepost == true) parameters += $"&includePrePost=true";
            else parameters += $"&includePrePost=false";

            return EP_HISTORY.Replace("{parameters}", parameters).Replace("?&", "?");
        }

        public string ConfigureSummaryEndpoint()
        {
            var parameters = "";
            if (Symbol != null) parameters += $"{Symbol}?";
            if (Modules != null) parameters += $"&modules={string.Join(",", Modules)}";
            return EP_SUMMARY.Replace("{parameters}", parameters).Replace("?&", "?");
        }

        #endregion REQUEST

        #region RESPONSE

        public string? Response { set; get; }

        #endregion RESPONSE
    }
}

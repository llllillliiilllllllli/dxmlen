using DxMLEngine.Features.GooglePatents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.YahooFinance
{
    public class Search
    {
        public object[]? explains { get; set; }
        public int count { get; set; }
        public Quote[]? quotes { get; set; }
        public News[]? news { get; set; }
        public object[]? nav { get; set; }
        public object[]? lists { get; set; }
        public object[]? researchReports { get; set; }
        public object[]? screenerFieldResults { get; set; }
        public int totalTime { get; set; }
        public int timeTakenForQuotes { get; set; }
        public int timeTakenForNews { get; set; }
        public int timeTakenForAlgowatchlist { get; set; }
        public int timeTakenForPredefinedScreener { get; set; }
        public int timeTakenForCrunchbase { get; set; }
        public int timeTakenForNav { get; set; }
        public int timeTakenForResearchReports { get; set; }
        public int timeTakenForScreenerField { get; set; }
        public int timeTakenForCulturalAssets { get; set; }

        public class Quote
        {
            public string? Exchange { get; set; }
            public string? shortname { get; set; }
            public string? quoteType { get; set; }
            public string? symbol { get; set; }
            public string? index { get; set; }
            public float score { get; set; }
            public string? typeDisp { get; set; }
            public string? longname { get; set; }
            public string? exchDisp { get; set; }
            public string? sector { get; set; }
            public string? industry { get; set; }
            public bool dispSecIndFlag { get; set; }
            public bool isYahooFinance { get; set; }
        }

        public class News
        {
            public string? uuid { get; set; }
            public string? title { get; set; }
            public string? publisher { get; set; }
            public string? link { get; set; }
            public int providerPublishTime { get; set; }
            public string? type { get; set; }
            public Thumbnail? thumbnail { get; set; }
            public string[]? relatedTickers { get; set; }
        }

        public class Thumbnail
        {
            public Resolution[]? resolutions { get; set; }
        }

        public class Resolution
        {
            public string? url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public string? tag { get; set; }
        }
    }
}

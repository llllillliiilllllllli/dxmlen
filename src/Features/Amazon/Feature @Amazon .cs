using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Data.Analysis;

using DxMLEngine.Attributes;
using DxMLEngine.Functions;

using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using DxMLEngine.Features.GooglePatents;

namespace DxMLEngine.Features.Amazon
{
    [Feature]
    internal class Amazon
    {
        public static void CollectProductAsin()
        {
            /// ====================================================================================
            /// Search patent codes from Google Patents with given query and parameters
            /// 
            /// >>> param:  string  # path to input file containing list of search queries 
            /// >>> param:  string  # path to output folder for storing collected data 
            /// >>> param:  string  # name of file to be saved as output after searching 
            ///             
            /// >>> funct:  0       # ...
            /// >>> funct:  1       # ...
            /// >>> funct:  2       # ...
            /// ====================================================================================
            
            ////0
            Console.Write("\nEnter input file path: ");
            var i_fil = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(i_fil))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var o_fol = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(o_fol))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output file name: ");
            var o_fil = Console.ReadLine()?.Replace(" ", "");

            if (string.IsNullOrEmpty(o_fil))
                throw new ArgumentNullException("file name is null or empty");

            Console.Write("\nEnter maximum page: ");
            var inputPages = Console.ReadLine()?.Replace(",", "");

            ////1            
            var amazonUrls = InputKeywords(i_fil);

            ////2
            var browser = BrowserAutomation.LaunghEdge();
            if (browser == null)
                throw new Exception("browser == null");

            ////3
            var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
                    new StringDataFrameColumn("Keyword"),
                    new StringDataFrameColumn("ASIN"),
                }
            );

            foreach (var amazonUrl in amazonUrls)
            {
                var url = amazonUrl.ConfigureSearchUrl();
                Console.WriteLine($"Collect: {url}");

                var tempTab = BrowserAutomation.OpenNewTab(browser, url);
                var pageText = BrowserAutomation.CopyPageText(tempTab);
                var pageSource = BrowserAutomation.CopyPageSource(tempTab);

                var pageLayout = CheckPageLayout(pageText, pageSource);

                int numPages;
                if (!string.IsNullOrEmpty(inputPages)) numPages = int.Parse(inputPages);
                else
                    numPages = FindNumberOfPageFormA(pageLayout, pageText, pageSource);

                BrowserAutomation.CloseCurrentTab(tempTab);

                for (int i = 0; i < numPages; i++)
                {
                    amazonUrl.Page = $"{i+1}";
                    url = amazonUrl.ConfigureSearchUrl();

                    var newTab = BrowserAutomation.OpenNewTab(browser, url);
                    pageText = BrowserAutomation.CopyPageText(newTab);
                    pageSource = BrowserAutomation.CopyPageSource(newTab);
                                        
                    if (pageLayout == PageLayout.FormA)
                    {
                        var foundedAsins = ExtractAsinFormA(pageText, pageSource);
                        
                        foreach (var foundedAsin in foundedAsins)
                        {
                            var dataRow = new List<KeyValuePair<string, object?>>()
                            {
                                new KeyValuePair<string, object?>("Keyword", amazonUrl.Keyword),
                                new KeyValuePair<string, object?>("ASIN", foundedAsin),
                            };

                            dataFrame.Append(dataRow, inPlace: true);
                        }
                    }

                    if (pageLayout == PageLayout.FormB)
                    {
                        var dataRows = ExtractAsinFormB(pageText, pageSource);
                        dataFrame.Append(dataRows, inPlace: true);
                    }

                    Console.WriteLine(dataFrame.Head(10));

                    BrowserAutomation.CloseCurrentTab(newTab);
                }
            }

            BrowserAutomation.CloseBrowser(browser!);
        }

        public static void CollectProductData()
        {

        }

        private static AmazonUrl[] InputKeywords(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, encoding: Encoding.UTF8);
            var amazonUrls = new List<AmazonUrl>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var amazonUrl = new AmazonUrl();

                amazonUrl.Keyword = dataFrame["Keyword"][i] != null ? dataFrame["Keyword"][i].ToString() : null;

                amazonUrls.Add(amazonUrl);
            }

            return amazonUrls.ToArray();
        }

        private static AmazonUrl[] InputAsins(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, encoding: Encoding.UTF8);
            var amazonUrls = new List<AmazonUrl>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var amazonUrl = new AmazonUrl();

                amazonUrl.Keyword = dataFrame["Asin"][i] != null ? dataFrame["Asin"][i].ToString() : null;

                amazonUrls.Add(amazonUrl);
            }

            return amazonUrls.ToArray();
        }

        private static int FindNumberOfPageFormA(PageLayout pageLayout, string pageText, string pageSource)
        {
            var targetText = pageText
                .Split("RESULTS")[0];

            var regex = new Regex("");
            var perPage = "";
            switch (pageLayout)
            {
                case PageLayout.FormA:
                    regex = new Regex(@"1-48 of over [\d,]+ results for ");
                    perPage = "48";
                    break;

                case PageLayout.FormB:
                    regex = new Regex(@"1-16 of over [\d,]+ results for ");
                    perPage = "16";
                    break;
            }

            var matches = regex.Matches(targetText);

            var numResults =
                from match in matches
                where match.Success
                where string.IsNullOrEmpty(match.Value) == false
                select int.Parse(match.Value
                    .Replace($"1-{perPage} of over ", "")
                    .Replace(" results for ", "")
                    .Replace(",", ""));

            return int.Parse((numResults.First() / int.Parse(perPage)).ToString().Split(".")[0]);
        }
    
        private static string[] ExtractAsinFormA(string pageText, string pageSource)
        {
            /// ====================================================================================
            /// https://www.amazon.com/s?k=chair
            /// ====================================================================================

            ////0
            var targetText = pageText
                .Split("RESULTS")[0];

            var regex = new Regex(@"1-48 of over [\d,]+ results for \u0022[\w]+\u0022");
            var match = regex.Match(targetText);

            var keyword = Regex
                .Replace(match.Value, @"1-48 of over [\\d,]+ results for ", "")
                .Replace("\u0022", "");

            ////1
            var targetSource = pageSource
                .Split("<div class=\"s-main-slot s-result-list s-search-results sg-row\">")[1]
                .Split("<div class=\"s-result-list-placeholder aok-hidden sg-row\">")[0];

            var asinHtml = new HtmlDocument();
            asinHtml.LoadHtml(targetSource);

            var asinXPath = "/div";
            var asinNodes = asinHtml.DocumentNode.SelectNodes(asinXPath);
            var foundedAsins = (
                from asinNode in asinNodes
                let asin = asinNode.GetAttributeValue("data-asin", null)
                where string.IsNullOrEmpty(asin) == false
                select asin).ToArray();

            return foundedAsins.ToArray();
        }

        private static string[] ExtractAsinFormB(string pageText, string pageSource)
        {
            /// ====================================================================================
            /// https://www.amazon.com/s?k=electronics
            /// ====================================================================================

            ////0
            var targetText = pageText
                .Split("RESULTS")[0];

            var regex = new Regex(@"1-16 of over [\d,]+ results for \u0022[\w]+\u0022");
            var match = regex.Match(targetText);

            var keyword = Regex
                .Replace(match.Value, @"1-16 of over [\\d,]+ results for ", "")
                .Replace("\u0022", "");

            var targetSource = pageSource
                .Split("<div class=\"s-main-slot s-result-list s-search-results sg-row\">")[1]
                .Split("<div class=\"s-result-list-placeholder aok-hidden sg-row\">")[0];

            ////1
            var asinHtml = new HtmlDocument();
            asinHtml.LoadHtml(targetSource);

            var asinXPath = "/div";
            var asinNodes = asinHtml.DocumentNode.SelectNodes(asinXPath);
            var foundedAsins = (
                from asinNode in asinNodes
                let asin = asinNode.GetAttributeValue("data-asin", null)
                where string.IsNullOrEmpty(asin) == false
                select asin).ToArray();

            return foundedAsins.ToArray();
        }

        private static PageLayout CheckPageLayout(string pageText, string pageSource)
        {
            ////0
            var targetText = pageText
                .Split("RESULTS")[0];

            var regexA = new Regex(@"1-48 of over [\d,]+ results for \u0022[\w]+\u0022");
            var matchA = regexA.Match(targetText);
            if (matchA.Success) return PageLayout.FormA;

            var regexB = new Regex(@"1-16 of over [\d,]+ results for \u0022[\w]+\u0022");
            var matchB = regexB.Match(targetText);
            if (matchB.Success) return PageLayout.FormB;

            ////1
            var html = new HtmlDocument();
            html.LoadHtml(pageSource);

            var checkXPath = @"/html/body/div[1]/div[2]/span/div/h1/div/div[1]/div/div/span[1]";

            var formANode = html.DocumentNode.SelectSingleNode(checkXPath);
            if (formANode != null) return PageLayout.FormA;

            var formBNode = html.DocumentNode.SelectSingleNode(checkXPath);
            if (formBNode != null) return PageLayout.FormB;

            return PageLayout.Unknown;
        }
    }
}

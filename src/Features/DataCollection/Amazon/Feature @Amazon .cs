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

namespace DxMLEngine.Features.Amazon
{
    [Feature]
    internal class Amazon
    {
        public static void CollectProductAsins()
        {
            /// ====================================================================================
            /// Collect ASIN of products on Amazon using given keywords and pagination
            /// 
            /// >>> param:  string  # path to input file containing list of search queries 
            /// >>> param:  string  # path to output folder for storing collected data 
            /// >>> param:  string  # name of file to be saved as output after searching 
            ///             
            /// >>> funct:  0       # inquire user input for paths to files and folders
            /// >>> funct:  1       # instantiate list of Amazon urls from input file
            /// >>> funct:  2       # prepare data frame to store results from website
            /// >>> funct:  3       # launch new browser instance for web automation
            /// >>> funct:  4       # loop through urls and determine correct page layout
            /// >>> funct:  5       # find the number of pages or leave as default input
            /// >>> funct:  6       # loop through pages and extract data in text and html
            /// >>> funct:  7       # export collected data frame to local file and folder
            /// ====================================================================================

            ////0
            Console.Write("\nEnter input file path: ");
            var inFile = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(inFile))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var outDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(outDir))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter number of pages: ");
            var inputPages = Console.ReadLine()?.Replace(",", "");

            ////1            
            var webpages = InputSearchKeywords(inFile);

            ////2
            var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
                    new StringDataFrameColumn("Keyword"),
                    new StringDataFrameColumn("ASIN"),
                    new StringDataFrameColumn("URL"),
                }
            );

            ////3
            var browser = BrowserAutomation.LaunghEdge();
            if (browser == null)
                throw new Exception("browser == null");

            ////4
            foreach (var webpage in webpages)
            {          
                var tempTab = BrowserAutomation.OpenNewTab(browser, webpage.SearchUrl);
                webpage.PageText = BrowserAutomation.CopyPageText(tempTab, 1000);
                webpage.PageSource = BrowserAutomation.CopyPageSource(tempTab, 5000);

                webpage.PageLayout = CheckPageLayout(webpage);
                
                BrowserAutomation.CloseCurrentTab(tempTab);

            ////5
                int numPages;
                if (!string.IsNullOrEmpty(inputPages)) numPages = int.Parse(inputPages);
                else
                    numPages = FindNumberOfProductPages(webpage);

            ////6
                for (int i = 0; i < numPages; i++)
                {
                    webpage.SearchPageNumber = $"{i+1}";
                    Console.WriteLine($"\nCollect: {webpage.SearchUrl}");

                    var newTab = BrowserAutomation.OpenNewTab(browser, webpage.SearchUrl);
                    webpage.PageText = BrowserAutomation.CopyPageText(newTab, 1000);
                    webpage.PageSource = BrowserAutomation.CopyPageSource(newTab, 5000);

                    OutputSearchPageText(outDir, webpage);
                    OutputSearchPageSource(outDir, webpage);

                    var foundedAsins = new List<string>();
                    if (webpage.PageLayout == PageLayout.FormA)
                        foundedAsins.AddRange(ExtractAsinFormA(webpage));
                    if (webpage.PageLayout == PageLayout.FormB)
                        foundedAsins.AddRange(ExtractAsinFormB(webpage));
                        
                    foreach (var foundedAsin in foundedAsins)
                    {
                        var dataRow = new List<KeyValuePair<string, object?>>()
                        {
                            new KeyValuePair<string, object?>("Keyword", webpage.Keyword),
                            new KeyValuePair<string, object?>("ASIN", foundedAsin),
                            new KeyValuePair<string, object?>("URL", webpage.SearchUrl),
                        };

                        Console.WriteLine(foundedAsin);
                        dataFrame.Append(dataRow, inPlace: true);
                    }

                    BrowserAutomation.CloseCurrentTab(newTab);
                }
            }

            BrowserAutomation.CloseBrowser(browser!);

            ////7
            OutputProductSearches(outDir, dataFrame);
        }

        public static void CollectProductDetails()
        {
            /// ====================================================================================
            /// Collect product information on Amazon using given list of ASINs
            /// 
            /// >>> param:  string  # path to input file containing list of search queries 
            /// >>> param:  string  # path to output folder for storing collected data 
            /// >>> param:  string  # name of file to be saved as output after searching 
            ///             
            /// >>> funct:  0       # inquire user input for paths to files and folders
            /// >>> funct:  1       # instantiate list of Amazon urls from input file
            /// >>> funct:  2       # launch new browser instance for web automation
            /// >>> funct:  3       # prepare data frame to store results from website
            /// >>> funct:  4       # loop through urls nd extract data in text and html
            /// >>> funct:  5       # export collected data frame to local file and folder
            /// 
            /// >>> examp: https://www.amazon.com/dp/B081H44MHD
            /// >>> examp: https://www.amazon.com/dp/B099VMT8VZ
            /// ====================================================================================

            ////0
            Console.Write("\nEnter input file path: ");
            var inFile = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(inFile))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var outDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(outDir))
                throw new ArgumentNullException("path is null or empty");

            ////1            
            var webpages = InputProductAsins(inFile);

            ////2
            var browser = BrowserAutomation.LaunghEdge();
            if (browser == null)
                throw new Exception("browser == null");

            ////3
            var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
                    new StringDataFrameColumn("Keyword"),
                    new StringDataFrameColumn("ASIN"),
                    new StringDataFrameColumn("Product Name"),
                    new StringDataFrameColumn("Seller Name"),
                    new StringDataFrameColumn("Selling Price"),
                    new StringDataFrameColumn("Category"),
                    new StringDataFrameColumn("Description"),
                    new StringDataFrameColumn("URL"),
                }
            );

            ////4
            foreach (var webpage in webpages)
            {
                Console.WriteLine($"\nCollect: {webpage.DetailUrl}");

                var newTab = BrowserAutomation.OpenNewTab(browser, webpage.DetailUrl);
                webpage.PageText = BrowserAutomation.CopyPageText(newTab, 2000);
                webpage.PageSource = BrowserAutomation.CopyPageSource(newTab, 5000);

                OutputDetailPageText(outDir, webpage);
                OutputDetailPageSource(outDir, webpage);

                var dataRows = new List<KeyValuePair<string, object?>>();
                dataRows.AddRange(new List<KeyValuePair<string, object?>>()
                {
                    new KeyValuePair<string, object?>("Keyword", webpage.Keyword),
                    new KeyValuePair<string, object?>("ASIN", webpage.Asin),
                    new KeyValuePair<string, object?>("URL", webpage.DetailUrl),
                });
                dataRows.AddRange(ExtractProductData(webpage));

                dataFrame.Append(dataRows, inPlace: true);
                BrowserAutomation.CloseCurrentTab(newTab);
            }

            BrowserAutomation.CloseBrowser(browser);

            ////5
            OutputProductDetails(outDir, dataFrame);
        }

        public static void CollectCustomerReviews()
        {
            ////0
            Console.Write("\nEnter input file path: ");
            var inFile = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(inFile))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var outDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(outDir))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nSelect options [Positive/Negative]: ");
            var option = Console.ReadLine();

            Console.Write("\nEnter number of pages: ");
            var inputPages = Console.ReadLine()?.Replace(",", "");

            ////1            
            var webpages = InputProductAsins(inFile);

            ////2
            var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
                    new StringDataFrameColumn("Keyword"),
                    new StringDataFrameColumn("ASIN"),
                    new StringDataFrameColumn("Product Name"),
                    new StringDataFrameColumn("Seller Name"),
                    new StringDataFrameColumn("Overall Score"),
                    new StringDataFrameColumn("Rating Score"),
                    new StringDataFrameColumn("Number of Ratings"),
                    new StringDataFrameColumn("5 Star"),
                    new StringDataFrameColumn("4 Star"),
                    new StringDataFrameColumn("3 Star"),
                    new StringDataFrameColumn("2 Star"),
                    new StringDataFrameColumn("1 Star"),
                    new StringDataFrameColumn("Review Id"),
                    new StringDataFrameColumn("Date & Location"),
                    new StringDataFrameColumn("Reviewer Name"),
                    new StringDataFrameColumn("Review Rating"),
                    new StringDataFrameColumn("Review Title"),
                    new StringDataFrameColumn("Review Content"),
                    new StringDataFrameColumn("Verified Purchase"),
                    new StringDataFrameColumn("URL"),
                }
            );

            ////3
            var browser = BrowserAutomation.LaunghEdge();
            if (browser == null)
                throw new Exception("browser == null");

            ////4
            foreach (var webpage in webpages)
            {
                switch (option)
                {
                    case "Positive":
                        webpage.FilterByStar = "positive";
                        break;

                    case "Negative":
                        webpage.FilterByStar = "critical";
                        break;

                    default:
                        webpage.FilterByStar = null;
                        break;
                }

                var tempTab = BrowserAutomation.OpenNewTab(browser, webpage.ReviewUrl);
                webpage.PageText = BrowserAutomation.CopyPageText(tempTab, 1000);
                webpage.PageSource = BrowserAutomation.CopyPageSource(tempTab, 5000);

                OutputReviewPageText(outDir, webpage);
                OutputReviewPageSource(outDir, webpage);

                BrowserAutomation.CloseCurrentTab(tempTab);

            ////5
                int numPages;
                if (!string.IsNullOrEmpty(inputPages)) numPages = int.Parse(inputPages);
                else
                    numPages = FindNumberOfReviewPages(webpage);

            ////6
                for (int i = 0; i < numPages; i++)
                {
                    webpage.ReviewPageNumber = $"{i+1}";

                    Console.WriteLine($"\nCollect: {webpage.ReviewUrl}");

                    var newTab = BrowserAutomation.OpenNewTab(browser, webpage.ReviewUrl);
                    webpage.PageText = BrowserAutomation.CopyPageText(newTab, 2000);
                    webpage.PageSource = BrowserAutomation.CopyPageSource(newTab, 5000);

                    var dataRows = new List<KeyValuePair<string, object?>>();
                    dataRows.AddRange(new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("Keyword", webpage.Keyword),
                        new KeyValuePair<string, object?>("ASIN", webpage.Asin),
                        new KeyValuePair<string, object?>("URL", webpage.ReviewUrl),
                    });
                    dataRows.AddRange(ExtractReviewInfo(webpage));

            ////7
                    var reviews = ExtractReviewDetails(webpage);
                    foreach (var review in reviews)
                    {
                        dataRows.AddRange(review);
                        dataFrame.Append(dataRows, inPlace: true);

                        Console.Write($"{review[0].Value?.ToString()?[1..^1]} | ");
                        Console.WriteLine($"{review[2].Value?.ToString()?[1..^1]}");
                        Console.WriteLine($"{review[5].Value?.ToString()?[1..^1]}");
                        Console.WriteLine();
                    }
                    
                    BrowserAutomation.CloseCurrentTab(newTab);
                }
            }

            BrowserAutomation.CloseBrowser(browser);

            ////8
            OutputProductReviews(outDir, dataFrame);
        }

        #region INPUT OUTPUT

        private static Webpage[] InputSearchKeywords(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);
            var webpages = new List<Webpage>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var webpage = new Webpage();
                webpage.Keyword = dataFrame["Keyword"][i] != null ? dataFrame["Keyword"][i].ToString() : null;
                webpages.Add(webpage);
            }

            return webpages.ToArray();
        }

        private static Webpage[] InputProductAsins(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);
            var webpages = new List<Webpage>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var webpage = new Webpage();
                webpage.Keyword = dataFrame["Keyword"][i] != null ? dataFrame["Keyword"][i].ToString() : null;
                webpage.Asin = dataFrame["ASIN"][i] != null ? dataFrame["ASIN"][i].ToString() : null;
                webpages.Add(webpage);
            }

            return webpages.ToArray();
        }

        private static void OutputSearchPageText(string location, Webpage webpage)
        {
            var path = $"{location}\\Datadoc @SearchPage #-------------- .txt";
            File.WriteAllText(path, webpage.PageText, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputSearchPageSource(string location, Webpage webpage)
        {
            var path = $"{location}\\Webpage @Search #-------------- .html";
            File.WriteAllText(path, webpage.PageSource, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputDetailPageText(string location, Webpage webpage)
        {
            var path = $"{location}\\Datadoc @DetailPage #-------------- .txt";
            File.WriteAllText(path, webpage.PageText, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputDetailPageSource(string location, Webpage webpage)
        {
            var path = $"{location}\\Webpage @Detail #-------------- .html";
            File.WriteAllText(path, webpage.PageSource, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputReviewPageText(string location, Webpage webpage)
        {
            var path = $"{location}\\Datadoc @ReviewPage #-------------- .txt";
            File.WriteAllText(path, webpage.PageText, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputReviewPageSource(string location, Webpage webpage)
        {
            var path = $"{location}\\Webpage @Review #-------------- .html";
            File.WriteAllText(path, webpage.PageSource, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputProductSearches(string location, DataFrame dataFrame)
        {
            var path = $"{location}\\Dataset @ProductSearches #-------------- .csv";
            DataFrame.WriteCsv(dataFrame, path, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputProductDetails(string location, DataFrame dataFrame)
        {
            var path = $"{location}\\Dataset @ProductDetails #-------------- .csv";
            DataFrame.WriteCsv(dataFrame, path, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputProductReviews(string location, DataFrame dataFrame)
        {
            var path = $"{location}\\Dataset @ProductReviews #-------------- .csv";
            DataFrame.WriteCsv(dataFrame, path, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        #endregion INPUT OUTPUT

        #region PAGE CHECKING

        private static int FindNumberOfProductPages(Webpage webpage)
        {
            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            var targetText = webpage.PageText
                .Split("RESULTS")[0];

            var regex = new Regex("");
            var perPage = "";
            switch (webpage.PageLayout)
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
    
        private static int FindNumberOfReviewPages(Webpage webpage)
        {
            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            var pageHtml = new HtmlDocument();
            pageHtml.LoadHtml(webpage.PageSource);

            var numResultXPath = "//*[@id='filter-info-section']/div";
            var numResultNode = pageHtml.DocumentNode.SelectSingleNode(numResultXPath);
            var numResults = int.Parse(Regex
                .Replace(numResultNode.InnerText, @"[\d,]+ total ratings, ", "")
                .Replace(" with reviews", "")
                .Replace(",", ""));

            return int.Parse((numResults / 10).ToString().Split(".")[0]);
        }
        
        private static PageLayout CheckPageLayout(Webpage webpage)
        {
            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            ////0
            var targetText = webpage.PageText
                .Split("RESULTS")[0];

            var regexA = new Regex(@"1-48 of over [\d,]+ results for \u0022[\w]+\u0022");
            var matchA = regexA.Match(targetText);
            if (matchA.Success) return PageLayout.FormA;

            var regexB = new Regex(@"1-16 of over [\d,]+ results for \u0022[\w]+\u0022");
            var matchB = regexB.Match(targetText);
            if (matchB.Success) return PageLayout.FormB;

            ////1
            var html = new HtmlDocument();
            html.LoadHtml(webpage.PageSource);

            var checkXPath = @"/html/body/div[1]/div[2]/span/div/h1/div/div[1]/div/div/span[1]";

            var formANode = html.DocumentNode.SelectSingleNode(checkXPath);
            if (formANode != null) return PageLayout.FormA;

            var formBNode = html.DocumentNode.SelectSingleNode(checkXPath);
            if (formBNode != null) return PageLayout.FormB;

            return PageLayout.Unknown;
        }

        #endregion PAGE CHECKING

        #region DATA EXTRACTION

        private static string[] ExtractAsinFormA(Webpage webpage)
        {
            /// ====================================================================================
            /// https://www.amazon.com/s?k=chair
            /// ====================================================================================

            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            ////0
            var targetText = webpage.PageText
                .Split("RESULTS")[0];

            var regex = new Regex(@"1-48 of over [\d,]+ results for \u0022[\w]+\u0022");
            var match = regex.Match(targetText);

            var keyword = Regex
                .Replace(match.Value, @"1-48 of over [\d,]+ results for ", "")
                .Replace("\u0022", "");

            ////1
            var targetSource = webpage.PageSource
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

        private static string[] ExtractAsinFormB(Webpage webpage)
        {
            /// ====================================================================================
            /// https://www.amazon.com/s?k=electronics
            /// ====================================================================================

            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            ////0
            var targetText = webpage.PageText
                .Split("RESULTS")[0];

            var regex = new Regex(@"1-16 of over [\d,]+ results for \u0022[\w]+\u0022");
            var match = regex.Match(targetText);

            var keyword = Regex
                .Replace(match.Value, @"1-16 of over [\d,]+ results for ", "")
                .Replace("\u0022", "");

            var targetSource = webpage.PageSource
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

        private static KeyValuePair<string, object?>[] ExtractProductData(Webpage webpage)
        {
            /// ====================================================================================
            /// https://www.amazon.com/dp/B081H44MHD
            /// https://www.amazon.com/dp/B099VMT8VZ
            /// ====================================================================================

            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            ////0
            var targetText = webpage.PageText
                .Split("Sell on Amazon")[1];

            var splittedLines = (
                from line in targetText.Split("\n", StringSplitOptions.TrimEntries)
                where string.IsNullOrEmpty(line) == false
                select line).ToArray();

            var archorRegex = new Regex(@"[\d.]+ out of 5 stars [\d, ]* ratings");
            var priceRegex = new Regex(@"\$(0|[1-9][0-9]{0,2})(,\d{3})*(\.\d{1,2})?");

            string? productName = null;
            string? sellerName = null;
            string? sellingPrice = null;
            for (int i = 0; i < splittedLines.Length; i++)
            {
                var archorMatch = archorRegex.Match(splittedLines[i]);
                if (archorMatch.Success)
                {
                    for (int j = i; j > 0; j--)
                    {
                        if (splittedLines[j].Contains("Visit"))
                        {
                            sellerName = splittedLines[j].Replace("Visit the ", "");
                            productName = splittedLines[j-1];                            
                            break;
                        }
                    }

                    for (int j = i; j < i+5; j++)
                    {
                        var priceMatch = priceRegex.Match(splittedLines[j]);
                        if (priceMatch.Success)
                            sellingPrice = priceMatch.Value;
                    }

                    break;
                }
            }

            //// 
            string[]? categories = null;
            try
            {
                var categorySource = webpage.PageSource
                    .Split("<ul class=\"a-unordered-list a-horizontal a-size-small\">")[1]
                    .Split("</ul>")[0];

                var categoryHtml = new HtmlDocument();
                categoryHtml.LoadHtml(categorySource);

                var categoryXPath = "//*[@class='a-link-normal a-color-tertiary']";
                var categoryNodes = categoryHtml.DocumentNode.SelectNodes(categoryXPath);
                categories = (
                    from node in categoryNodes
                    let category = node.InnerText.Trim()
                    where string.IsNullOrEmpty(category) == false
                    select category).ToArray();
            }
            catch
            {
                categories = null;  
            }

            ////          
            var descriptionHtml = new HtmlDocument();
            descriptionHtml.LoadHtml(webpage.PageSource);

            string[]? description = null;

            try
            {
                var desriptionXPath = "//*[@id='productDescription']";
                var decriptionNode = descriptionHtml.DocumentNode.SelectSingleNode(desriptionXPath);

                if (decriptionNode != null)
                    description = decriptionNode.InnerText
                        .Split("\n", StringSplitOptions.TrimEntries);
            }
            catch { description = null; }

            var dataRow = new List<KeyValuePair<string, object?>>()
            {
                new KeyValuePair<string, object?>("Product Name", $"\"{productName}\""),
                new KeyValuePair<string, object?>("Seller Name", $"\"{sellerName}\""),
                new KeyValuePair<string, object?>("Selling Price", $"\"{sellingPrice}\""),
                new KeyValuePair<string, object?>("Category", 
                    categories != null ? $"\"{string.Join(",", categories).Trim()}\"" : null),
                new KeyValuePair<string, object?>("Description", 
                    description != null ? $"\"{string.Join("\n", description).Trim()}\"" : null),
            };

            return dataRow.ToArray();
        }

        private static KeyValuePair<string, object?>[] ExtractReviewInfo(Webpage webpage)
        {
            /// ====================================================================================
            /// https://www.amazon.com/product-reviews/B081H44MHD
            /// https://www.amazon.com/product-reviews/B099VMT8VZ
            /// ====================================================================================

            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            ////0
            var pageHtml = new HtmlDocument();
            pageHtml.LoadHtml(webpage.PageSource);

            var productXpath = "//*[@id='cm_cr-product_info']/div/div[2]/div/div/div[2]/div[1]/h1/a";
            var productNode = pageHtml.DocumentNode.SelectSingleNode(productXpath);
            var productName = productNode.InnerText.Trim();

            var sellerXpath = "//*[@id='cr-arp-byline']/a";
            var sellerNode = pageHtml.DocumentNode.SelectSingleNode(sellerXpath);
            var sellerName = sellerNode.InnerText.Trim();

            ////1
            var ratingText = webpage.PageText
                .Split("Customer reviews")[2]
                .Split("Write a review")[0];

            var splittedLines = (
                from line in ratingText.Split("\n", StringSplitOptions.TrimEntries)
                where string.IsNullOrEmpty(line) == false
                select line).ToArray();

            var archorRegex = new Regex(@"^[\d.]+ out of 5 stars$");

            string? overallScore = null;
            float? ratingScore = null;
            int? numRatings = null;
            string? fiveStar = null;
            string? fourStar = null;
            string? threeStar = null;
            string? twoStar = null;
            string? oneStar = null;

            for (int i = 0; i < splittedLines.Length; i++)
            {
                var archorMatch = archorRegex.Match(splittedLines[i]);
                if (archorMatch.Success)
                {
                    overallScore = archorMatch.Value;

                    ratingScore = float.Parse(
                        overallScore.Replace(" out of 5 stars", ""));

                    numRatings = int.Parse(
                        Regex.Match(splittedLines[i + 2], @"[\d,]+")
                        .Value.Replace(",", ""));

                    fiveStar = splittedLines[i + 4];
                    fourStar = splittedLines[i + 6];
                    threeStar = splittedLines[i + 8];
                    twoStar = splittedLines[i + 10];
                    oneStar = splittedLines[i + 12];

                    break;
                }
            }

            var dataRow = new List<KeyValuePair<string, object?>>()
            {
                new KeyValuePair<string, object?>("Product Name", $"\"{productName}\""),
                new KeyValuePair<string, object?>("Seller Name", $"\"{sellerName}\""),
                new KeyValuePair<string, object?>("Overall Score", overallScore),
                new KeyValuePair<string, object?>("Rating Score", ratingScore),
                new KeyValuePair<string, object?>("Number of Ratings", numRatings),
                new KeyValuePair<string, object?>("5 Star", fiveStar),
                new KeyValuePair<string, object?>("4 Star", fourStar),
                new KeyValuePair<string, object?>("3 Star", threeStar),
                new KeyValuePair<string, object?>("2 Star", twoStar),
                new KeyValuePair<string, object?>("1 Star", oneStar),
            };

            return dataRow.ToArray();
        }

        private static KeyValuePair<string, object?>[][] ExtractReviewDetails(Webpage webpage)
        {
            /// ====================================================================================
            /// https://www.amazon.com/product-reviews/B081H44MHD
            /// https://www.amazon.com/product-reviews/B099VMT8VZ
            /// ====================================================================================

            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            ////0
            var pageHtml = new HtmlDocument();
            pageHtml.LoadHtml(webpage.PageSource);

            var reviewSectionXPath = "//*[@id='cm_cr-review_list']";
            var reviewSectionNode = pageHtml.DocumentNode.SelectSingleNode(reviewSectionXPath);
            var reviewNodes = reviewSectionNode.SelectNodes("//div");

            var reviewIds = (
                from node in reviewNodes
                let id = node.GetAttributeValue("id", null)
                where string.IsNullOrEmpty(id) == false
                let foundedId = Regex.Match(id, @"^[\w\d]+$")
                where foundedId.Success
                where foundedId.Value.Length == 13 
                    || foundedId.Value.Length == 14
                select foundedId.Value).ToArray();

            var dateLocations = (
                from reviewId in reviewIds
                let xpath = $"//*[@id='customer_review-{reviewId}']/span"
                let node = reviewSectionNode.SelectSingleNode(xpath)
                select node.InnerText).ToArray();

            var reviewerNames = new List<string?>();
            foreach (var reviewId in reviewIds)
            {
                var node = reviewSectionNode
                    .SelectSingleNode($"//*[@id='customer_review-{reviewId}']/div[1]/a/div[2]/span");
                
                if (node == null)
                    node = reviewSectionNode
                        .SelectSingleNode($"//*[@id='customer_review-{reviewId}']/div[1]/div[1]/div/a/div[2]/span");

                reviewerNames.Add(node.InnerText);
            }

            var reviewRatings = (
                from reviewId in reviewIds
                let xpath = $"//*[@id='customer_review-{reviewId}']/div[2]/a[1]/i/span"
                let node = reviewSectionNode.SelectSingleNode(xpath)
                select node.InnerText).ToArray();

            var reviewTitles = (
                from reviewId in reviewIds
                let xpath = $"//*[@id='customer_review-{reviewId}']/div[2]/a[2]/span"
                let node = reviewSectionNode.SelectSingleNode(xpath)
                select node.InnerText).ToArray();

            var reviewContents = new List<string?>();
            foreach (var reviewId in reviewIds)
            {
                var node = reviewSectionNode
                    .SelectSingleNode($"//*[@id='customer_review-{reviewId}']/div[4]/span/span");

                if (node != null)
                    reviewContents.Add(node.InnerText);
                else
                    reviewContents.Add(null);
            }

            var verifiedPurchases = new List<string?>();
            foreach (var reviewId in reviewIds)
            {
                var node = reviewSectionNode
                    .SelectSingleNode($"//*[@id='customer_review-{reviewId}']/div[3]/a[2]/span");

                if (node != null && !string.IsNullOrEmpty(node.InnerText))
                    verifiedPurchases.Add(node.InnerText);
                else
                    verifiedPurchases.Add("Unverified Purchase");
            }

            var dataRows = new List<KeyValuePair<string, object?>[]>();
            for (int i = 0; i < reviewIds.Length; i++)
            {
                dataRows.Add(new KeyValuePair<string, object?>[]
                    {
                        new KeyValuePair<string, object?>("Review Id", $"\"{reviewIds[i]}\""),
                        new KeyValuePair<string, object?>("Date & Location", $"\"{dateLocations[i]}\""),
                        new KeyValuePair<string, object?>("Reviewer Name", $"\"{reviewerNames[i]}\""),
                        new KeyValuePair<string, object?>("Review Rating", $"\"{reviewRatings[i]}\""),
                        new KeyValuePair<string, object?>("Review Title", reviewTitles[i] != null ? $"\"{reviewTitles[i]?.Replace("\"", "\"\"")}\"" : null),
                        new KeyValuePair<string, object?>("Review Content", reviewContents[i] != null ? $"\"{reviewContents[i]?.Replace("\"", "\"\"")}\"" : null),
                        new KeyValuePair<string, object?>("Verified Purchase", $"\"{verifiedPurchases[i]}\""),
                    }
                );
            }

            return dataRows.ToArray();
        }

        #endregion DATA EXTRACTION
    }
}

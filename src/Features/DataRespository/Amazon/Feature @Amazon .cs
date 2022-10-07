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
using DxMLEngine.Objects;

using HtmlDocument = HtmlAgilityPack.HtmlDocument;

using DxMLEngine.Utilities;

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

            ////1            
            var searches = InputAsinSearches(inFile);

            ////2
            var browser = Browser.LaunghEdge();
            if (browser == null)
                throw new Exception("browser == null");

            ////3
            var foundedProducts = new List<Product>();
            for (int i = 0; i < searches.Length; i++)
            {
                var search = searches[i];

                var tempTab = Browser.OpenNewTab(browser, search.SearchUrl);
                search.PageText = Browser.CopyPageText(tempTab, 1000);
                search.PageSource = Browser.CopyPageSource(tempTab, 5000);

                search.PageLayout = CheckPageLayout(search);                
                Browser.CloseCurrentTab(tempTab);

            ////4
                if (search.NumberOfPages == null) 
                    search.NumberOfPages = FindNumberOfProductPages(search);
                if (search.NumberOfPages == null)
                    continue;

            ////5               
                for (int j = 0; j < 2; j++)
                {
                    search.SearchPageNumber = $"{j+1}";
                    Console.WriteLine($"\nCollect: {search.SearchUrl}");

                    var newTab = Browser.OpenNewTab(browser, search.SearchUrl);
                    search.PageText = Browser.CopyPageText(newTab, 1000);
                    search.PageSource = Browser.CopyPageSource(newTab, 5000);

                    var fileName = $"{search.Keyword?.Replace(" ", "")}Page{j + 1}";
                    OutputPageText(outDir, fileName, search);
                    OutputPageSource(outDir, fileName, search);

                    foundedProducts.AddRange(ExtractProductAsins(search));
                    Browser.CloseCurrentTab(newTab);
                }
            }

            Browser.CloseBrowser(browser!);

            ////6
            OutputProductAsins(outDir, "ProductAsins", foundedProducts.ToArray());
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
            var searches = InputProductSearches(inFile);

            ////2
            var browser = Browser.LaunghEdge();
            if (browser == null)
                throw new Exception("browser == null");

            ////4
            var products = new List<Product>();
            for (int i = 0; i < searches.Length; i++)
            {
                var search = searches[i];
                Console.WriteLine($"\nCollect: {search.DetailUrl}");

                var newTab = Browser.OpenNewTab(browser, search.DetailUrl);
                search.PageText = Browser.CopyPageText(newTab, 2000);
                search.PageSource = Browser.CopyPageSource(newTab, 5000);

                var fileName = $"{search.Asin}Page{i + 1}";
                OutputPageText(outDir, fileName, search);
                OutputPageSource(outDir, fileName, search);

                products.Add(ExtractProductDetails(search));
                Browser.CloseCurrentTab(newTab);
            }

            Browser.CloseBrowser(browser);

            ////5
            OutputProductDetails(outDir, "ProductDetails", products.ToArray());
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

            ////1            
            var searches = InputReviewSearches(inFile);

            ////2
            var browser = Browser.LaunghEdge();
            if (browser == null)
                throw new Exception("browser == null");

            ////3
            var reviews = new List<Review>();
            for (int i = 0; i < searches.Length; i++)
            {
                var search = searches[i];

                var tempTab = Browser.OpenNewTab(browser, search.ReviewUrl);
                search.PageText = Browser.CopyPageText(tempTab, 1000);
                search.PageSource = Browser.CopyPageSource(tempTab, 5000);

                Browser.CloseCurrentTab(tempTab);

            ////4
                if (search.NumberOfPages == null)
                    search.NumberOfPages = FindNumberOfReviewPages(search);
                if (search.NumberOfPages == null) continue;

            ////5
                for (int j = 0; j < search.NumberOfPages; j++)
                {
                    search.ReviewPageNumber = $"{j+1}";

                    Console.WriteLine($"\nCollect: {search.ReviewUrl}");
                    
                    var fileName = $"{search.Asin}Review{i + 1}";
                    OutputPageText(outDir, fileName, search);
                    OutputPageSource(outDir, fileName, search);

                    var newTab = Browser.OpenNewTab(browser, search.ReviewUrl);
                    search.PageText = Browser.CopyPageText(newTab, 2000);
                    search.PageSource = Browser.CopyPageSource(newTab, 5000);

                    reviews.Add(ExtractCustomerReviews(search));                 
                    Browser.CloseCurrentTab(newTab);
                }
            }

            Browser.CloseBrowser(browser);

            ////8
            OutputCustomerReviews(outDir, "CustomerReviews", reviews.ToArray());
        }

        #region INPUT OUTPUT

        private static Search[] InputAsinSearches(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);
            var searches = new List<Search>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var search = new Search();
                search.Keyword = Convert.ToString(dataFrame["Keyword"][i]);
                search.NumberOfPages = Convert.ToInt32(dataFrame["Number of Pages"][i]);
                
                searches.Add(search);
            }

            return searches.ToArray();
        }

        private static Search[] InputProductSearches(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);
            var searches = new List<Search>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var search = new Search();
                search.Asin = Convert.ToString(dataFrame["ASIN"][i]);
                searches.Add(search);
            }

            return searches.ToArray();
        }
        
        private static Search[] InputReviewSearches(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);
            var searches = new List<Search>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var search = new Search();
                search.Asin = Convert.ToString(dataFrame["ASIN"][i]);
                search.NumberOfPages = Convert.ToInt32(dataFrame["Number of Pages"][i]);

                switch (Convert.ToString(dataFrame["Filter By Star"][i]))
                {
                    case "Positive":
                        search.FilterByStar = "positive";
                        break;

                    case "Negative":
                        search.FilterByStar = "critical";
                        break;

                    default:
                        search.FilterByStar = null;
                        break;
                }

                searches.Add(search);
            }

            return searches.ToArray();
        }

        private static void OutputPageText(string location, string fileName, Search search)
        {
            Log.Info("OutputPageText");

            var path = $"{location}\\Datadoc @{fileName} #-------------- .txt";
            File.WriteAllText(path, search.PageText, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputPageSource(string location, string fileName, Search search)
        {
            Log.Info("OutputPageSource");

            var path = $"{location}\\Dataweb @{fileName} #-------------- .html";
            File.WriteAllText(path, search.PageSource, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputProductAsins(string location, string fileName, Product[] products)
        {
            var dataFrame = new DataFrame(new List<DataFrameColumn>()
            {
                new StringDataFrameColumn("ASIN"),
            });

            foreach (var product in products)
            {
                var dataRow = new List<KeyValuePair<string, object?>>()
                {
                    new KeyValuePair<string, object?>("ASIN", product.Asin),
                };

                Console.WriteLine(product.Asin);
                dataFrame.Append(dataRow, inPlace: true);
            }

            var path = $"{location}\\Dataset @{fileName} #-------------- .csv";
            DataFrame.WriteCsv(dataFrame, path, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputProductDetails(string location, string fileName, Product[] products)
        {
            var dataFrame = new DataFrame(new List<DataFrameColumn>()
            {
                new StringDataFrameColumn("ASIN"),
                new StringDataFrameColumn("Product Name"),
                new StringDataFrameColumn("Seller Name"),
                new StringDataFrameColumn("Selling Price"),
                new StringDataFrameColumn("Category"),
                new StringDataFrameColumn("Description"),
                new StringDataFrameColumn("URL"),
            });        
            
            foreach (var product in products)
            {
                var dataRow = new List<KeyValuePair<string, object?>>()
                {
                    new KeyValuePair<string, object?>("ASIN", $"\"{product.Asin}\""),
                    new KeyValuePair<string, object?>("Product Name", $"\"{product.Name}\""),
                    new KeyValuePair<string, object?>("Seller Name", $"\"{product.Seller}\""),
                    new KeyValuePair<string, object?>("Selling Price", $"\"{product.Price}\""),
                    new KeyValuePair<string, object?>("Category", product.Categories != null
                        ? $"\"{string.Join(", ", product.Categories)}\"" : null),
                    new KeyValuePair<string, object?>("Description", product.Description != null
                        ? $"\"{product.Description?.Replace("\"", "\"\"")}\"" : null),
                    new KeyValuePair<string, object?>("URL", $"\"{product.Url}\""),
                };

                dataFrame.Append(dataRow, inPlace: true);
            }

            var path = $"{location}\\Dataset @{fileName} #-------------- .csv";
            DataFrame.WriteCsv(dataFrame, path, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputCustomerReviews(string location, string fileName, Review[] reviews)
        {
            var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
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

            foreach (var review in reviews)
            {
                if (review.Comments == null) continue;
                foreach (var comment in review.Comments)
                {
                    var dataRow = new KeyValuePair<string, object?>[]
                    {
                        new KeyValuePair<string, object?>("ASIN", $"\"{review.Asin}\""),
                        new KeyValuePair<string, object?>("Product Name", $"\"{review.ProductName}\""),
                        new KeyValuePair<string, object?>("Seller Name", $"\"{review.SellerName}\""),
                        new KeyValuePair<string, object?>("Overall Score", $"\"{review.OverallScore}\""),
                        new KeyValuePair<string, object?>("Rating Score", $"\"{review.RatingScore}\""),
                        new KeyValuePair<string, object?>("Number of Ratings", $"\"{review.NumberOfRating}\""),
                        new KeyValuePair<string, object?>("5 Star", $"\"{review.FiveStar}\""),
                        new KeyValuePair<string, object?>("4 Star", $"\"{review.FourStar}\""),
                        new KeyValuePair<string, object?>("3 Star", $"\"{review.ThreeStar}\""),
                        new KeyValuePair<string, object?>("2 Star", $"\"{review.TwoStar}\""),
                        new KeyValuePair<string, object?>("1 Star", $"\"{review.OneStar}\""),

                        new KeyValuePair<string, object?>("Review Id", $"\"{comment.Id}\""),
                        new KeyValuePair<string, object?>("Date & Location", $"\"{comment.DateLocation}\""),
                        new KeyValuePair<string, object?>("Reviewer Name", $"\"{comment.ReviewerName}\""),
                        new KeyValuePair<string, object?>("Review Rating", $"\"{comment.ReviewRating}\""),
                        new KeyValuePair<string, object?>("Review Title", comment.ReviewTitle != null 
                            ? $"\"{comment.ReviewTitle.Replace("\"", "\"\"")}\"" : null),
                        new KeyValuePair<string, object?>("Review Content", comment.ReviewContent != null
                            ? $"\"{comment.ReviewContent.Replace("\"", "\"\"")}\"" : null),
                        new KeyValuePair<string, object?>("Verified Purchase", $"\"{comment.VerifiedPurchase}\""),
                        
                        new KeyValuePair<string, object?>("URL", $"\"{review.Url}\""),
                    };

                    dataFrame.Append(dataRow, inPlace: true);
                }             
            }

            var path = $"{location}\\Dataset @{fileName} #-------------- .csv";
            DataFrame.WriteCsv(dataFrame, path, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        #endregion INPUT OUTPUT

        #region PAGE CHECKING

        private static int FindNumberOfProductPages(Search search)
        {
            if (search.PageText == null)
                throw new ArgumentException("search.PageText == null");

            if (search.PageSource == null)
                throw new ArgumentException("search.PageSource == null");

            var targetText = search.PageText
                .Split("RESULTS")[0];

            Match? match = null;
            while (true)
            {
                match = Regex.Match(targetText, @"1-16 of( over)* [\d,]+ results for ");
                if (match.Success) break;

                match = Regex.Match(targetText, @"1-48 of( over)* [\d,]+ results for ");
                if (match.Success) break;
            }

            var numResults = int.Parse(
                Regex.Match(match.Value[4..], @"[\d,]+").Value.Replace(",", ""));

            var perPage = int.Parse(
                Regex.Match(match.Value[..4], @"[-][\d,]+").Value.Replace("-", ""));

            return int.Parse((numResults / perPage).ToString().Split(".")[0]);
        }
    
        private static int FindNumberOfReviewPages(Search search)
        {
            if (search.PageSource == null)
                throw new ArgumentException("search.PageSource == null");

            var pageHtml = new HtmlDocument();
            pageHtml.LoadHtml(search.PageSource);

            var numResultXPath = "//*[@id='filter-info-section']/div";
            var numResultNode = pageHtml.DocumentNode.SelectSingleNode(numResultXPath);
            var numResults = int.Parse(Regex
                .Replace(numResultNode.InnerText, @"[\d,]+ total ratings, ", "")
                .Replace(" with reviews", "")
                .Replace(",", ""));

            return int.Parse((numResults / 10).ToString().Split(".")[0]);
        }
        
        private static PageLayout CheckPageLayout(Search search)
        {
            if (search.PageText == null)
                throw new ArgumentException("search.PageText == null");

            if (search.PageSource == null)
                throw new ArgumentException("search.PageSource == null");

            ////0
            var targetText = search.PageText
                .Split("RESULTS")[0];

            var regexA = new Regex(@"1-48 of( over)* [\d,]+ results for \u0022[\w]+\u0022");
            var matchA = regexA.Match(targetText);
            if (matchA.Success) return PageLayout.FormA;

            var regexB = new Regex(@"1-16 of( over)* [\d,]+ results for \u0022[\w]+\u0022");
            var matchB = regexB.Match(targetText);
            if (matchB.Success) return PageLayout.FormB;

            return PageLayout.Unknown;
        }

        #endregion PAGE CHECKING

        #region DATA EXTRACTION

        private static Product[] ExtractProductAsins(Search search)
        {
            var foundedAsins = new List<string>();
            
            if (search.PageLayout == PageLayout.FormA)
                foundedAsins.AddRange(ExtractAsinFormA(search));
            
            if (search.PageLayout == PageLayout.FormB)
                foundedAsins.AddRange(ExtractAsinFormB(search));

            var products = new List<Product>();
            foreach (var foundedAsin in foundedAsins)
            {
                var product = new Product();
                product.Asin = foundedAsin;
                products.Add(product);
            }
            return products.ToArray();
        }

        private static string[] ExtractAsinFormA(Search search)
        {
            /// ====================================================================================
            /// https://www.amazon.com/s?k=chair
            /// ====================================================================================

            if (search.PageText == null)
                throw new ArgumentException("search.PageText == null");

            if (search.PageSource == null)
                throw new ArgumentException("search.PageSource == null");

            ////0
            var targetText = search.PageText
                .Split("RESULTS")[0];

            var regex = new Regex(@"1-48 of over [\d,]+ results for \u0022[\w]+\u0022");
            var match = regex.Match(targetText);

            var keyword = Regex
                .Replace(match.Value, @"1-48 of over [\d,]+ results for ", "")
                .Replace("\u0022", "");

            ////1
            var targetSource = search.PageSource
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

        private static string[] ExtractAsinFormB(Search search)
        {
            /// ====================================================================================
            /// https://www.amazon.com/s?k=electronics
            /// ====================================================================================

            if (search.PageText == null)
                throw new ArgumentException("search.PageText == null");

            if (search.PageSource == null)
                throw new ArgumentException("search.PageSource == null");

            ////0
            var targetText = search.PageText
                .Split("RESULTS")[0];

            var regex = new Regex(@"1-16 of over [\d,]+ results for \u0022[\w]+\u0022");
            var match = regex.Match(targetText);

            var keyword = Regex
                .Replace(match.Value, @"1-16 of over [\d,]+ results for ", "")
                .Replace("\u0022", "");

            var targetSource = search.PageSource
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

        private static Product ExtractProductDetails(Search search)
        {
            /// ====================================================================================
            /// https://www.amazon.com/dp/B081H44MHD
            /// https://www.amazon.com/dp/B099VMT8VZ
            /// ====================================================================================

            if (search.PageText == null)
                throw new ArgumentException("search.PageText == null");

            if (search.PageSource == null)
                throw new ArgumentException("search.PageSource == null");

            var product = new Product();
            product.Asin = search.Asin;
            product.Url = search.DetailUrl;

            ////0
            var targetText = search.PageText
                .Split("Sell on Amazon")[1];

            var splittedLines = (
                from line in targetText.Split("\n", StringSplitOptions.TrimEntries)
                where string.IsNullOrEmpty(line) == false
                select line).ToArray();

            var archorRegex = new Regex(@"[\d.]+ out of 5 stars [\d, ]* ratings");
            var priceRegex = new Regex(@"\$(0|[1-9][0-9]{0,2})(,\d{3})*(\.\d{1,2})?");

            for (int i = 0; i < splittedLines.Length; i++)
            {
                var archorMatch = archorRegex.Match(splittedLines[i]);
                if (archorMatch.Success)
                {
                    for (int j = i; j > 0; j--)
                    {
                        if (splittedLines[j].Contains("Visit"))
                        {
                            product.Seller = splittedLines[j].Replace("Visit the ", "").Trim();
                            product.Name = splittedLines[j-1].Trim();                            
                            break;
                        }
                    }

                    for (int j = i; j < i+5; j++)
                    {
                        var priceMatch = priceRegex.Match(splittedLines[j]);
                        if (priceMatch.Success)
                            product.Price = priceMatch.Value;
                    }

                    break;
                }
            }

            //// 
            try
            {
                var categorySource = search.PageSource
                    .Split("<ul class=\"a-unordered-list a-horizontal a-size-small\">")[1]
                    .Split("</ul>")[0];

                var categoryHtml = new HtmlDocument();
                categoryHtml.LoadHtml(categorySource);

                var categoryXPath = "//*[@class='a-link-normal a-color-tertiary']";
                var categoryNodes = categoryHtml.DocumentNode.SelectNodes(categoryXPath);
                
                product.Categories = (
                    from node in categoryNodes
                    let category = node.InnerText
                    where string.IsNullOrEmpty(category) == false
                    select category.Trim()).ToArray();
            }
            catch { product.Categories = null; }

            ////          
            var descriptionHtml = new HtmlDocument();
            descriptionHtml.LoadHtml(search.PageSource);

            try
            {
                var desriptionXPath = "//*[@id='productDescription']";
                var decriptionNode = descriptionHtml.DocumentNode.SelectSingleNode(desriptionXPath);

                if (decriptionNode != null)
                    product.Description = decriptionNode.InnerText.Trim();
            }
            catch { product.Description = null; }

            return product;
        }

        private static Review ExtractCustomerReviews(Search search)
        {
            var review = new Review();
            ExtractReviewInfo(search, ref review);
            ExtractReviewDetails(search, ref review);

            return review;
        }

        private static void ExtractReviewInfo(Search search, ref Review review)
        {
            /// ====================================================================================
            /// https://www.amazon.com/product-reviews/B081H44MHD
            /// https://www.amazon.com/product-reviews/B099VMT8VZ
            /// ====================================================================================

            if (search.PageText == null)
                throw new ArgumentException("search.PageText == null");

            if (search.PageSource == null)
                throw new ArgumentException("search.PageSource == null");

            ////0
            var pageHtml = new HtmlDocument();
            pageHtml.LoadHtml(search.PageSource);

            var productXpath = "//*[@id='cm_cr-product_info']/div/div[2]/div/div/div[2]/div[1]/h1/a";
            var productNode = pageHtml.DocumentNode.SelectSingleNode(productXpath);
            var productName = productNode.InnerText.Trim();

            var sellerXpath = "//*[@id='cr-arp-byline']/a";
            var sellerNode = pageHtml.DocumentNode.SelectSingleNode(sellerXpath);
            var sellerName = sellerNode.InnerText.Trim();

            ////1
            var ratingText = search.PageText
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

            review.Asin = search.Asin;
            review.ProductName = productName;
            review.SellerName = sellerName;
            review.OverallScore = overallScore;
            review.RatingScore = ratingScore;
            review.NumberOfRating = numRatings;
            review.FiveStar = fiveStar;
            review.FourStar = fourStar;
            review.ThreeStar = threeStar;
            review.TwoStar = twoStar;
            review.OneStar = oneStar;            
        }

        private static void ExtractReviewDetails(Search search, ref Review review)
        {
            /// ====================================================================================
            /// https://www.amazon.com/product-reviews/B081H44MHD
            /// https://www.amazon.com/product-reviews/B099VMT8VZ
            /// ====================================================================================

            if (search.PageText == null)
                throw new ArgumentException("search.PageText == null");

            if (search.PageSource == null)
                throw new ArgumentException("search.PageSource == null");

            ////0
            var pageHtml = new HtmlDocument();
            pageHtml.LoadHtml(search.PageSource);

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

            var comments = new List<Review.Comment>();
            for (int i = 0; i < reviewIds.Length; i++)
            {
                var comment = new Review.Comment(); 
                comment.Id = reviewIds[i];
                comment.DateLocation = dateLocations[i];
                comment.ReviewerName = reviewerNames[i];
                comment.ReviewRating = reviewRatings[i];
                comment.ReviewTitle = reviewTitles[i];
                comment.ReviewContent = reviewContents[i];
                comment.VerifiedPurchase = verifiedPurchases[i];

                comments.Add(comment);
            }

            review.Comments = comments.ToArray();
        }

        #endregion DATA EXTRACTION
    }
}

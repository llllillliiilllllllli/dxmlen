using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Net;
using System.Drawing;

using Microsoft.Data.Analysis;

using DxMLEngine.Utilities;
using DxMLEngine.Attributes;
using DxMLEngine.Functions;

using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace DxMLEngine.Features.GooglePatents
{
    [Feature]
    internal class GooglePatents
    {
        public static void CollectPatentCodes()
        {
            /// ====================================================================================
            /// Search patent codes from Google Patents with given query and parameters
            /// 
            /// >>> param:  string  # path to input file containing list of search queries 
            /// >>> param:  string  # path to output folder for storing collected data 
            /// >>> param:  string  # name of file to be saved as output after searching 
            ///             
            /// >>> funct:  0       # inquire user input for paths and optional arguments
            /// >>> funct:  1       # read queries data from csv file into data frame
            /// >>> funct:  2       # instantiate new Edge browser with maximized window
            /// >>> funct:  3       # configure each webpage url using search by methods
            /// >>> funct:  4       # copy raw text and source from webpage using clipboard
            /// >>> funct:  5       # determine number of pages by total number of results
            /// >>> funct:  6       # start extracting all patent codes in earch result page
            /// >>> funct:  7       # export founded patent code data saving to csv file
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
            
            Console.Write("\nEnter output file name: ");
            var outFile = Console.ReadLine()?.Replace(" ", "");

            if (string.IsNullOrEmpty(outFile))
                throw new ArgumentNullException("file name is null or empty");            

            ////1            
            var webpages = InputSearchParameters(inFile);
            
            ////2
            var browser = Browser.LaunghEdge();
            if (browser == null)
                throw new Exception("browser == null");

            ////3
            var patentCodes = new List<PatentCode>();
            foreach (var webpage in webpages)
            {        
                var tempTab = Browser.OpenNewTab(browser, webpage.SearchUrl);
                webpage.PageText = Browser.CopyPageText(tempTab);
                webpage.PageSource = Browser.CopyPageSource(tempTab);
                Browser.CloseCurrentTab(tempTab);

            ////4
                if (webpage.NumberOfPages == null)
                    webpage.NumberOfPages = FindNumberOfPages(webpage);

                if (webpage.NumberOfPages == null) continue;

            ////5              
                for (int i = 0; i <= webpage.NumberOfPages; i++)
                {
                    webpage.PageNumber = $"{i}";
                    Console.WriteLine($"\nCollect: {webpage.SearchUrl}");

                    var newTab = Browser.OpenNewTab(browser, webpage.SearchUrl);
                    webpage.PageText = Browser.CopyPageText(newTab);
                    webpage.PageSource = Browser.CopyPageSource(newTab);

                    OutputSearchPageText(outDir, webpage);
                    OutputSearchPageSource(outDir, webpage);

                    patentCodes.AddRange(ExtractPatentCodes(webpage));

                    Browser.CloseCurrentTab(newTab);
                }
            }

            Browser.CloseBrowser(browser);

            ////6
            OutputPatentCodes(outDir, outFile, patentCodes.ToArray());
        }
    
        public static void CollectPatentDetails()
        {
            /// ====================================================================================
            /// Collect patents from Google Patents using codes registed througth patent offices   
            /// 
            /// >>> param:  string  # path to input file containing list of patent codes 
            /// >>> param:  string  # path to output folder for storing collected data 
            /// >>> param:  string  # name of file for saving unprocessed patent text data 
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
            var options = new string[13]
            {
                "Abstract",
                "Images",
                "Classifications",
                "GeneralInfo",

                "Description",
                "Claims",
                "Concepts",

                "PatentCitations",
                "NonPatentCitations",
                "CitedBy",
                "SimilarDocuments",

                "PriorityApplications",
                "LegalEvents"
            };

            Console.WriteLine("\nSelect from options: ");
            for (int i = 0; i < options.Length; i++)
                Console.WriteLine($"{i+1, 2} {options[i]}");

            Console.Write("\nSelect: ");
            var selection = Console.ReadLine();

            if (string.IsNullOrEmpty(selection))
                throw new ArgumentNullException("selection is null or empty");

            if (!string.IsNullOrEmpty(selection))
            {
                var selections = (
                    from option in selection.Split(", ")
                    where options.Contains(option)
                    select option).ToArray();

                options = selections.ToArray();
            }

            ////2
            var webpages = InputPatentCodes(inFile);

            ////3
            var browser = Browser.LaunghEdge();
            if (browser == null)
                throw new Exception("browser == null");

            ////4
            foreach (var webpage in webpages)
            {
                Console.WriteLine($"\nCollect: {webpage.PatentUrl}");

                var newTab = Browser.OpenNewTab(browser, webpage.PatentUrl);
                webpage.PageText = Browser.CopyPageText(newTab);
                webpage.PageSource = Browser.CopyPageSource(newTab);

                if (CheckTextHeaders(webpage).Length == 0) continue;
                if (CheckSourceHeaders(webpage).Length == 0) continue;

                OutputDetailPageText(outDir, webpage);
                OutputDetailPageSource(outDir, webpage);
            
            ////5
                var patent = new Patent();

                patent.Title = ExtractTitle(webpage);
                patent.Url = webpage.PatentUrl;

                if (options.Contains("Abstract")) 
                    patent.Abstract = ExtractAbstract(webpage);

                if (options.Contains("Images")) 
                    patent.Images = ExtractImages(webpage);

                if (options.Contains("Classifications"))
                    patent.Classifications = ExtractClassifications(webpage);

                if (options.Contains("GeneralInfo"))
                    patent.GeneralInfo = ExtractGeneralInfo(webpage);

                if (options.Contains("Description"))
                    patent.Description = ExtractDescription(webpage);

                if (options.Contains("Claims"))
                    patent.Claims = ExtractClaims(webpage);

                if (options.Contains("Concepts"))
                    patent.Concepts = ExtractConcepts(webpage);

                if (options.Contains("PatentCitations"))
                    patent.PatentCitations = ExtractPatentCitations(webpage);

                if (options.Contains("NonPatentCitations"))
                    patent.NonPatentCitations = ExtractNonPatentCitations(webpage);

                if (options.Contains("CitedBy"))
                    patent.CitedBy = ExtractCitedBy(webpage);

                if (options.Contains("SimilarDocuments"))
                    patent.SimilarDocuments = ExtractSimilarDocuments(webpage);

                if (options.Contains("PriorityApplications"))
                    patent.PriorityApplications = ExtractPriorityApplications(webpage);

                if (options.Contains("LegalEvents"))
                    patent.LegalEvents = ExtractLegalEvents(webpage);

                Browser.CloseCurrentTab(newTab);

                OutputPatentDetails(outDir, webpage.PatentCode!, patent);
            }
        }

        #region INPUT OUTPUT

        private static Webpage[] InputSearchParameters(string path)
        {
            Log.Info("InputSearchParameters");
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);

            var webpages = new List<Webpage>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var webpage = new Webpage();

                webpage.PatentCode = Convert.ToString(dataFrame["Patent Code"][i]);
                webpage.ClassCode = Convert.ToString(dataFrame["Class Code"][i]);
                webpage.Keyword = Convert.ToString(dataFrame["Keyword"][i]);

                webpage.Before = Convert.ToString(dataFrame["Before"][i]);
                webpage.After = Convert.ToString(dataFrame["After"][i]);
                webpage.Inventor = Convert.ToString(dataFrame["Inventor"][i]);
                webpage.Assignee = Convert.ToString(dataFrame["Assignee"][i]);

                webpage.Country = Convert.ToString(dataFrame["Country"][i]);
                webpage.Language = Convert.ToString(dataFrame["Language"][i]);
                webpage.Status = Convert.ToString(dataFrame["Status"][i]);
                webpage.Type = Convert.ToString(dataFrame["Type"][i]);
                webpage.Litigation = Convert.ToString(dataFrame["Litigation"][i]);
                
                var searchBy = Convert.ToString(dataFrame["Search By"][i]);
                var NumberOfPages = dataFrame["Number of Pages"][i];

                switch (searchBy)
                {
                    case "Patent Code":
                        webpage.SearchBy = SearchBy.PatentCode;
                        break;

                    case "Class Code":
                        webpage.SearchBy = SearchBy.ClassCode;
                        break;

                    case "Keyword":
                        webpage.SearchBy = SearchBy.Keyword;
                        break;

                    default:
                        webpage.SearchBy = null;
                        break;
                }

                webpage.NumberOfPages = NumberOfPages != null ? Convert.ToInt32(NumberOfPages) : null;

                if (webpage.SearchBy == SearchBy.PatentCode) Console.WriteLine($"{webpage.PatentCode}");
                if (webpage.SearchBy == SearchBy.ClassCode) Console.WriteLine($"{webpage.ClassCode}");
                if (webpage.SearchBy == SearchBy.Keyword) Console.WriteLine($"{webpage.Keyword}");

                webpages.Add(webpage);
            }

            return webpages.ToArray();
        }

        private static Webpage[] InputPatentCodes(string path)
        {
            Log.Info("InputPatentCodes");
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);

            var webpages = new List<Webpage>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var webpage = new Webpage();

                webpage.PatentCode = dataFrame["Patent Code"][i] != null
                    ? dataFrame["Patent Code"][i]!.ToString()!.Replace(" ", "+") : null;

                Console.WriteLine($"{webpage.PatentCode}");
                webpages.Add(webpage);
            }

            return webpages.ToArray();
        }

        private static void OutputSearchPageText(string location, Webpage webpage)
        {
            Log.Info("OutputSearchPageText");
            var path = $"{location}\\Datadoc @{webpage.Id}SearchPage #-------------- .txt";
            File.WriteAllText(path, webpage.PageText, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputSearchPageSource(string location, Webpage webpage)
        {
            Log.Info("OutputSearchPageSource");
            var path = $"{location}\\Webpage @{webpage.Id}SearchPage #-------------- .html";
            File.WriteAllText(path, webpage.PageSource, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputDetailPageText(string location, Webpage webpage)
        {
            Log.Info("OutputDetailPageText");
            var path = $"{location}\\Datadoc @{webpage.PatentCode}DetailPage #-------------- .txt";
            File.WriteAllText(path, webpage.PageText, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputDetailPageSource(string location, Webpage webpage)
        {
            Log.Info("OutputDetailPageSource");
            var path = $"{location}\\Webpage @{webpage.PatentCode}DetailPage #-------------- .html";
            File.WriteAllText(path, webpage.PageSource, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputPatentCodes(string location, string fileName, PatentCode[] patentCodes)
        {
            Log.Info("OutputPatentCodes");
            var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
                    new StringDataFrameColumn("Patent Code"),
                    new StringDataFrameColumn("Class Code"),                    
                    new StringDataFrameColumn("Keyword"),
                    new StringDataFrameColumn("Before"),
                    new StringDataFrameColumn("After"),
                    new StringDataFrameColumn("Inventor"),
                    new StringDataFrameColumn("Assignee"),
                    new StringDataFrameColumn("Country"),
                    new StringDataFrameColumn("Language"),
                    new StringDataFrameColumn("Status"),
                    new StringDataFrameColumn("Type"),
                    new StringDataFrameColumn("Litigation"),
                }
            );

            foreach (var patentCode in patentCodes)
            {
                var dataRow = new List<KeyValuePair<string, object?>>()
                {
                    new KeyValuePair<string, object?>("Patent Code", patentCode.Code),
                    new KeyValuePair<string, object?>("Class Code", patentCode.ClassCode),
                    new KeyValuePair<string, object?>("Keyword", patentCode.Keyword),
                    new KeyValuePair<string, object?>("Before", patentCode.Before),
                    new KeyValuePair<string, object?>("After", patentCode.After),
                    new KeyValuePair<string, object?>("Inventor", patentCode.Inventor),
                    new KeyValuePair<string, object?>("Assignee", patentCode.Assignee),
                    new KeyValuePair<string, object?>("Country", patentCode.Country),
                    new KeyValuePair<string, object?>("Language", patentCode.Language),
                    new KeyValuePair<string, object?>("Status", patentCode.Status),
                    new KeyValuePair<string, object?>("Type", patentCode.Type),
                    new KeyValuePair<string, object?>("Litigation", patentCode.Litigation),
                };

                Console.WriteLine($"{patentCode.Code}");
                dataFrame.Append(dataRow, inPlace: true);
            }

            var path = $"{location}\\Dataset @{fileName} #-------------- .csv";
            DataFrame.WriteCsv(dataFrame, path, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputPatentDetails(string location, string fileName, Patent patent)
        {
            Log.Info("OutputPatentDetails");

            var jsonString = JsonSerializer.Serialize(patent, new JsonSerializerOptions() {  WriteIndented = true });

            Console.WriteLine($"{patent.Title}");

            var path = $"{location}\\Datason @{fileName} #-------------- .json";
            File.WriteAllText(path, jsonString, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        #endregion INPUT OUTPUT            
                
        #region PAGE CHECKING
        
        private static int? FindNumberOfPages(Webpage webpage)
        {
            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            var regex = new Regex(@"(About) [\d,]+ (results)");
            var match = regex.Match(webpage.PageText);

            if (match.Success)
            {
                var numResults = int.Parse(match.Value.Split(" ")[1].Replace(",", ""));

                var numPages = 99;
                if (numResults < 1000)
                {
                    if (numResults < 100)
                    {
                        var digits = numResults.ToString();
                        numPages = int.Parse(digits[0].ToString());
                    }
                    else
                    {
                        var digits = numResults.ToString();
                        numPages = int.Parse(digits[0..2]);
                    }
                }
                return numPages;
            }
            else
                return null;
        }
        
        private static string[] CheckTextHeaders(Webpage webpage)
        {
            /// ====================================================================================
            /// Abstract
            /// Images (16)
            /// Classifications
            /// Description
            /// Claims (36)
            /// Patent Citations (69)
            /// Non-Patent Citations (17)
            /// Cited By (77)
            /// Similar Documents
            /// Parent Applications (1)
            /// Priority Applications (6)
            /// Applications Claiming Priority (1)
            /// Legal Events
            /// Concepts
            /// ====================================================================================
            
            if (webpage.PageText == null)
                throw new ArgumentNullException("webpage.PageText == null");

            var textHeaders = new string[]
            {
                "Abstract",
                "Images",
                "Classifications",
                "Description",
                "Claims",
                "Patent Citations",
                "Non-Patent Citations",
                "Cited By",
                "Similar Documents",
                "Parent Applications",
                "Priority Applications",
                "Applications Claiming Priority",
                "Legal Events",
                "Concepts",
            };

            var splittedLines =
                from line in webpage.PageText.Split("\n", StringSplitOptions.TrimEntries)
                where string.IsNullOrEmpty(line) == false
                select line;

            var foundedHeaders = (
                from line in splittedLines
                from header in textHeaders
                where line.Contains(header)
                select header).ToArray();

            return foundedHeaders;
        }

        private static string[] CheckSourceHeaders(Webpage webpage)
        {
            /// ====================================================================================
            /// <h2>Info</h2>
            /// <h2>Links</h2>
            /// <h2>Images</h2>
            /// <h2>Classifications</h2>
            /// <h2>Abstract</h2>
            /// <h2>Description</h2>
            /// <h2>Claims (<span itemprop="count">36</span>)</h2>
            /// <h2>Abstract</h2>
            /// <h2>Priority Applications (6)</h2>
            /// <h2>Applications Claiming Priority (1)</h2>
            /// <h2>Related Parent Applications (1)</h2>
            /// <h2>Publications (2)</h2>
            /// <h2>ID=53042265</h2>
            /// <h2>Family Applications (2)</h2>
            /// <h2>Family Applications Before (1)</h2>
            /// <h2>Country Status (15)</h2>
            /// <h2>Cited By (2)</h2>
            /// <h2>Families Citing this family (75)</h2>
            /// <h2>Citations (57)</h2>
            /// <h2>Family Cites Families (12)</h2>
            /// <h2>Patent Citations (58)</h2>
            /// <h2>Non-Patent Citations (17)</h2>
            /// <h2>Cited By (4)</h2>
            /// <h2>Also Published As</h2>
            /// <h2>Similar Documents</h2>
            /// <h2>Legal Events</h2>
            /// ====================================================================================

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageText == null");

            var foundedHeaders = (
                from match in Regex.Matches(webpage.PageSource, @"<h2>[\w\d\s\n\r\t.]+<[/]h2>")
                where match.Success && string.IsNullOrEmpty(match.Value) == false
                select match.Value.Trim()).ToArray();

            return foundedHeaders;
        }

        #endregion PAGE CHECKING
                
        #region DATA EXTRACTION

        private static PatentCode[] ExtractPatentCodes(Webpage webpage)
        {
            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            var foundedPatentCodes =
                from field in typeof(PatentCodePatterns).GetFields()
                let patentCodePattern = (string?)field.GetValue(null)
                let codeRegex = new Regex(patentCodePattern)
                let codeMatches = codeRegex.Matches(webpage.PageText)
                from codeMatch in codeMatches
                where codeMatch.Success
                where Regex.Match(codeMatch.Value, @"[\d]+").Success
                select codeMatch.Value;

            var patentCodes = new List<PatentCode>();
            foreach (var foundedPatentCode in foundedPatentCodes)
            {
                var patentCode = new PatentCode();
                patentCode.Code = foundedPatentCode;
                patentCode.ClassCode = webpage.ClassCode;
                patentCode.Keyword = webpage.Keyword;
                patentCode.Before = webpage.Before;
                patentCode.After = webpage.After;
                patentCode.Inventor = webpage.Inventor;
                patentCode.Assignee = webpage.Assignee;
                patentCode.Country = webpage.Country;
                patentCode.Language = webpage.Language;
                patentCode.Status = webpage.Status;
                patentCode.Type = webpage.Type;
                patentCode.Litigation = webpage.Litigation;
                
                patentCodes.Add(patentCode);
            }

            return patentCodes.ToArray();
        }

        private static string ExtractTitle(Webpage webpage)
        {        
            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            var targetSource = webpage.PageSource
                .Split("<body unresolved>")[0];

            var pageHtml = new HtmlDocument();
            pageHtml.LoadHtml(targetSource);

            var titleNode = (
                from node in pageHtml.DocumentNode.SelectNodes("/html/head/meta")
                where node.GetAttributeValue("name", null) == "DC.title"
                select node.GetAttributeValue("content", null)).First();

            return titleNode.Trim();
        }

        private static Abstract? ExtractAbstract(Webpage webpage)
        {
            /// ====================================================================================
            /// Extract summary from Google Patents webpage using raw text and page source
            /// 
            /// >>> param:  string  # raw text content collected from Google Patents webpage
            ///             
            /// >>> funct:  0       # retrieve patent codes for regular searching and selection
            /// >>> funct:  1       # select the first patent code to locate text locations
            /// >>> funct:  2       # scan through different portions in raw text to get data
            /// ====================================================================================
            
            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            var pageHtml = new HtmlDocument();
            pageHtml.LoadHtml(webpage.PageSource);

            var language = (
                from node in pageHtml.DocumentNode.SelectNodes("/html/head/link")
                where node.GetAttributeValue("rel", null) == "canonical"
                let href = node.GetAttributeValue("href", null)                
                select href.Split("/").Last()).First().ToUpper();

            var content = (
                from node in pageHtml.DocumentNode.SelectNodes("/html/head/meta")
                where node.GetAttributeValue("name", null) == "DC.description"
                select node.GetAttributeValue("content", null).Trim()).First();

            return new Abstract(content, language);
        }

        private static Images? ExtractImages(Webpage webpage)
        {
            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            var textHeaders = CheckTextHeaders(webpage);
            if (!textHeaders.Contains("Images")) return null;

            var targetSource = webpage.PageSource
                .Split("<h2>Images</h2>")[1]
                .Split("<h2>Classifications</h2>")[0];

            var imageHtml = new HtmlDocument();
            imageHtml.LoadHtml(targetSource);

            var thumbnailXPath = "/ul/li/img";
            var thumbnailNodes = imageHtml.DocumentNode.SelectNodes(thumbnailXPath);

            var fullImageXPath = "/ul/li/meta";
            var fullImageNodes = imageHtml.DocumentNode.SelectNodes(fullImageXPath);

            var imagesHrefs = new List<Images.ImageHref>();
            for (int i = 0; i < thumbnailNodes.Count; i++)
            {
                var thumbnailHref = thumbnailNodes[i].GetAttributeValue("src", null);
                var fullImageHref = fullImageNodes[i].GetAttributeValue("content", null);
                imagesHrefs.Add(new Images.ImageHref(thumbnailHref, fullImageHref));
            }

            return new Images(imagesHrefs.ToArray());
        }

        private static Classifications? ExtractClassifications(Webpage webpage)
        {
            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            var textHeaders = CheckTextHeaders(webpage);
            if (!textHeaders.Contains("Classifications")) return null;
            var sourceHeaders = CheckSourceHeaders(webpage);
            if (!sourceHeaders.Contains("<h2>Classifications</h2>")) return null;

            var targetSource = webpage.PageSource
                .Split("<h2>Classifications</h2>")[1];

            var classHtml = new HtmlDocument();
            classHtml.LoadHtml(targetSource);

            var classXPath = "/ul/li/ul/li/span";
            var classNodes = classHtml.DocumentNode.SelectNodes(classXPath);

            var classCodes = (
                from node in classNodes
                let itemprop = node.GetAttributeValue("itemprop", null)
                where itemprop == "Code"
                select node.InnerText).ToArray();

            var classDescriptions = (
                from node in classNodes
                let itemprop = node.GetAttributeValue("itemprop", null)
                where itemprop == "Description"
                select node.InnerText).ToArray();

            var classes = new List<Classifications.Class>();
            for (int i = 0; i < classCodes.Length; i++)
                classes.Add(new Classifications.Class(classCodes[i], classDescriptions[i]));

            return new Classifications(classes.ToArray());
        }

        private static GeneralInfo? ExtractGeneralInfo(Webpage webpage)
        {
            /// ====================================================================================
            /// Extract info card section from Google Patents webpage using raw text data
            /// 
            /// >>> param:  string  # raw text content collected from Google Patents webpage
            ///             
            /// >>> funct:  0       # retrieve patent codes for regular searching and selection
            /// >>> funct:  1       # select the first patent code to locate text locations
            /// >>> funct:  2       # scan through different portions in raw text to get data
            /// ====================================================================================

            ////0
            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");
            
            ////
            var sourceHeaders = CheckSourceHeaders(webpage);
            if (!sourceHeaders.Contains("<h2>Info</h2>")) return null;

            ////
            var targetSource = webpage.PageSource
                .Split("<h2>Info</h2>")[1];

            var infoHtml = new HtmlDocument();
            infoHtml.LoadHtml(targetSource);

            var headInfoXPath = "/dl/dd";
            var bodyInfoXPath = "/dd";
            var headInfoNodes = infoHtml.DocumentNode.SelectNodes(headInfoXPath);
            var bodyInfoNodes = infoHtml.DocumentNode.SelectNodes(bodyInfoXPath);

            ////
            var publicationNumber = (
                from infoNode in headInfoNodes
                let itemprop = infoNode.GetAttributeValue("itemprop", null)
                where itemprop == "publicationNumber"
                select infoNode.InnerText).First();

            var countryCode = (
                from infoNode in headInfoNodes
                let itemprop = infoNode.GetAttributeValue("itemprop", null)
                where itemprop == "countryCode"
                select infoNode.InnerText).First();

            var countryName = (
                from infoNode in headInfoNodes
                let itemprop = infoNode.GetAttributeValue("itemprop", null)
                where itemprop == "countryName"
                select infoNode.InnerText).First();

            var pageHtml = new HtmlDocument();
            pageHtml.LoadHtml(webpage.PageSource);
            string? downloadHref;
            try
            {
                downloadHref = (
                    from node in pageHtml.DocumentNode.SelectNodes("/html/head/meta")
                    where node.GetAttributeValue("name", null) == "citation_pdf_url"
                    select node.GetAttributeValue("content", null)).First();
            }
            catch (Exception)
            {
                downloadHref = null;
            }              

            var priorArtKeywords = (
                from infoNode in headInfoNodes
                let itemprop = infoNode.GetAttributeValue("itemprop", null)
                where itemprop == "priorArtKeywords"
                select infoNode.InnerText).ToArray();

            string? legalStatus;
            try
            {
                legalStatus = (
                    from infoNode in headInfoNodes
                    let itemprop = infoNode.GetAttributeValue("itemprop", null)
                    where itemprop == "legalStatusIfi"
                    select infoNode.InnerText.Trim()).First();
            }
            catch (Exception)
            {
                legalStatus = null;
            }

            var applicationNumber = (
                from infoNode in bodyInfoNodes
                let itemprop = infoNode.GetAttributeValue("itemprop", null)
                where itemprop == "applicationNumber"
                select infoNode.InnerText).First();

            var inventors = (
                from infoNode in bodyInfoNodes
                let itemprop = infoNode.GetAttributeValue("itemprop", null)
                where itemprop == "inventor"
                select infoNode.InnerText).ToArray();

            string? currentAssignee;             
            try
            {
                currentAssignee = (
                    from infoNode in bodyInfoNodes
                    let itemprop = infoNode.GetAttributeValue("itemprop", null)
                    where itemprop == "assigneeCurrent"
                    select infoNode.InnerText.Trim()).First();
            }
            catch (Exception)
            {
                currentAssignee = null;
            }

            var originalAssignees = (
                from infoNode in bodyInfoNodes
                let itemprop = infoNode.GetAttributeValue("itemprop", null)
                where itemprop == "assigneeOriginal"
                select infoNode.InnerText).ToArray();

            var eventNodes = (
                from infoNode in bodyInfoNodes
                let itemprop = infoNode.GetAttributeValue("itemprop", null)
                where itemprop == "events"
                select infoNode).ToArray();

            ////
            var applicationEvents = new List<Dictionary<string, string>>();
            foreach (var eventNode in eventNodes)
            {
                var newEvent = new Dictionary<string, string>();
                foreach (var node in eventNode.ChildNodes)
                {
                    var itemprop = node.GetAttributeValue("itemprop", null);

                    if (itemprop != null)
                        newEvent[itemprop] = node.InnerText;
                }
                applicationEvents.Add(newEvent);
            }

            ////3
            var linkXPath = "/ul/li";
            var linkNodes = (
                from node in infoHtml.DocumentNode.SelectNodes(linkXPath)
                let itemprop = node.GetAttributeValue("itemprop", null)
                where itemprop == "links"
                select node).ToArray();

            var externalLinks = new Dictionary<string, string>();
            foreach (var linkNode in linkNodes)
            {
                var newLink = new Dictionary<string, string>();

                var contents = (
                    from node in linkNode.ChildNodes
                    let itemprop = node.GetAttributeValue("itemprop", null)
                    where itemprop == "id"
                    select node.GetAttributeValue("content", null)).ToArray();

                var hrefs = (
                    from node in linkNode.ChildNodes
                    let itemprop = node.GetAttributeValue("itemprop", null)
                    where itemprop == "url"
                    select node.GetAttributeValue("href", null)).ToArray();

                for (int i = 0; i < contents.Length; i++)
                    externalLinks[contents[i]] = hrefs[i];
            }

            return new GeneralInfo(publicationNumber, countryCode, countryName, downloadHref,
                priorArtKeywords, inventors, currentAssignee, originalAssignees, legalStatus,
                applicationNumber, applicationEvents.ToArray(), externalLinks);
        }

        private static Description? ExtractDescription(Webpage webpage)
        {
            ////0
            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            var textHeaders = CheckTextHeaders(webpage);
            if (!textHeaders.Contains("Description")) return null;

            ////1
            var targetText = webpage.PageText
                .Split("Description")[1]
                .Split("Claims")[0];

            var content = targetText;

            ////2
            var targetSource = webpage.PageSource
                .Split("<section itemprop=\"description\" itemscope>")[1]
                .Split("<section itemprop=\"claims\" itemscope>")[0];

            var descriptionHtml = new HtmlDocument();
            descriptionHtml.LoadHtml(targetSource);

            var language = (
                from node in descriptionHtml.DocumentNode.SelectNodes("/div")
                where node.GetAttributeValue("itemprop", null) == "content"
                select node.GetAttributeValue("lang", null)).First();

            return new Description(content, language);
        }

        private static Claims? ExtractClaims(Webpage webpage)
        {
            if (webpage.PageText == null)
                throw new ArgumentNullException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentNullException("webpage.PageSource == null");

            var textHeaders = CheckTextHeaders(webpage);
            if (!textHeaders.Contains("Claims")) return null;

            string? targetText = null;
            for (int i = 0; i < textHeaders.Length; i++)
            {
                if (textHeaders[i].Contains("Patent Citations"))
                {
                    targetText = Regex
                        .Split(webpage.PageText, @"Claims [(][\d]+[)]")[1]
                        .Split("Patent Citations")[0];
                    break;
                }

                if (textHeaders[i].Contains("Patent Citations"))
                {
                    targetText = Regex
                        .Split(webpage.PageText, @"Claims [(][\d]+[)]")[1]
                        .Split("Patent Citations")[0];
                    break;
                }

                if (textHeaders[i].Contains("Patent Citations"))
                {
                    targetText = Regex
                        .Split(webpage.PageText, @"Claims [(][\d]+[)]")[1]
                        .Split("Non-Patent Citations")[0];
                    break;
                }

                if (textHeaders[i].Contains("Cited By"))
                {
                    targetText = Regex
                        .Split(webpage.PageText, @"Claims [(][\d]+[)]")[1]
                        .Split("Cited By")[0];
                    break;
                }

                if (textHeaders[i].Contains("Similar Documents"))
                {
                    targetText = Regex
                        .Split(webpage.PageText, @"Claims [(][\d]+[)]")[1]
                        .Split("Similar Documents")[0];
                    break;
                }                
                
                if (textHeaders[i].Contains("Parent Applications"))
                {
                    targetText = Regex
                        .Split(webpage.PageText, @"Claims [(][\d]+[)]")[1]
                        .Split("Parent Applications")[0];
                    break;
                }                
                
                if (textHeaders[i].Contains("Priority Applications"))
                {
                    targetText = Regex
                        .Split(webpage.PageText, @"Claims [(][\d]+[)]")[1]
                        .Split("Priority Applications")[0];
                    break;
                }                
                
                if (textHeaders[i].Contains("Applications Claiming Priority"))
                {
                    targetText = Regex
                        .Split(webpage.PageText, @"Claims [(][\d]+[)]")[1]
                        .Split("Applications Claiming Priority")[0];
                    break;
                }                
                
                if (textHeaders[i].Contains("Legal Events"))
                {
                    targetText = Regex
                        .Split(webpage.PageText, @"Claims [(][\d]+[)]")[1]
                        .Split("Legal Events")[0];
                    break;
                }

                if (textHeaders[i].Contains("Concepts"))
                {
                    targetText = Regex
                        .Split(webpage.PageText, @"Claims [(][\d]+[)]")[1]
                        .Split("Concepts")[0];
                    break;
                }
            }

            var content = targetText != null ? targetText.Trim() : null;

            ////
            var targetSource = webpage.PageSource
                .Split("<section itemprop=\"claims\" itemscope>")[1]
                .Split("<section itemprop=\"application\" itemscope>")[0];

            var claimsHtml = new HtmlDocument();
            claimsHtml.LoadHtml(targetSource);

            var language = (
                from node in claimsHtml.DocumentNode.SelectNodes("/div")
                where node.GetAttributeValue("itemprop", null) == "content"
                select node.GetAttributeValue("lang", null)).First();

            return new Claims(content, language);
        }

        private static Concepts? ExtractConcepts(Webpage webpage)
        {
            if (webpage.PageText == null)
                throw new ArgumentNullException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentNullException("webpage.PageSource == null");

            var sourceHeaders = CheckSourceHeaders(webpage);
            if (!sourceHeaders.Contains("<h2>Links</h2>")) return null;

            var targetSource = webpage.PageSource
                .Split("<h2>Links</h2>")[1];

            var conceptsHtml = new HtmlDocument();
            conceptsHtml.LoadHtml(targetSource);

            var conceptNodes = (
                from node in conceptsHtml.DocumentNode.SelectNodes("/ul/li")
                where node.GetAttributeValue("itemprop", null) == "match"
                select node).ToArray();

            var concepts = new List<Concepts.Concept>();
            foreach (var conceptNode in conceptNodes)
            {
                var concept = new Concepts.Concept();

                try
                {
                    concept.Id = (
                        from node in conceptNode.ChildNodes
                        where node.GetAttributeValue("itemprop", null) == "id"
                        select node.InnerText).First();
                }
                catch (Exception) { concept.Id = null; }

                try
                {
                    concept.Name = (
                        from node in conceptNode.ChildNodes
                        where node.GetAttributeValue("itemprop", null) == "name"
                        select node.InnerText).First();
                }
                catch (Exception) { concept.Name = null; }

                try
                {
                    concept.Domain = (
                        from node in conceptNode.ChildNodes
                        where node.GetAttributeValue("itemprop", null) == "domain"
                        select node.InnerText).First();
                }
                catch (Exception) { concept.Domain = null; }

                try
                {
                    concept.ImageSmall = (
                        from node in conceptNode.ChildNodes
                        where node.GetAttributeValue("itemprop", null) == "svg_small"
                        select node.InnerText).First();

                    concept.ImageSmall = concept.ImageSmall[0..100] + "...";
                }
                catch (Exception) { concept.ImageSmall = null; }

                try
                {
                    concept.ImageLarge = (
                        from node in conceptNode.ChildNodes
                        where node.GetAttributeValue("itemprop", null) == "svg_large"
                        select node.InnerText).First();

                    concept.ImageLarge = concept.ImageLarge[0..100] + "...";
                }
                catch (Exception) { concept.ImageLarge = null; }

                try
                {
                    concept.Smiles = (
                        from node in conceptNode.ChildNodes
                        where node.GetAttributeValue("itemprop", null) == "smiles"
                        select node.InnerText).First();
                }
                catch (Exception) { concept.ImageLarge = null; }

                try
                {
                    concept.IchiKey = (
                        from node in conceptNode.ChildNodes
                        where node.GetAttributeValue("itemprop", null) == "inchi_key"
                        select node.InnerText).First();
                }
                catch (Exception) { concept.IchiKey = null; }
                
                try
                {
                    concept.Similarity = Convert.ToSingle((
                        from node in conceptNode.ChildNodes
                        where node.GetAttributeValue("itemprop", null) == "similarity"
                        select node.InnerText).First());
                }
                catch (Exception) { concept.Similarity = null; }

                try
                {
                    concept.Sections = (
                        from node in conceptNode.ChildNodes
                        where node.GetAttributeValue("itemprop", null) == "sections"
                        select node.InnerText).ToArray();
                }
                catch (Exception) { concept.Sections = null; }

                try
                {
                    concept.Count = Convert.ToInt32((
                        from node in conceptNode.ChildNodes
                        where node.GetAttributeValue("itemprop", null) == "count"
                        select node.InnerText).First());
                }
                catch (Exception) { concept.Count = null; }

                concepts.Add(concept);
            }

            return new Concepts(concepts.ToArray());
        }

        private static PatentCitations ExtractPatentCitations(Webpage webpage)
        {
            throw new NotFiniteNumberException();
        }

        private static NonPatentCitations ExtractNonPatentCitations(Webpage webpage)
        {
            throw new NotFiniteNumberException();
        }

        private static CitedBy ExtractCitedBy(Webpage webpage)
        {
            throw new NotFiniteNumberException();
        }

        private static SimilarDocuments ExtractSimilarDocuments(Webpage webpage)
        {
            throw new NotFiniteNumberException();
        }        
        
        private static PriorityApplications ExtractPriorityApplications(Webpage webpage)
        {
            throw new NotFiniteNumberException();
        }        
        
        private static LegalEvents ExtractLegalEvents(Webpage webpage)
        {
            throw new NotFiniteNumberException();
        }

        #endregion DATA EXTRACTION
    }
}

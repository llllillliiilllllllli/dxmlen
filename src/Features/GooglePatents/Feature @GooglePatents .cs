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

using DxMLEngine.Attributes;
using DxMLEngine.Functions;

using HtmlDocument = HtmlAgilityPack.HtmlDocument;

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
            /// >>> funct:  2       # prepare data frame to store patent code results
            /// >>> funct:  3       # instantiate new Edge browser with maximized window
            /// >>> funct:  4       # configure each webpage url using search by methods
            /// >>> funct:  5       # copy raw text and source from webpage using clipboard
            /// >>> funct:  6       # determine number of pages by total number of results
            /// >>> funct:  7       # start extracting all patent codes in earch result page
            /// >>> funct:  8       # close web browser after iterating through all webpages
            /// >>> funct:  9       # export founded patent code data saving to csv file
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

            Console.WriteLine("\nSelect search by method: ");
            Console.WriteLine("  1 Search by keyword");
            Console.WriteLine("  2 Search by class code");
            Console.WriteLine("  3 Search by patent code");

            Console.Write("\nSelect: ");
            var selection = Console.ReadLine()?.Trim();

            Console.Write("\nEnter number of pages: ");
            var inputPage = Console.ReadLine()?.Trim();

            ////1            
            var webpages = InputSearchParameters(inFile);

            ////2
            var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
                    new StringDataFrameColumn("Keyword"),
                    new StringDataFrameColumn("Class Code"),
                    new StringDataFrameColumn("Patent Code"),

                    new StringDataFrameColumn("Before"),
                    new StringDataFrameColumn("After"),
                    new StringDataFrameColumn("Inventor"),
                    new StringDataFrameColumn("Assignee"),
                    new StringDataFrameColumn("Country"),
                    new StringDataFrameColumn("Language"),
                    new StringDataFrameColumn("Status"),
                    new StringDataFrameColumn("Type"),
                    new StringDataFrameColumn("Litigation"),

                    new StringDataFrameColumn("Founded Patent Code"),
                }
            );
            
            ////3
            var browser = BrowserAutomation.LaunghEdge();
            if (browser == null)
                throw new Exception("browser == null");

            ////4            
            foreach (var webpage in webpages)
            {
                switch (selection)
                {
                    case "1":
                        webpage.searchBy = SearchBy.Keyword;
                        break;

                    case "2":
                        webpage.searchBy = SearchBy.ClassCode;
                        break;

                    case "3":
                        webpage.searchBy = SearchBy.PatentCode;
                        break;

                    default:
                        webpage.searchBy = SearchBy.Keyword;
                        break;
                }

            ////5            
                var tempTab = BrowserAutomation.OpenNewTab(browser, webpage.SearchUrl);
                webpage.PageText = BrowserAutomation.CopyPageText(tempTab);
                webpage.PageSource = BrowserAutomation.CopyPageSource(tempTab);
                BrowserAutomation.CloseCurrentTab(tempTab);

            ////6
                int? numPages;
                if (!string.IsNullOrEmpty(inputPage)) 
                    numPages = int.Parse(inputPage);
                else
                {
                    numPages = FindNumberOfPages(webpage);
                    if (numPages == null)
                    {
                        var dataRow = new List<KeyValuePair<string, object?>>()
                        {
                            new KeyValuePair<string, object?>("Keyword", webpage.Keyword != null ? webpage.Keyword : null),
                            new KeyValuePair<string, object?>("Class Code", webpage.ClassCode != null ? webpage.ClassCode : null),
                            new KeyValuePair<string, object?>("Patent Code", webpage.PatentCode != null ? webpage.PatentCode : null),
                            new KeyValuePair<string, object?>("Before", webpage.Before != null ? webpage.Before : null),
                            new KeyValuePair<string, object?>("After", webpage.After != null ? webpage.After : null),
                            new KeyValuePair<string, object?>("Inventor", webpage.Inventor != null ? webpage.Inventor : null),
                            new KeyValuePair<string, object?>("Assignee", webpage.Assignee != null ? webpage.Assignee : null),
                            new KeyValuePair<string, object?>("Country", webpage.Country != null ? webpage.Country : null),
                            new KeyValuePair<string, object?>("Language", webpage.Language != null ? webpage.Language : null),
                            new KeyValuePair<string, object?>("Status", webpage.Status != null ? webpage.Status : null),
                            new KeyValuePair<string, object?>("Type", webpage.Type != null ? webpage.Type : null),
                            new KeyValuePair<string, object?>("Litigation", webpage.Litigation != null ? webpage.Litigation : null),
                            new KeyValuePair<string, object?>("Founded Patent Code", null),
                        };

                        dataFrame.Append(dataRow, inPlace: true);
                        continue;
                    }
                }

            ////7
                for (int i = 0; i < numPages; i++)
                {
                    webpage.PageNumber = $"{i}";
                    Console.WriteLine($"Collect: {webpage.SearchUrl}");

                    var newTab = BrowserAutomation.OpenNewTab(browser, webpage.SearchUrl);
                    webpage.PageText = BrowserAutomation.CopyPageText(newTab);
                    webpage.PageSource = BrowserAutomation.CopyPageSource(newTab);

                    OutputSearchPageText(outDir, webpage);
                    OutputSearchPageSource(outDir, webpage);

                    var foundedPatentCodes = ExtractPatentCodes(webpage);

                    foreach (var foundedPatentCode in foundedPatentCodes) 
                    { 
                        var dataRow = new List<KeyValuePair<string, object?>>()
                        {
                            new KeyValuePair<string, object?>("Keyword", webpage.Keyword != null ? webpage.Keyword : null),
                            new KeyValuePair<string, object?>("Class Code", webpage.ClassCode != null ? webpage.ClassCode : null),
                            new KeyValuePair<string, object?>("Patent Code", webpage.PatentCode != null ? webpage.PatentCode : null),
                            new KeyValuePair<string, object?>("Before", webpage.Before != null ? webpage.Before : null),
                            new KeyValuePair<string, object?>("After", webpage.After != null ? webpage.After : null),
                            new KeyValuePair<string, object?>("Inventor", webpage.Inventor != null ? webpage.Inventor : null),
                            new KeyValuePair<string, object?>("Assignee", webpage.Assignee != null ? webpage.Assignee : null),
                            new KeyValuePair<string, object?>("Country", webpage.Country != null ? webpage.Country : null),
                            new KeyValuePair<string, object?>("Language", webpage.Language != null ? webpage.Language : null),
                            new KeyValuePair<string, object?>("Status", webpage.Status != null ? webpage.Status : null),
                            new KeyValuePair<string, object?>("Type", webpage.Type != null ? webpage.Type : null),
                            new KeyValuePair<string, object?>("Litigation", webpage.Litigation != null ? webpage.Litigation : null),
                            new KeyValuePair<string, object?>("Founded Patent Code", foundedPatentCode),
                        };

                        Console.WriteLine(foundedPatentCode);
                        dataFrame.Append(dataRow, inPlace: true);
                    }

                    BrowserAutomation.CloseCurrentTab(newTab);
                }
            }

            BrowserAutomation.CloseBrowser(browser);

            ////8
            OutputPatentCodes(outDir, outFile, dataFrame);
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
                var selections =
                    from option in selection.Split(", ")
                    where options.Contains(option)
                    select option;

                options = selections.ToArray();
            }

            ////2
            var webpages = InputPatentCodes(inFile);

            ////3
            var browser = BrowserAutomation.LaunghEdge();
            if (browser == null)
                throw new Exception("browser == null");

            ////4
            foreach (var webpage in webpages)
            {
                Console.WriteLine($"Collect: {webpage.PatentUrl}");

                var newTab = BrowserAutomation.OpenNewTab(browser, webpage.PatentUrl);
                webpage.PageText = BrowserAutomation.CopyPageText(newTab);
                webpage.PageSource = BrowserAutomation.CopyPageSource(newTab);

                OutputDetailPageText(outDir, webpage);
                OutputDetailPageSource(outDir, webpage);
            
            ////5
                var patent = new Patent();

                patent.Title = ExtractTitle(webpage);
                                
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

                BrowserAutomation.CloseCurrentTab(newTab);

                var jsonString = JsonSerializer.Serialize(patent);

                Console.WriteLine(jsonString);
                OutputPatentDetails(outDir, webpage.PatentCode!, jsonString);
            }
        }

        #region INPUT OUTPUT

        private static Webpage[] InputSearchParameters(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);

            var webpages = new List<Webpage>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var webpage = new Webpage();

                webpage.Keyword = dataFrame["Keyword"][i] != null
                    ? dataFrame["Keyword"][i]!.ToString()!.Replace(" ", "+") : null;
                webpage.ClassCode = dataFrame["Class Code"][i] != null
                    ? dataFrame["Class Code"][i]!.ToString()!.Replace(" ", "+") : null;
                webpage.PatentCode = dataFrame["Patent Code"][i] != null
                    ? dataFrame["Patent Code"][i]!.ToString()!.Replace(" ", "+") : null;

                webpage.Before = dataFrame["Before"][i] != null
                    ? dataFrame["Before"][i]!.ToString()!.Replace(" ", "+") : null;
                webpage.After = dataFrame["After"][i] != null
                    ? dataFrame["After"][i]!.ToString()!.Replace(" ", "+") : null;
                webpage.Inventor = dataFrame["Inventor"][i] != null
                    ? dataFrame["Inventor"][i]!.ToString()!.Replace(" ", "+") : null;
                webpage.Assignee = dataFrame["Assignee"][i] != null
                    ? dataFrame["Assignee"][i]!.ToString()!.Replace(" ", "+") : null;

                webpage.Country = dataFrame["Country"][i] != null
                    ? dataFrame["Country"][i]!.ToString()!.Replace(" ", "+") : null;
                webpage.Language = dataFrame["Language"][i] != null
                    ? dataFrame["Language"][i]!.ToString()!.Replace(" ", "+") : null;
                webpage.Status = dataFrame["Status"][i] != null
                    ? dataFrame["Status"][i]!.ToString()!.Replace(" ", "+") : null;
                webpage.Type = dataFrame["Type"][i] != null
                    ? dataFrame["Type"][i]!.ToString()!.Replace(" ", "+") : null;
                webpage.Litigation = dataFrame["Litigation"][i] != null
                    ? dataFrame["Litigation"][i]!.ToString()!.Replace(" ", "+") : null;

                webpages.Add(webpage);
            }

            return webpages.ToArray();
        }

        private static Webpage[] InputPatentCodes(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);

            var webpages = new List<Webpage>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var webpage = new Webpage();

                webpage.PatentCode = dataFrame["Patent Code"][i] != null
                    ? dataFrame["Patent Code"][i]!.ToString()!.Replace(" ", "+") : null;

                webpages.Add(webpage);
            }

            return webpages.ToArray();
        }

        private static void OutputSearchPageText(string location, Webpage webpage)
        {
            var path = $"{location}\\Webpage @SearchPage #-------------- .txt";
            File.WriteAllText(path, webpage.PageText, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputSearchPageSource(string location, Webpage webpage)
        {
            var path = $"{location}\\Webpage @SearchPage #-------------- .html";
            File.WriteAllText(path, webpage.PageSource, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputDetailPageText(string location, Webpage webpage)
        {
            var path = $"{location}\\Webpage @DetailPage #-------------- .txt";
            File.WriteAllText(path, webpage.PageText, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputDetailPageSource(string location, Webpage webpage)
        {
            var path = $"{location}\\Webpage @DetailPage #-------------- .html";
            File.WriteAllText(path, webpage.PageSource, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputPatentCodes(string location, string fileName, DataFrame dataFrame)
        {
            var path = $"{location}\\Dataset @{fileName} #-------------- .csv";
            DataFrame.WriteCsv(dataFrame, path, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputPatentDetails(string location, string fileName, string jsonString)
        {
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
        
        #endregion PAGE CHECKING
                
        #region DATA EXTRACTION

        private static string[] ExtractPatentCodes(Webpage webpage)
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
                select codeMatch.Value;

            return foundedPatentCodes.ToArray();
        }

        private static string ExtractTitle(Webpage webpage)
        {
            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");            
            
            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            ////
            var targetText = webpage.PageText
                .Split("Abstract")[0];

            var slittedLines = (
                from line in targetText.Split("\n", StringSplitOptions.TrimEntries)
                where string.IsNullOrEmpty(line) == false
                select line).ToArray();

            var title = slittedLines.Last();

            ////
            var targetSource = webpage.PageSource
                .Split("<body unresolved>")[0];

            var document = new HtmlDocument();
            document.LoadHtml(targetSource);

            var metaXPath = "/html/head/meta";
            var metaNodes = document.DocumentNode.SelectNodes(metaXPath);

            var altTitle = (
                from metaNode in metaNodes
                let name = metaNode.GetAttributeValue("name", null)
                where name == "DC.title"
                let content = metaNode.GetAttributeValue("content", null)
                select content).First();

            Console.WriteLine(title);
            return title;
        }

        private static Abstract ExtractAbstract(Webpage webpage)
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

            ////0
            var targetText = webpage.PageText
                .Split("Abstract")[1]
                .Split("Images")[0];

            var abstractText = targetText;

            var targetSource = webpage.PageSource
                .Split("<h2>Abstract</h2>")[1]
                .Split("<h2>Description</h2>")[0];

            var abstractHtml = new HtmlDocument();
            abstractHtml.LoadHtml(targetSource);

            var languageXPath = "/div/abstract";
            var languageNode = abstractHtml.DocumentNode.SelectSingleNode(languageXPath);
            var language = languageNode.GetAttributeValue("lang", null);

            var contentXPath = "/div/abstract/div";
            var contentNode = abstractHtml.DocumentNode.SelectSingleNode(contentXPath);
            var content = contentNode.InnerText;

            if (!string.IsNullOrEmpty(abstractText))
                Console.WriteLine(abstractText);

            return new Abstract(content, language);
        }

        private static Images ExtractImages(Webpage webpage)
        {
            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

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

        private static Classifications ExtractClassifications(Webpage webpage)
        {
            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null"); 

            var targetSource = webpage.PageSource
                .Split("<h2>Classifications</h2>")[1]
                .Split("<h2>Abstract</h2>")[0];

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

        private static GeneralInfo ExtractGeneralInfo(Webpage webpage)
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

            if (webpage.PageText == null)
                throw new ArgumentException("webpage.PageText == null");

            if (webpage.PageSource == null)
                throw new ArgumentException("webpage.PageSource == null");

            ////0
            var document = new HtmlDocument();
            document.LoadHtml(webpage.PageSource);

            ////1
            var targetText = webpage.PageText
                .Split("classifications")[1]
                .Split("Description")[0];

            var infoLines = (
                from line in targetText.Split("\n", StringSplitOptions.TrimEntries)
                where string.IsNullOrEmpty(line) == false
                select line).ToArray();

            var patentCode = infoLines[0];

            ////2
            var targetSource = webpage.PageSource
                .Split("<h2>Info</h2>")[1]
                .Split("<h2>Images</h2>")[0];

            var infoHtml = new HtmlDocument();
            infoHtml.LoadHtml(targetSource);

            var headInfoXPath = "/dl/dd";
            var bodyInfoXPath = "/dd";
            var headInfoNodes = infoHtml.DocumentNode.SelectNodes(headInfoXPath);
            var bodyInfoNodes = infoHtml.DocumentNode.SelectNodes(bodyInfoXPath);

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

            var downloadXPath = "/html/head/meta[10]";
            var downloadNode = document.DocumentNode.SelectSingleNode(downloadXPath);
            var downloadHref = downloadNode.GetAttributeValue("content", null);

            var priorArtKeywords = (
                from infoNode in headInfoNodes
                let itemprop = infoNode.GetAttributeValue("itemprop", null)
                where itemprop == "priorArtKeywords"
                select infoNode.InnerText).ToArray();

            var legalStatus = (
                from infoNode in headInfoNodes
                let itemprop = infoNode.GetAttributeValue("itemprop", null)
                where itemprop == "legalStatusIfi"
                select infoNode.InnerText.Trim()).First();

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

            var currentAssignee = (
                from infoNode in bodyInfoNodes
                let itemprop = infoNode.GetAttributeValue("itemprop", null)
                where itemprop == "assigneeCurrent"
                select infoNode.InnerText.Trim()).First();

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

        private static Description ExtractDescription(Webpage webpage)
        {
            throw new NotFiniteNumberException();
        }

        private static Claims ExtractClaims(Webpage webpage)
        {
            throw new NotFiniteNumberException();
        }

        private static Concepts ExtractConcepts(Webpage webpage)
        {
            throw new NotFiniteNumberException();
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

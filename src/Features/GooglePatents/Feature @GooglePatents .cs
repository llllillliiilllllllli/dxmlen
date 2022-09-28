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
        internal const string URL_SEARCH_KEYWORD = "https://patents.google.com/?q={keyword}";
        internal const string URL_SEARCH_CLASS_CODE = "https://patents.google.com/?q={classCode}";
        internal const string URL_SEARCH_PATENT_CODE = "https://patents.google.com/?q={patendCode}";
        internal const string URL_SEARCH_SIMILAR = "https://patents.google.com/?q=~patent%2f{patendCode}";
        internal const string URL_PATENT_PAGE = "https://patents.google.com/patent/{patentCode}";

        public static void SearchPatents()
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
            /// >>> funct:  4       # loop through each patent search url with page zero 
            /// >>> funct:  5       # copy all raw text from webpage using clipboard
            /// >>> funct:  6       # get total number of results to determine page number
            /// >>> funct:  7       # close current tab after having number for pagination 
            /// >>> funct:  8       # start collecting all patent codes in earch result page
            /// >>> funct:  9       # append founded patent codes into the output data frame
            /// >>> funct:  0       # close web browser after iterating through all webpages
            /// >>> funct:  1       # export founded patent code data saving to csv file
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

            Console.WriteLine("\nSelect search method: ");
            Console.WriteLine("1 Search by keyword");
            Console.WriteLine("2 Search by class code");
            Console.WriteLine("3 Search by patent code");

            Console.Write("\nSelection: ");
            var selection = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(selection))
                throw new ArgumentNullException("selection is null or empty");

            SearchBy searchBy;
            switch (selection)
            {
                case "1":
                    searchBy = SearchBy.Keyword;
                    break;                
                
                case "2":
                    searchBy = SearchBy.ClassCode;
                    break;                
                
                case "3":
                    searchBy = SearchBy.PatentCode;
                    break;    
                    
                default:
                    searchBy = SearchBy.Keyword;
                    break;
            }

            ////1            
            var dataFrame = DataFrame.LoadCsv(i_fil, header: true, encoding: Encoding.UTF8);

            var searchUrls = new List<Webpage>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var searchUrl = new Webpage();

                var keyword = dataFrame["Keyword"][i].ToString();
                var classCode = dataFrame["Class Code"][i].ToString();
                var patentCode = dataFrame["Patent Code"][i].ToString();

                searchUrl.Keyword = dataFrame["Keyword"][i] == null 
                    ? null : dataFrame["Keyword"][i]!.ToString()!.Replace(" ", "+");
                searchUrl.ClassCode = dataFrame["Class Code"][i] == null 
                    ? null : dataFrame["Class Code"][i]!.ToString()!.Replace(" ", "+");
                searchUrl.PatentCode = dataFrame["Patent Code"][i] == null 
                    ? null : dataFrame["Patent Code"][i]!.ToString()!.Replace(" ", "+");

                searchUrl.Before = dataFrame["Before"][i] == null 
                    ? null : dataFrame["Before"][i]!.ToString()!.Replace(" ", "+");
                searchUrl.After = dataFrame["After"][i] == null 
                    ? null : dataFrame["After"][i]!.ToString()!.Replace(" ", "+");
                searchUrl.Inventor = dataFrame["Inventor"][i] == null 
                    ? null : dataFrame["Inventor"][i]!.ToString()!.Replace(" ", "+");
                searchUrl.Assignee = dataFrame["Assignee"][i] == null 
                    ? null : dataFrame["Assignee"][i]!.ToString()!.Replace(" ", "+");

                searchUrl.Country = dataFrame["Country"][i] == null 
                    ? null : dataFrame["Country"][i]!.ToString()!.Replace(" ", "+");
                searchUrl.Language = dataFrame["Language"][i] == null 
                    ? null : dataFrame["Language"][i]!.ToString()!.Replace(" ", "+");
                searchUrl.Status = dataFrame["Status"][i] == null 
                    ? null : dataFrame["Status"][i]!.ToString()!.Replace(" ", "+");
                searchUrl.Type = dataFrame["Type"][i] == null 
                    ? null : dataFrame["Type"][i]!.ToString()!.Replace(" ", "+");
                searchUrl.Litigation = dataFrame["Litigation"][i] == null 
                    ? null : dataFrame["Litigation"][i]!.ToString()!.Replace(" ", "+");

                searchUrls.Add(searchUrl);                
            }

            ////2
            var foundedDataFrame = new DataFrame(new List<DataFrameColumn>()
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
            var browser = BrowserAutomation.LaunghEdge(windowStyle: ProcessWindowStyle.Maximized);

            ////4            
            foreach (var searchUrl in searchUrls)
            {
                var url = searchUrl.ConfigureWebpage(searchBy);
                var process = BrowserAutomation.OpenNewTab(browser!, url.Replace("{page}", "0"));

            ////5
                var text = BrowserAutomation.CopyPageText(process);
            
            ////6
                int numResults;

                var regex = new Regex(@"(About) [\d,]+ (results)");
                var match = regex.Match(text!);

                if (match.Success)
                    numResults = int.Parse(match.Value.Split(" ")[1].Replace(",", ""));
                else 
                {
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("Keyword", searchUrl.Keyword != null ? searchUrl.Keyword : null),
                        new KeyValuePair<string, object?>("Class Code", searchUrl.ClassCode != null ? searchUrl.ClassCode : null),
                        new KeyValuePair<string, object?>("Patent Code", searchUrl.PatentCode != null ? searchUrl.PatentCode : null),
                        new KeyValuePair<string, object?>("Before", searchUrl.Before != null ? searchUrl.Before : null),
                        new KeyValuePair<string, object?>("After", searchUrl.After != null ? searchUrl.After : null),
                        new KeyValuePair<string, object?>("Inventor", searchUrl.Inventor != null ? searchUrl.Inventor : null),
                        new KeyValuePair<string, object?>("Assignee", searchUrl.Assignee != null ? searchUrl.Assignee : null),
                        new KeyValuePair<string, object?>("Country", searchUrl.Country != null ? searchUrl.Country : null),
                        new KeyValuePair<string, object?>("Language", searchUrl.Language != null ? searchUrl.Language : null),
                        new KeyValuePair<string, object?>("Status", searchUrl.Status != null ? searchUrl.Status : null),
                        new KeyValuePair<string, object?>("Type", searchUrl.Type != null ? searchUrl.Type : null),
                        new KeyValuePair<string, object?>("Litigation", searchUrl.Litigation != null ? searchUrl.Litigation : null),
                        new KeyValuePair<string, object?>("Founded Patent Code", null),
                    };

                    foundedDataFrame.Append(dataRow, inPlace: true);
                    BrowserAutomation.CloseCurrentTab(process);
                    continue; 
                }                

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

            ////7
                BrowserAutomation.CloseCurrentTab(process);

            ////8
                var foundedPatentCodes = new List<string>();
                for (int page = 0; page < 1; page++)
                {
                    Console.WriteLine($"Download: {url.Replace("{page}", page.ToString())}");

                    process = BrowserAutomation.OpenNewTab(browser!, url.Replace("{page}", page.ToString()));

                    var pageText = BrowserAutomation.CopyPageText(process);

                    var foundedCodes =
                        from field in typeof(PatentCodePatterns).GetFields()
                        let patentCodePattern = (string?)field.GetValue(null)
                        let codeRegex = new Regex(patentCodePattern)
                        let codeMatches = codeRegex.Matches(pageText!)
                        from codeMatch in codeMatches
                        where codeMatch.Success
                        select codeMatch.Value;

                    foundedPatentCodes.AddRange(foundedCodes);

                    BrowserAutomation.CloseCurrentTab(process);
                }

            ////9
                foreach (var foundedPatentCode in foundedPatentCodes) 
                { 
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("Keyword", searchUrl.Keyword != null ? searchUrl.Keyword : null),
                        new KeyValuePair<string, object?>("Class Code", searchUrl.ClassCode != null ? searchUrl.ClassCode : null),
                        new KeyValuePair<string, object?>("Patent Code", searchUrl.PatentCode != null ? searchUrl.PatentCode : null),
                        new KeyValuePair<string, object?>("Before", searchUrl.Before != null ? searchUrl.Before : null),
                        new KeyValuePair<string, object?>("After", searchUrl.After != null ? searchUrl.After : null),
                        new KeyValuePair<string, object?>("Inventor", searchUrl.Inventor != null ? searchUrl.Inventor : null),
                        new KeyValuePair<string, object?>("Assignee", searchUrl.Assignee != null ? searchUrl.Assignee : null),
                        new KeyValuePair<string, object?>("Country", searchUrl.Country != null ? searchUrl.Country : null),
                        new KeyValuePair<string, object?>("Language", searchUrl.Language != null ? searchUrl.Language : null),
                        new KeyValuePair<string, object?>("Status", searchUrl.Status != null ? searchUrl.Status : null),
                        new KeyValuePair<string, object?>("Type", searchUrl.Type != null ? searchUrl.Type : null),
                        new KeyValuePair<string, object?>("Litigation", searchUrl.Litigation != null ? searchUrl.Litigation : null),
                        new KeyValuePair<string, object?>("Founded Patent Code", foundedPatentCode),
                    };

                    foundedDataFrame.Append(dataRow, inPlace: true);
                }
            }

            ////0
            BrowserAutomation.CloseBrowser(browser!);                 

            ////1
            o_fil = $"{o_fol}\\Dataset @{o_fil}Patents #-------------- .csv";
            DataFrame.WriteCsv(foundedDataFrame, o_fil, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(o_fil).ToString("yyyyMMddHHmmss");
            File.Move(o_fil, o_fil.Replace("#--------------", $"#{timestamp}"));
        }
    
        public static void CollectPatents()
        {
            /// ====================================================================================
            /// Collect patents from Google Patents using codes registed througth patent offices   
            /// 
            /// >>> param:  string  # path to input file containing list of patent codes 
            /// >>> param:  string  # path to output folder for storing collected data 
            /// >>> param:  string  # name of file for saving unprocessed patent text data 
            ///             
            /// >>> funct:  0       # ...
            /// >>> funct:  1       # ...
            /// >>> funct:  2       # ...
            /// >>> funct:  3       # ...
            /// >>> funct:  4       # ...
            /// >>> funct:  5       # ...
            /// >>> funct:  6       # ...
            /// >>> funct:  7       # ...
            /// >>> funct:  8       # ...
            /// >>> funct:  9       # ...
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

            ////1
            var options = new string[14]
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
                "LegalEvents",

                "All",
            };

            Console.WriteLine("\nSelect from options: ");
            for (int i = 0; i < options.Length; i++)
                Console.WriteLine($"{i+1, 2} {options[i]}");

            Console.Write("\nSelect: ");
            var selection = Console.ReadLine();

            if (string.IsNullOrEmpty(selection))
                throw new ArgumentNullException("selection is null or empty");

            if (selection != "All")
            {
                var selections =
                    from option in selection.Split(", ")
                    where options.Contains(option)
                    select option;

                options = selections.ToArray();
            }

            ////2
            var dataFrame = DataFrame.LoadCsv(i_fil, header: true, encoding: Encoding.UTF8);
            var patentCodes = new List<string>();
            foreach (var patentCode in dataFrame["Founded Patent Code"])
                if (patentCode != null)
                    patentCodes.Add(patentCode.ToString()!);

            ////3
            var browser = BrowserAutomation.LaunghEdge(windowStyle: ProcessWindowStyle.Maximized);
            Thread.Sleep(100);

            foreach (var patentCode in patentCodes)
            {
                var patentUrl = URL_PATENT_PAGE.Replace("{patentCode}", patentCode);
                var process = BrowserAutomation.OpenNewTab(browser!, patentUrl);

            ////5
                var pageText = BrowserAutomation.CopyPageText(process);
                var pageSource = BrowserAutomation.CopyPageSource(process);

            ////6
                var path = $"{o_fol}\\Datadoc @{patentCode}Patents #-------------- .txt";
                File.WriteAllText(path, pageText, encoding: Encoding.UTF8);

                var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
                File.Move(path, path.Replace("#--------------", $"#{timestamp}"));

            ////7
                var patent = new Patent();

                patent.Title = ExtractTitle(pageText, pageSource);
                                
                if (options.Contains("Abstract")) 
                    patent.Abstract = ExtractAbstract(pageText, pageSource);

                if (options.Contains("Images")) 
                    patent.Images = ExtractImages(pageText, pageSource);

                if (options.Contains("Classifications"))
                    patent.Classifications = ExtractClassifications(pageText, pageSource);

                if (options.Contains("GeneralInfo"))
                    patent.GeneralInfo = ExtractGeneralInfo(pageText, pageSource);

                if (options.Contains("Description"))
                    patent.Description = ExtractDescription(pageText, pageSource);

                if (options.Contains("Claims"))
                    patent.Claims = ExtractClaims(pageText, pageSource);

                if (options.Contains("Concepts"))
                    patent.Concepts = ExtractConcepts(pageText, pageSource);

                if (options.Contains("PatentCitations"))
                    patent.PatentCitations = ExtractPatentCitations(pageText, pageSource);

                if (options.Contains("NonPatentCitations"))
                    patent.NonPatentCitations = ExtractNonPatentCitations(pageText, pageSource);

                if (options.Contains("CitedBy"))
                    patent.CitedBy = ExtractCitedBy(pageText, pageSource);

                if (options.Contains("SimilarDocuments"))
                    patent.SimilarDocuments = ExtractSimilarDocuments(pageText, pageSource);

                if (options.Contains("PriorityApplications"))
                    patent.PriorityApplications = ExtractPriorityApplications(pageText, pageSource);

                if (options.Contains("LegalEvents"))
                    patent.LegalEvents = ExtractLegalEvents(pageText, pageSource);

                DxKeyboard.SendKeys(process, "CTRL+W", 100);

                Console.WriteLine(JsonSerializer.Serialize(patent));
            }
        }

        private static string ExtractTitle(string pageText, string pageSource)
        {
            ////
            var targetText = pageText
                .Split("Abstract")[0];

            var slittedLines = (
                from line in targetText.Split("\n", StringSplitOptions.TrimEntries)
                where string.IsNullOrEmpty(line) == false
                select line).ToArray();

            var title = slittedLines.Last();

            ////
            var targetSource = pageSource
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

        private static Abstract ExtractAbstract(string pageText, string pageSource)
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

            ////0
            var targetText = pageText
                .Split("Abstract")[1]
                .Split("Images")[0];

            var abstractText = targetText;

            var targetSource = pageSource
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

        private static Images ExtractImages(string pageText, string pageSource)
        {
            var targetSource = pageSource
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

        private static Classifications ExtractClassifications(string pageText, string pageSource)
        {
            var targetSource = pageSource
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

        private static GeneralInfo ExtractGeneralInfo(string pageText, string pageSource)
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
            var document = new HtmlDocument();
            document.LoadHtml(pageSource);

            ////1
            var targetText = pageText
                .Split("classifications")[1]
                .Split("Description")[0];

            var infoLines = (
                from line in targetText.Split("\n", StringSplitOptions.TrimEntries)
                where string.IsNullOrEmpty(line) == false
                select line).ToArray();

            var patentCode = infoLines[0];

            ////2
            var targetSource = pageSource
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

        private static Description ExtractDescription(string pageText, string pageSource)
        {
            throw new NotFiniteNumberException();
        }

        private static Claims ExtractClaims(string pageText, string pageSource)
        {
            throw new NotFiniteNumberException();
        }

        private static Concepts ExtractConcepts(string pageText, string pageSource)
        {
            throw new NotFiniteNumberException();
        }

        private static PatentCitations ExtractPatentCitations(string pageText, string pageSource)
        {
            throw new NotFiniteNumberException();
        }

        private static NonPatentCitations ExtractNonPatentCitations(string pageText, string pageSource)
        {
            throw new NotFiniteNumberException();
        }

        private static CitedBy ExtractCitedBy(string pageText, string pageSource)
        {
            throw new NotFiniteNumberException();
        }

        private static SimilarDocuments ExtractSimilarDocuments(string pageText, string pageSource)
        {
            throw new NotFiniteNumberException();
        }        
        
        private static PriorityApplications ExtractPriorityApplications(string pageText, string pageSource)
        {
            throw new NotFiniteNumberException();
        }        
        
        private static LegalEvents ExtractLegalEvents(string pageText, string pageSource)
        {
            throw new NotFiniteNumberException();
        }
    }
}

using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Text.RegularExpressions;
using Microsoft.Data.Analysis;

using DxMLEngine.Attributes;
using DxMLEngine.Functions;

using Clipboard = DxMLEngine.Functions.Clipboard;

namespace DxMLEngine.Features.GooglePatents
{
    [Feature]
    internal class GooglePatents
    {
        internal const string URL_SEARCH_KEYWORD = "https://patents.google.com/?q={keyword}";
        internal const string URL_SEARCH_CLASS_CODE = "https://patents.google.com/?q={classCode}";
        internal const string URL_SEARCH_PATENT_CODE = "https://patents.google.com/?q={patendCode}";
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
            
            Console.Write("\nEnter output file name: ");
            var o_fil = Console.ReadLine()?.Replace(" ", "");

            if (string.IsNullOrEmpty(o_fil))
                throw new ArgumentNullException("path is null or empty");            

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

            ////2
            var searchUrls = new List<SearchUrl>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var searchUrl = new SearchUrl();

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

            ////3
            var browser = Process.Start("MicrosoftEdge.exe", "edge://version/");
            Thread.Sleep(100);

            ////4
            var dataColumns = new List<DataFrameColumn>()
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
            };

            dataFrame = new DataFrame(dataColumns);

            ////5            
            foreach (var searchUrl in searchUrls)
            {
                var url = searchUrl.ConfigureSearchUrl(searchBy);

            ////6
                var process = Process.Start("MicrosoftEdge.exe", url.Replace("{page}", "0"));

                Thread.Sleep(5000);
                Keyboard.SendKeys(process, "CTRL+A", 100);
                Keyboard.SendKeys(process, "CTRL+C", 100);

                var text = Clipboard.GetText();
                var pattern = @"(About) [\d,]+ (results)";
                var regex = new Regex(pattern);
                var match = regex.Match(text!);

            ////7
                int numResults;
                if (match.Success)
                    numResults = int.Parse(match.Value.Split(" ")[1].Replace(",", ""));
                else 
                {
                    var dataRow = new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("Keyword", searchUrl.Keyword != null ? searchUrl.Keyword : "Null"),
                        new KeyValuePair<string, object>("Class Code", searchUrl.ClassCode != null ? searchUrl.ClassCode : "Null"),
                        new KeyValuePair<string, object>("Patent Code", searchUrl.PatentCode != null ? searchUrl.PatentCode : "Null"),
                        new KeyValuePair<string, object>("Before", searchUrl.Before != null ? searchUrl.Before : "Null"),
                        new KeyValuePair<string, object>("After", searchUrl.After != null ? searchUrl.After : "Null"),
                        new KeyValuePair<string, object>("Inventor", searchUrl.Inventor != null ? searchUrl.Inventor : "Null"),
                        new KeyValuePair<string, object>("Assignee", searchUrl.Assignee != null ? searchUrl.Assignee : "Null"),
                        new KeyValuePair<string, object>("Country", searchUrl.Country != null ? searchUrl.Country : "Null"),
                        new KeyValuePair<string, object>("Language", searchUrl.Language != null ? searchUrl.Language : "Null"),
                        new KeyValuePair<string, object>("Status", searchUrl.Status != null ? searchUrl.Status : "Null"),
                        new KeyValuePair<string, object>("Type", searchUrl.Type != null ? searchUrl.Type : "Null"),
                        new KeyValuePair<string, object>("Litigation", searchUrl.Litigation != null ? searchUrl.Litigation : "Null"),
                        new KeyValuePair<string, object>("Founded Patent Code", "Null"),
                    };

                    dataFrame.Append(dataRow, inPlace: true);

                    Keyboard.SendKeys(process, "CTRL+W", 100); 
                    continue; 
                }

            ////8
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

                Keyboard.SendKeys(process, "CTRL+W", 100);

            ////9
                var foundedPatentCodes = new List<string>();
                for (int page = 0; page < 1; page++)
                {
                    Console.WriteLine($"Download: {url.Replace("{page}", page.ToString())}");
                    process = Process.Start("MicrosoftEdge.exe", url.Replace("{page}", page.ToString()));

                    Thread.Sleep(5000);
                    Keyboard.SendKeys(process, "CTRL+A", 100);
                    Keyboard.SendKeys(process, "CTRL+C", 100);

                    var pageText = Clipboard.GetText();

                    var patentCodePatterns =
                        from field in typeof(PatentCodePatterns).GetFields()
                        select (string?)field.GetValue(null);

                    foreach (var patentCodePattern in patentCodePatterns.ToArray())
                    {
                        var codeRegex = new Regex(patentCodePattern);
                        var codeMatches = codeRegex.Matches(pageText!);
                        foreach (var code in codeMatches.ToArray())
                            foundedPatentCodes.Add(code.Value);
                    }

                    Keyboard.SendKeys(process, "CTRL+W", 100);
                }

            ////0
                foreach (var foundedPatentCode in foundedPatentCodes) 
                { 
                    var dataRow = new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("Keyword", searchUrl.Keyword != null ? searchUrl.Keyword : "Null"),
                        new KeyValuePair<string, object>("Class Code", searchUrl.ClassCode != null ? searchUrl.ClassCode : "Null"),
                        new KeyValuePair<string, object>("Patent Code", searchUrl.PatentCode != null ? searchUrl.PatentCode : "Null"),
                        new KeyValuePair<string, object>("Before", searchUrl.Before != null ? searchUrl.Before : "Null"),
                        new KeyValuePair<string, object>("After", searchUrl.After != null ? searchUrl.After : "Null"),
                        new KeyValuePair<string, object>("Inventor", searchUrl.Inventor != null ? searchUrl.Inventor : "Null"),
                        new KeyValuePair<string, object>("Assignee", searchUrl.Assignee != null ? searchUrl.Assignee : "Null"),
                        new KeyValuePair<string, object>("Country", searchUrl.Country != null ? searchUrl.Country : "Null"),
                        new KeyValuePair<string, object>("Language", searchUrl.Language != null ? searchUrl.Language : "Null"),
                        new KeyValuePair<string, object>("Status", searchUrl.Status != null ? searchUrl.Status : "Null"),
                        new KeyValuePair<string, object>("Type", searchUrl.Type != null ? searchUrl.Type : "Null"),
                        new KeyValuePair<string, object>("Litigation", searchUrl.Litigation != null ? searchUrl.Litigation : "Null"),
                        new KeyValuePair<string, object>("Founded Patent Code", foundedPatentCode),
                    };

                    dataFrame.Append(dataRow, inPlace: true);
                }
            }

            ////1
            Keyboard.SendKeys(browser, "ALT+F4", 100);

            ////2
            o_fil = $"{o_fol}\\Dataset @{o_fil}Patents #-------------- .csv";
            DataFrame.WriteCsv(dataFrame, o_fil, encoding: Encoding.UTF8);

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
            var options = new string[15]
            {
                "Summary",
                "InfoCard",
                "Description",
                "Claims",
                "Citations",
                "CitedBy",
                "SimilarDocument",

                "PriorityAndRelatedApplications",
                "ParentApplications",
                "ChildApplications",
                "PriorityApplications",
                "ApplicationsClaimingPriority",
                "LegalEvents",
                "Concepts",

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
            var dataValues = dataFrame["Founded Patent Code"];

            var patentCodes = new List<string>();
            foreach (var value in dataValues)
                if (value != null)
                    patentCodes.Add(value.ToString()!);

            ////3
            var browser = Process.Start("MicrosoftEdge.exe", "edge://version/");
            Thread.Sleep(100);

            foreach (var patentCode in patentCodes)
            {
                var patentUrl = URL_PATENT_PAGE.Replace("{patentCode}", patentCode);
                var process = Process.Start("MicrosoftEdge.exe", patentUrl);

                Thread.Sleep(5000);

            ////4
                ClickExpandClassification();

            ////5
                Keyboard.SendKeys(process, "CTRL+A", 100);
                Keyboard.SendKeys(process, "CTRL+C", 100);

                var text = Clipboard.GetText();
                if (text == null) continue;

                var path = $"{o_fol}\\Datadoc @{patentCode}Patents #-------------- .txt";
                File.WriteAllText(path, text, encoding: Encoding.UTF8);

                var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
                File.Move(path, path.Replace("#--------------", $"#{timestamp}"));                
                
                if (options.Contains("Summary"))
                {                    
                    var patentSummary = ExtractSummary(text);
                    Console.WriteLine(patentSummary.AbstractText);
                    Console.WriteLine(patentSummary.NumberOfImage);
                    foreach (var key in patentSummary.Classifications.Keys)
                    {
                        Console.WriteLine(key);
                        Console.WriteLine(patentSummary.Classifications[key]);
                        Console.WriteLine();
                    }
                }
                
                if (options.Contains("InfoCard"))
                    CollectInfoCard(o_fol, text);                
                
                if (options.Contains("Description"))
                    CollectDescription(o_fol, text);                
                
                if (options.Contains("Claims"))
                    CollectClaims(o_fol, text);              
                
                if (options.Contains("Citations"))
                    CollectCitations(o_fol, text);                
                
                if (options.Contains("CitedBy"))
                    CollectCitedBy(o_fol, text);                
                
                if (options.Contains("SimilarDocuments"))
                    CollectSimilarDocuments(o_fol, text);              
                
                if (options.Contains("PriorityAndRelatedApplications"))
                    CollectPriorityAndRelatedApplications(o_fol, text);

                if (options.Contains("ParentApplications"))
                    CollectParentApplications(o_fol, text);

                if (options.Contains("ChildApplications"))
                    CollectChildApplications(o_fol, text);

                if (options.Contains("CollectPriorityApplications"))
                    CollectPriorityApplications(o_fol, text);                
                
                if (options.Contains("ApplicationsClaimingPriority"))
                    CollectApplicationsClaimingPriority(o_fol, text);

                if (options.Contains("LegalEvents"))
                    CollectLegalEvents(o_fol, text);

                if (options.Contains("Concepts"))
                    CollectConcepts(o_fol, text);

                Keyboard.SendKeys(process, "CTRL+W", 100);
            }
        }
    
        private static PatentSummary ExtractSummary(string text)
        {
            ////0
            var foundedPatentCodes =
                from field in typeof(PatentCodePatterns).GetFields()
                let patentCodePattern = (string?)field.GetValue(null)
                let codeRegex = new Regex(patentCodePattern)
                let codeMatch = codeRegex.Match(text)
                select codeMatch.Value;

            var patentCode = foundedPatentCodes.First();

            ////1
            var targetText = text
                .Split(patentCode)[0];

            var abstractText = targetText
                .Split("Abstract")[1]
                .Split("Image")[0];

            var imageText = targetText
                .Split("Abstract")[1]
                .Split("Classifications")[0];            
            
            var imageRegex = new Regex(@"Images [(][\d]+[)]");
            var imageMatch = imageRegex.Match(imageText);
            var numImage = imageMatch.Value
                .Replace("Images ", "")
                .Replace("(", "").Replace(")", "");

            var classificationText = targetText
                .Split("Classifications")[1]
                .Split("Hide more classifications")[0];
            var classificationLines = classificationText.Split("\n", StringSplitOptions.TrimEntries);

            var classifications = new Dictionary<string, string>();  
            foreach (var line in classificationLines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                var classCode = line.Split(" ")[0];
                var classDescription = line.Replace(classCode, "").Trim();
                classifications[classCode] = classDescription;
            }            

            return new PatentSummary(abstractText, int.Parse(numImage), classifications);
        }

        private static void CollectInfoCard(string path, string text)
        {
            ////0
            var patentCodePatterns =
                from field in typeof(PatentCodePatterns).GetFields()
                select (string?)field.GetValue(null);            
           
            var foundedPatentCodes = new List<string>();
            foreach (var patentCodePattern in patentCodePatterns.ToArray())
            {
                var codeRegex = new Regex(patentCodePattern);
                var codeMatches = codeRegex.Matches(text);
                foreach (var code in codeMatches.ToArray())
                    foundedPatentCodes.Add(code.Value);
            }

            var patentCode = foundedPatentCodes.First();

            ////1
            var targetText = text
                .Split(patentCode)[1]
                .Split("Description")[0];

            ////2
            ////3
            ////4
            ////5
        }

        private static void CollectDescription(string path, string text)
        {

        }

        private static void CollectClaims(string path, string text)
        {

        }

        private static void CollectCitations(string path, string text)
        {

        }

        private static void CollectCitedBy(string path, string text)
        {

        }

        private static void CollectSimilarDocuments(string path, string text)
        {

        }

        private static void CollectPriorityAndRelatedApplications(string path, string text)
        {

        }

        private static void CollectParentApplications(string path, string text)
        {

        }        
        
        private static void CollectChildApplications(string path, string text)
        {

        }        
        
        private static void CollectPriorityApplications(string path, string text)
        {

        }        
        
        private static void CollectApplicationsClaimingPriority(string path, string text)
        {

        }        
        
        private static void CollectLegalEvents(string path, string text)
        {

        }        
        
        private static void CollectConcepts(string path, string text)
        {

        }
    
        private static void ClickExpandClassification()
        {
            var (X1, Y1, _, _) = ScreenOCR.FindTextOnScreen("Classifications");

            X1 = Convert.ToInt32(X1 / 2 + 10);
            Y1 = Convert.ToInt32(Y1 / 2 + 10);

            for (int i = 0; i < 100; i += 5)
                Mouse.LeftClick(X1, Y1 + i);

            Mouse.LeftClick(0, Y1);
        }
    }
}

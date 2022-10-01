using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using DxMLEngine.Attributes;
using DxMLEngine.Functions;

using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using Microsoft.Data.Analysis;
using System.IO;
using System.Reflection.Metadata;
using System.Data;
using System.Text.RegularExpressions;

namespace DxMLEngine.Features.GoogleScholar
{
    [Feature]
    internal class GoogleScholar
    {
        private const string IdXPath = "/html/body/div/div[10]/div[2]/div[3]/div[2]/div[{index}]/div/h3/a";
        private const string TitleXpath = "/html/body/div/div[10]/div[2]/div[3]/div[2]/div[{index}]/div/h3/a";
        private const string AuthorsXPath = "/html/body/div/div[10]/div[2]/div[3]/div[2]/div[{index}]/div/div[1]";
        private const string PublisherXPath = "/html/body/div/div[10]/div[2]/div[3]/div[2]/div[{index}]/div/div[1]";
        private const string DescriptionXPath = "/html/body/div/div[10]/div[2]/div[3]/div[2]/div[{index}]/div/div[2]";
        private const string CitedByXPath = "/html/body/div/div[10]/div[2]/div[3]/div[2]/div[{index}]/div/div[3]/a[3]";
        private const string RelatedArticlesXPath = "/html/body/div/div[10]/div[2]/div[3]/div[2]/div[{index}]/div/div[3]/a[4]";
        private const string NumberOfVersionsXPath = "/html/body/div/div[10]/div[2]/div[3]/div[2]/div[{index}]/div/div[3]/a[5]";
        private const string UrlXPath = "/html/body/div/div[10]/div[2]/div[3]/div[2]/div[{index}]/div/h3/a";

        public static void DownloadSearchPages()
        {
            ////
            Console.Write("\nEnter input file path: ");
            var inFile = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(inFile))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var outDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(outDir))
                throw new ArgumentNullException("path is null or empty");

            ////
            var webpages = InputSearchUrls(inFile);

            ////
            var browser = BrowserAutomation.LaunghEdge();
            if (browser == null)
                throw new Exception("browser == null");

            ////
            foreach (var webpage in webpages)
            {   
                var tempTab = BrowserAutomation.OpenNewTab(browser, webpage.SearchUrl);
                webpage.PageText = BrowserAutomation.CopyPageText(tempTab);
                webpage.PageSource = BrowserAutomation.CopyPageSource(tempTab);
                BrowserAutomation.CloseCurrentTab(tempTab);

                var numPages = FindNumberOfPages(webpage);

                for (int i = 0; i < numPages; i++)
                {
                    webpage.Page = $"{i * 10}";
                    Console.WriteLine($"\nCollect: {webpage.SearchUrl}");

                    var newTab = BrowserAutomation.OpenNewTab(browser, webpage.SearchUrl);
                    BrowserAutomation.DownloadWebpage(newTab);
                    BrowserAutomation.CloseCurrentTab(newTab);
                }
            }
            BrowserAutomation.CloseBrowser(browser);
        }
        
        public static void CollectResearchPapers()
        {
            ////
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
                throw new ArgumentNullException("path is null or empty");

            ////
            var pageHtmls = InputPageHtmls(inFile);

            ////
            var researchPapers = new List<ResearchPaper>();
            foreach (var pageHtml in pageHtmls)
            {
                for (int i = 0; i < 10; i++)
                {
                    var idXPath = IdXPath.Replace("{index}", $"{i + 1}");
                    var titleXPath = TitleXpath.Replace("{index}", $"{i + 1}");
                    var authorsXPath = AuthorsXPath.Replace("{index}", $"{i + 1}");
                    var publisherXPath = PublisherXPath.Replace("{index}", $"{i + 1}");
                    var descriptionXPath = DescriptionXPath.Replace("{index}", $"{i + 1}");
                    var citedByXPath = CitedByXPath.Replace("{index}", $"{i + 1}");
                    var relatedArticlesXPath = RelatedArticlesXPath.Replace("{index}", $"{i + 1}");
                    var numberOfVersionsXPath = NumberOfVersionsXPath.Replace("{index}", $"{i + 1}");
                    var urlXPath = UrlXPath.Replace("{index}", $"{i + 1}");

                    var researchPaper = new ResearchPaper();

                    try
                    {
                        researchPaper.Id = pageHtml.DocumentNode
                            .SelectSingleNode(idXPath)
                            .GetAttributeValue("id", null);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            idXPath = idXPath.Replace("/div/h3/a", "/div[2]/h3/a");
                            researchPaper.Id = pageHtml.DocumentNode
                                .SelectSingleNode(idXPath)
                                .GetAttributeValue("id", null);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                idXPath = idXPath.Replace("/div[2]/h3/a", "/div/h3/span[2]");
                                researchPaper.Id = pageHtml.DocumentNode
                                    .SelectSingleNode(idXPath)
                                    .GetAttributeValue("id", null);
                            }
                            catch (Exception)
                            {
                                idXPath = idXPath.Replace("/div/h3/span[2]", "/div[2]/h3/span[2]");
                                researchPaper.Id = pageHtml.DocumentNode
                                    .SelectSingleNode(idXPath)
                                    .GetAttributeValue("id", null);
                            }
                        }
                    }

                    try
                    {
                        researchPaper.Title = pageHtml.DocumentNode
                            .SelectSingleNode(titleXPath).InnerText;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            titleXPath = titleXPath.Replace("/div/h3/a", "/div[2]/h3/a");
                            researchPaper.Title = pageHtml.DocumentNode
                                .SelectSingleNode(titleXPath).InnerText;
                        } 
                        catch (Exception)
                        {
                            try
                            {
                                titleXPath = titleXPath.Replace("/div[2]/h3/a", "/div/h3/span[2]");
                                researchPaper.Title = pageHtml.DocumentNode
                                    .SelectSingleNode(titleXPath).InnerText;
                            }
                            catch (Exception)
                            {
                                titleXPath = titleXPath.Replace("/div/h3/span[2]", "/div[2]/h3/span[2]");
                                researchPaper.Title = pageHtml.DocumentNode
                                    .SelectSingleNode(titleXPath).InnerText;
                            }
                        }
                    }

                    try
                    {
                        researchPaper.Authors = ((pageHtml.DocumentNode
                            .SelectSingleNode(authorsXPath).InnerText)
                            .Split("-", StringSplitOptions.TrimEntries)[0])
                            .Split(",", StringSplitOptions.TrimEntries);
                    }
                    catch (Exception)
                    {
                        authorsXPath = authorsXPath.Replace("/div/div[1]", "/div[2]/div[1]");
                        researchPaper.Authors = ((pageHtml.DocumentNode
                            .SelectSingleNode(authorsXPath).InnerText)
                            .Split("-", StringSplitOptions.TrimEntries)[0])
                            .Split(",", StringSplitOptions.TrimEntries);
                    }

                    try 
                    {
                        researchPaper.Publisher = (pageHtml.DocumentNode
                            .SelectSingleNode(publisherXPath).InnerText)
                            .Split("-", StringSplitOptions.TrimEntries)[1];
                    }
                    catch (Exception)
                    {
                        publisherXPath = publisherXPath.Replace("/div/div[1]", "/div[2]/div[1]");
                        researchPaper.Publisher = (pageHtml.DocumentNode
                            .SelectSingleNode(publisherXPath).InnerText)
                            .Split("-", StringSplitOptions.TrimEntries)[1];
                    }

                    try
                    {
                        researchPaper.Desription = pageHtml.DocumentNode
                            .SelectSingleNode(descriptionXPath)
                            .InnerText.Trim();
                    }
                    catch (Exception)
                    {
                        descriptionXPath = descriptionXPath.Replace("/div/div[2]", "/div[2]/div[2]");
                        researchPaper.Desription = pageHtml.DocumentNode
                            .SelectSingleNode(descriptionXPath)
                            .InnerText.Trim();
                    }    
                    
                    try
                    {
                        researchPaper.CitedBy = Convert.ToInt32((pageHtml.DocumentNode
                            .SelectSingleNode(citedByXPath).InnerText)
                            .Replace("Cited by ", ""));
                    }
                    catch (Exception)
                    {
                        citedByXPath = citedByXPath.Replace("/div/div[3]/a[3]", "/div[2]/div[3]/a[3]");
                        researchPaper.CitedBy = Convert.ToInt32((pageHtml.DocumentNode
                            .SelectSingleNode(citedByXPath).InnerText)
                            .Replace("Cited by ", ""));
                    }

                    try
                    {
                        researchPaper.RelatedArticles = pageHtml.DocumentNode
                            .SelectSingleNode(relatedArticlesXPath)
                            .GetAttributeValue("href", null);                      
                    }
                    catch (Exception)
                    {
                        relatedArticlesXPath = relatedArticlesXPath.Replace("/div/div[3]/a[4]", "/div[2]/div[3]/a[4]");
                        researchPaper.RelatedArticles = pageHtml.DocumentNode
                            .SelectSingleNode(relatedArticlesXPath)
                            .GetAttributeValue("href", null);
                    }

                    try
                    {
                        researchPaper.NumberOfVersions = Convert.ToInt32(pageHtml.DocumentNode
                            .SelectSingleNode(numberOfVersionsXPath)
                            .InnerText
                            .Replace("All ", "")
                            .Replace(" versions", ""));                        
                    }
                    catch (Exception)
                    {
                        try
                        {
                            numberOfVersionsXPath = numberOfVersionsXPath.Replace("/div/div[3]/a[5]", "/div[2]/div[3]/a[5]");
                            researchPaper.NumberOfVersions = Convert.ToInt32(pageHtml.DocumentNode
                                .SelectSingleNode(numberOfVersionsXPath)
                                .InnerText
                                .Replace("All ", "")
                                .Replace(" versions", ""));
                        }
                        catch
                        {
                            researchPaper.NumberOfVersions = null;
                        }
                    }

                    try
                    {
                        researchPaper.Url = pageHtml.DocumentNode
                            .SelectSingleNode(urlXPath)
                            .GetAttributeValue("href", null);                       
                    }
                    catch (Exception)
                    {
                        try
                        {
                            urlXPath = urlXPath.Replace("/div/h3/a", "/div[2]/h3/a");
                            researchPaper.Url = pageHtml.DocumentNode
                                .SelectSingleNode(urlXPath)
                                .GetAttributeValue("href", null);
                        }
                        catch (Exception)
                        {
                            researchPaper.Url = null;
                        }
                    }

                    researchPapers.Add(researchPaper);
                }
            }
            OutputResearchPapers(outDir, outFile, researchPapers.ToArray());
        }

        private static Webpage[] InputSearchUrls(string inFile)
        {
            var dataFrame = DataFrame.LoadCsv(inFile, header: true, separator: '\t', encoding: Encoding.UTF8);
            var webpages = new List<Webpage>();
            for (int i = 0; i < dataFrame.Rows.Count; i++) 
            {
                var webpage = new Webpage();
                webpage.Query = Convert.ToString(dataFrame["Query"][i]);
                webpage.Page = Convert.ToString(dataFrame["Page"][i]);
                webpage.Language = Convert.ToString(dataFrame["Language"][i]);
                webpage.FromYear = Convert.ToString(dataFrame["From Year"][i]);
                webpage.ToYear = Convert.ToString(dataFrame["To Year"][i]);
                webpage.Reviewed = Convert.ToBoolean(dataFrame["Reviewed"][i]);
                
                webpages.Add(webpage);
            }

            return webpages.ToArray();
        }

        private static HtmlDocument[] InputPageHtmls(string inFile)
        {
            var dataFrame = DataFrame.LoadCsv(inFile, header: true, separator: '\t', encoding: Encoding.UTF8);
            var pageHtmls = new List<HtmlDocument>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var path = Convert.ToString(dataFrame["Path"][i]);
                Console.WriteLine($"Input: {path}");

                var pageHtml = new HtmlDocument();
                pageHtml.Load(path);
                pageHtmls.Add(pageHtml);
            }

            return pageHtmls.ToArray();
        }

        private static void OutputResearchPapers(string outDir, string outFile, ResearchPaper[] papers)
        {
            var dataFrame = new DataFrame(new List<DataFrameColumn>()
                {
                    new StringDataFrameColumn("Id"),
                    new StringDataFrameColumn("Title"),
                    new StringDataFrameColumn("Authors"),
                    new StringDataFrameColumn("Publisher"),
                    new StringDataFrameColumn("Description"),
                    new StringDataFrameColumn("Cited By"),
                    new StringDataFrameColumn("Related Articles"),
                    new StringDataFrameColumn("Number of Versions"),
                    new StringDataFrameColumn("Url"),
                }
            );

            foreach (var paper in papers)
            {
                var dataRow = new List<KeyValuePair<string, object?>>()
                {
                    new KeyValuePair<string, object?>("Id", $"\"{paper.Id}\""),
                    new KeyValuePair<string, object?>("Title", $"\"{paper.Title}\""),
                    new KeyValuePair<string, object?>("Authors", 
                        paper.Authors != null ? $"\"{string.Join(", ", paper.Authors)}\"" : null),
                    new KeyValuePair<string, object?>("Publisher", $"\"{paper.Publisher}\""),
                    new KeyValuePair<string, object?>("Description",
                        paper.Desription != null ? $"\"{paper.Desription.Replace("\"", "\"\"")}\"" : null),                    
                    new KeyValuePair<string, object?>("Cited By", $"\"{paper.CitedBy}\""),
                    new KeyValuePair<string, object?>("Related Articles", $"\"{paper.RelatedArticles}\""),
                    new KeyValuePair<string, object?>("Number of Versions", $"\"{paper.NumberOfVersions}\""),
                    new KeyValuePair<string, object?>("Url", $"\"{paper.Url}\""),
                };

                Console.WriteLine($"Output: {dataRow}");
                dataFrame.Append(dataRow, inPlace: true);
            }

            outFile = $"{outDir}\\Dataset @{outFile}ResearchPapers #-------------- .csv";
            DataFrame.WriteCsv(dataFrame, outFile, header: true, separator: ',', encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(outFile).ToString("yyyyMMddHHmmss");
            File.Move(outFile, outFile.Replace("#--------------", $"#{timestamp}"));
        }
    
        private static int? FindNumberOfPages(Webpage webpage)
        {
            if (webpage.PageText == null)
                throw new ArgumentNullException("webpage.PageText != null");

            var match = Regex.Match(webpage.PageText, @"About [\d,]+ results [(][\d.]+ sec[)]");

            if (match.Success)
            {
                var numResults = int.Parse(
                    Regex.Replace(match.Value, @" results [(][\d.]+ sec[)]", @"")
                    .Replace(@"About ", "")
                    .Replace(@",", ""));

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
    }
}

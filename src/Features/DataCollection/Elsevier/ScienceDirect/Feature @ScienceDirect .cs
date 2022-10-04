using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Data.Analysis;

using DxMLEngine.Attributes;
using System.Diagnostics;
using System.Xml.Serialization;
using DxMLEngine.Utilities;

namespace DxMLEngine.Features.ScienceDirect
{
    [Feature]
    internal class ScienceDirect
    {
        public static void CollectArticles()
        {
            Console.Write("\nEnter input file path: ");
            var inFile = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(inFile))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var outDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(outDir))
                throw new ArgumentNullException("path is null or empty");

            var endpoints = InputScienceDirectSearchEndpoints(inFile);

            for (int i = 0; i < endpoints.Length; i++)
            {
                var endpoint = endpoints[i];
                Console.WriteLine($"\nCollect: {endpoint.ScienceDirectSearchEndpoint}");

                RequestScienceDirectSearch(ref endpoint);
                var articles = ExtractArticles(endpoint);

                OutputScienceDirectSearch(outDir, $"ScienceDirectSearch{i + 1}", endpoint);
                if (articles != null) OutputArticles(outDir, $"Articles{i + 1}", articles, FileFormat.TXT);
                if (articles != null) OutputArticles(outDir, $"Articles{i + 1}", articles, FileFormat.CSV);
                if (articles != null) OutputArticles(outDir, $"Articles{i + 1}", articles, FileFormat.JSON);
                if (articles != null) OutputArticles(outDir, $"Articles{i + 1}", articles, FileFormat.XML);
            }
        }

        public static void CollectJournals()
        {

        }

        public static void CollectAuthors()
        {

        }

        #region ENDPOINT IO

        private static Endpoint[] InputScienceDirectSearchEndpoints(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);

            var endpoints = new List<Endpoint>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var endpoint = new Endpoint();
                endpoint.Id = Convert.ToString(dataFrame["ID"][i]);
                endpoint.Query = Convert.ToString(dataFrame["Query"][i]);
                endpoint.ApiKey = Convert.ToString(dataFrame["API Key"][i]);
                endpoint.HttpAccept = Convert.ToString(dataFrame["Http Accept"][i]);

                endpoints.Add(endpoint);
            }

            return endpoints.ToArray();
        }
        
        private static void InputArticleRetrievalEndpoints()
        {

        }
        
        private static void InputArticleEntitlementRetrievalEndpoints()
        {

        }

        private static void InputObjectRetrievalEndpoints()
        {

        }

        private static void InputArticleMetadataEndpoints()
        {

        }

        private static void OutputScienceDirectSearch(string location, string fileName, Endpoint endpoint)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(endpoint.ScienceDirectSearch, options);

            var path = $"{location}\\Datason @{fileName} #-------------- .json";
            File.WriteAllText(path, jsonString, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputArticleRetrievalByDoi(string location, string fileName, Endpoint endpoint)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(endpoint.ArticleRetrievalByDoi, options);

            var path = $"{location}\\Datason @{fileName} #-------------- .json";
            File.WriteAllText(path, jsonString, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        #endregion ENDPOINT IO

        #region SEARCH IO

        private static void InputScienceDirectSearchSearch()
        {

        }

        private static void InputArticleRetrievalSearch()
        {

        }

        private static void InputArticleEntitlementRetrievalSearch()
        {

        }

        private static void InputObjectRetrievalSearch()
        {

        }

        private static void InputArticleMetadataSearch()
        {

        }

        private static void OutputScienceDirectSearchSearch()
        {

        }

        private static void OutputArticleRetrievalSearch()
        {

        }

        private static void OutputArticleEntitlementRetrievalSearch()
        {

        }

        private static void OutputObjectRetrievalSearch()
        {

        }

        private static void OutputArticleMetadataSearch()
        {

        }

        #endregion SEARCH IO

        #region API REQUEST

        private static void RequestScienceDirectSearch(ref Endpoint endpoint)
        {
            var client = new HttpClient();
            var uri = new Uri(endpoint.ScienceDirectSearchEndpoint);
            var request = new HttpRequestMessage() { RequestUri = uri };
            endpoint.Response = client.Send(request).Content.ReadAsStringAsync().Result;
        }

        private static void RequestArticleRetrieval(ref Endpoint endpoint)
        {

        }

        private static void RequestArticleEntitlementRetrieval(ref Endpoint endpoint)
        {

        }

        private static void RequestObjectRetrieval(ref Endpoint endpoint)
        {

        }

        private static void RequestArticleMetadata(ref Endpoint endpoint)
        {

        }

        #endregion API REQUEST

        #region WEB REQUEST

        private static void RequestSearchPage()
        {

        }

        private static void RequestArticlePage()
        {

        }

        private static void RequestJournalPage()
        {

        }

        private static void RequestAuthorPage()
        {

        }

        #endregion WEB REQUEST

        #region DATA EXTRACTION

        private static Article[]? ExtractArticles(object entity)
        {
            if (entity.GetType() == typeof(Endpoint))
            {
                var endpoint = (Endpoint)entity;
                return ExtractArticlesFromEndpoint(endpoint);
            }

            if (entity.GetType() == typeof(Search))
            {
                var search = (Search)entity;
                return ExtractArticlesFromWebpage(search);
            }

            return null;
        }

        private static Article[]? ExtractArticlesFromEndpoint(Endpoint endpoint)
        {
            var response = endpoint.ScienceDirectSearch;

            if (response == null) return null;
            if (response.searchResult == null) return null;
            if (response.searchResult.entry == null) return null;

            var articles = new List<Article>();
            foreach (var entry in response.searchResult.entry)
            {
                var article = new Article();
                article.Identifier = entry.dcidentifier;
                article.Title = entry.dctitle;
                article.Creator = entry.dccreator;
                article.Url = entry.prismurl;
                article.PublicationName = entry.prismpublicationName;
                article.Volume = entry.prismurl;
                article.CoverDate = entry.prismcoverDate;
                article.StartingPage = entry.prismstartingPage;
                article.EndingPage = entry.prismendingPage;
                article.Doi = entry.prismdoi;
                article.OpenAccess = entry.openaccess;
                article.Pii = entry.pii;

                if (entry.authorList != null)
                    if (entry.authorList.authors != null)
                        if (entry.authorList.authors != null)
                        {
                            var jsonString = Convert.ToString(entry.authorList.authors);
                            
                            if (jsonString != null)
                            {
                                try
                                {
                                    var authorDicts = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonString);
                                    article.Authors = (
                                        from authorDict in authorDicts
                                        select authorDict["$"]).ToArray();
                                }
                                catch (Exception) { article.Authors = new string[] { jsonString }; }
                            }
                        }

                articles.Add(article);
            }
            return articles.ToArray();
        }

        private static Article[]? ExtractArticlesFromWebpage(Search search)
        {

            return new Article[0];
        }

        private static void ExtractJournal()
        {

        }

        private static void ExtractAuthor()
        {

        }

        private static void ExtractArticles()
        {

        }

        private static void ExtractJournals()
        {

        }

        private static void ExtractAuthors()
        {

        }

        #endregion DATA EXTRACTION

        #region DATA COLLECTION

        private static Article[]? InputArticles(string path, FileFormat? format)
        {
            Log.Info("InputArticles");

            switch (format)
            {
                case FileFormat.TXT:
                    return ReadTXT(path);

                case FileFormat.CSV:
                    return ReadCSV(path);

                case FileFormat.JSON:
                    return ReadJSON(path);

                case FileFormat.XML:
                    return ReadXML(path);

                default:
                    return null;
            }
        }

        private static Article[] ReadTXT(string path)
        {
            throw new NotImplementedException();
        }

        private static Article[] ReadCSV(string path)
        {
            throw new NotImplementedException();
        }

        private static Article[] ReadJSON(string path)
        {
            throw new NotImplementedException();
        }

        private static Article[] ReadXML(string path)
        {
            throw new NotImplementedException();
        }

        private static void OutputArticles(string location, string fileName, Article[] articles, FileFormat? format)
        {
            Log.Info("OutputArticles");

            switch (format)
            {
                case FileFormat.TXT:
                    WriteTXT(location, fileName, articles);
                    break;

                case FileFormat.CSV:
                    WriteCSV(location, fileName, articles);
                    break;

                case FileFormat.JSON:
                    WriteJSON(location, fileName, articles);
                    break;

                case FileFormat.XML:
                    WriteXML(location, fileName, articles);
                    break;

                default:
                    Console.WriteLine(articles);
                    break;
            }
        }

        private static void WriteTXT(string location, string fileName, Article[] articles)
        {
            var lines = new List<string>();
            foreach (var article in articles)
            {
                lines.Add($"Identifier: {article.Identifier}");
                lines.Add($"Title: {article.Title}");
                lines.Add($"Creator: {article.Creator}");

                lines.Add($"Url: {article.Url}");
                lines.Add($"PublicationName: {article.PublicationName}");
                lines.Add($"Volume: {article.Volume}");
                lines.Add($"CoverDate: {article.CoverDate}");
                lines.Add($"StartingPage: {article.StartingPage}");
                lines.Add($"EndingPage: {article.EndingPage}");
                lines.Add($"Doi: {article.Doi}");

                lines.Add($"OpenAccess: {article.OpenAccess}");
                lines.Add($"Pii: {article.Pii}");
                lines.Add($"Authors: {article.Authors}");

                lines.Add($"\n");
            }

            var path = $"{location}\\Datadoc @{fileName} #-------------- .txt";
            File.WriteAllLines(path, lines, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void WriteCSV(string location, string fileName, Article[] articles)
        {
            var dataFrame = new DataFrame(new List<DataFrameColumn>()
            {
                new StringDataFrameColumn("Identifier"),
                new StringDataFrameColumn("Title"),
                new StringDataFrameColumn("Creator"),
                new StringDataFrameColumn("Url"),
                new StringDataFrameColumn("Publication Name"),
                new StringDataFrameColumn("Volume"),
                new StringDataFrameColumn("Cover Date"),
                new StringDataFrameColumn("Starting Page"),
                new StringDataFrameColumn("Ending Page"),
                new StringDataFrameColumn("DOI"),
            });

            foreach (var article in articles)
            {
                var dataRow = new List<KeyValuePair<string, object?>>()
                {
                    new KeyValuePair<string, object?>("Identifier", $"\"{article.Identifier}\""),
                    new KeyValuePair<string, object?>("Title", $"\"{article.Title}\""),
                    new KeyValuePair<string, object?>("Creator", $"\"{article.Creator}\""),
                    new KeyValuePair<string, object?>("Url", $"\"{article.Url}\""),
                    new KeyValuePair<string, object?>("Publication Name", $"\"{article.PublicationName}\""),
                    new KeyValuePair<string, object?>("Volume", $"\"{article.Volume}\""),
                    new KeyValuePair<string, object?>("Cover Date", $"\"{article.CoverDate}\""),
                    new KeyValuePair<string, object?>("Starting Page", $"\"{article.StartingPage}\""),
                    new KeyValuePair<string, object?>("Ending Page", $"\"{article.EndingPage}\""),
                    new KeyValuePair<string, object?>("DOI", $"\"{article.Doi}\""),
                };

                dataFrame.Append(dataRow, inPlace: true);
            }

            var path = $"{location}\\Dataset @{fileName} #-------------- .csv";
            DataFrame.WriteCsv(dataFrame, path, header: true, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void WriteJSON(string location, string fileName, Article[] articles)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(articles, options);

            var path = $"{location}\\Datason @{fileName} #-------------- .json";
            File.WriteAllText(path, jsonString, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void WriteXML(string location, string fileName, Article[] articles)
        {
            var serializer = new XmlSerializer(typeof(Article[]));            

            var path = $"{location}\\Dataxml @{fileName} #-------------- .xml";
            using (var stream = new StreamWriter(path)) 
                serializer.Serialize(stream, articles);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        #endregion DATA COLLECTION
    }

    internal enum FileFormat
    {
        TXT = 0,
        CSV = 1,
        JSON = 2,
        XML = 3,
    }
}

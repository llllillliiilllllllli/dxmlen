using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml.Serialization;

using Microsoft.Data.Analysis;

using DxMLEngine.Attributes;
using DxMLEngine.Utilities;
using DxMLEngine.Objects;

namespace DxMLEngine.Features.ScienceDirect
{
    [Feature]
    internal class ScienceDirect
    {
        public static void SearchArticles()
        {
            Console.Write("\nEnter input file path: ");
            var inFile = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(inFile))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var outDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(outDir))
                throw new ArgumentNullException("path is null or empty");

            var endpoints = InputScienceDirectSearchRequests(inFile);

            for (int i = 0; i < endpoints.Length; i++)
            {
                var endpoint = endpoints[i];
                if (endpoint.Request == null)
                    throw new ArgumentNullException("endpoint.Request == null");

                Console.WriteLine($"\nCollect: {endpoint.Request.ScienceDirectSearchUrl}");

                RequestScienceDirectSearch(ref endpoint);
                var results = ExtractSearchResults(endpoint);

                OutputScienceDirectSearchResponse(outDir, $"Search{endpoint.Request!.Id}", endpoint);
                if (results != null) OutputSearchResults(outDir, $"Articles{endpoint.Request!.Id}", results, FileFormat.Txt);
                if (results != null) OutputSearchResults(outDir, $"Articles{endpoint.Request!.Id}", results, FileFormat.Csv);
                if (results != null) OutputSearchResults(outDir, $"Articles{endpoint.Request!.Id}", results, FileFormat.Json);
                if (results != null) OutputSearchResults(outDir, $"Articles{endpoint.Request!.Id}", results, FileFormat.Xml);
            }
        }

        public static void RetrieveArticles()
        {
            Console.Write("\nEnter input file path: ");
            var inFile = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(inFile))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var outDir = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(outDir))
                throw new ArgumentNullException("path is null or empty");

            var endpoints = InputArticleRetrievalRequests(inFile);

            for (int i = 0; i < endpoints.Length; i++)
            {
                var endpoint = endpoints[i];
                if (endpoint.Request == null)
                    throw new ArgumentNullException("endpoint.Request == null");

                Console.WriteLine($"\nCollect: {endpoint.Request.ArticleRetrievalUrl}");

                RequestArticleRetrieval(ref endpoint);
                var articles = ExtractArticles(endpoint);

                OutputArticleRetrievalResponse(outDir, $"Search{endpoint.Request!.Id}", endpoint);
                if (articles != null) OutputArticles(outDir, $"Articles{endpoint.Request!.Id}", articles, FileFormat.Txt);
                if (articles != null) OutputArticles(outDir, $"Articles{endpoint.Request!.Id}", articles, FileFormat.Csv);
                if (articles != null) OutputArticles(outDir, $"Articles{endpoint.Request!.Id}", articles, FileFormat.Json);
                if (articles != null) OutputArticles(outDir, $"Articles{endpoint.Request!.Id}", articles, FileFormat.Xml);
            }
        }

        #region ENDPOINT IO

        private static Endpoint[] InputScienceDirectSearchRequests(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);

            var endpoints = new List<Endpoint>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var request = new Request();
                request.Id = Convert.ToString(dataFrame["ID"][i]);
                request.Query = Convert.ToString(dataFrame["Query"][i]);
                request.ApiKey = Convert.ToString(dataFrame["API Key"][i]);
                request.HttpAccept = Convert.ToString(dataFrame["Http Accept"][i]);

                var endpoint = new Endpoint();
                endpoint.Request = request;

                endpoints.Add(endpoint);
            }

            return endpoints.ToArray();
        }

        private static Endpoint[] InputArticleRetrievalRequests(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);

            var endpoints = new List<Endpoint>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var request = new Request();
                request.Id = Convert.ToString(dataFrame["ID"][i]);
                request.ApiKey = Convert.ToString(dataFrame["API Key"][i]);
                request.HttpAccept = Convert.ToString(dataFrame["Http Accept"][i]);
                
                switch (Convert.ToString(dataFrame["Retrieve By"][i]))
                {
                    case "DOI":
                        request.Doi = Convert.ToString(dataFrame["DOI"][i]);
                        request.RetrieveBy = RetrieveBy.Doi;
                        break;

                    case "EID":
                        request.Eid = Convert.ToString(dataFrame["EID"][i]);
                        request.RetrieveBy = RetrieveBy.Eid;
                        break;

                    case "PII":
                        request.Eid = Convert.ToString(dataFrame["PII"][i]);
                        request.RetrieveBy = RetrieveBy.Pii;
                        break;

                    case "PUBMED":
                        request.Eid = Convert.ToString(dataFrame["PUBMED"][i]);
                        request.RetrieveBy = RetrieveBy.PubMedId;
                        break;

                    default:
                        request.RetrieveBy = null;
                        break;
                }

                var endpoint = new Endpoint();
                endpoint.Request = request;

                if (request.RetrieveBy != null)
                    endpoints.Add(endpoint);
            }

            return endpoints.ToArray();
        }

        private static Endpoint[] InputArticleEntitlementRetrievalRequests(string path)
        {
            throw new NotImplementedException();
        }

        private static Endpoint[] InputObjectRetrievalRequests(string path)
        {
            throw new NotImplementedException();
        }

        private static Endpoint[] InputArticleMetadataRequests(string path)
        {
            throw new NotImplementedException();
        }

        private static void OutputScienceDirectSearchResponse(string location, string fileName, Endpoint endpoint)
        {
            if (endpoint.Response == null)
                throw new ArgumentNullException("endpoint.Response == null");

            var jsonObject = endpoint.Response.ScienceDirectSearch;
            var options = new JsonSerializerOptions() { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(jsonObject, options);

            var path = $"{location}\\Datason @{fileName} #-------------- .json";
            File.WriteAllText(path, jsonString, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputArticleRetrievalResponse(string location, string fileName, Endpoint endpoint)
        {
            if (endpoint.Response == null)
                throw new ArgumentNullException("endpoint.Response == null");

            var jsonObject = endpoint.Response.ArticleRetrieval;
            var options = new JsonSerializerOptions() { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(jsonObject, options);

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
            if (endpoint.Request == null)
                throw new ArgumentNullException("endpoint.Request == null");

            if (endpoint.Response == null)
                throw new ArgumentNullException("endpoint.Response == null");

            var client = new HttpClient();
            var uri = new Uri(endpoint.Request.ScienceDirectSearchUrl);
            var request = new HttpRequestMessage() { RequestUri = uri };
            endpoint.Response.Content = client.Send(request).Content.ReadAsStringAsync().Result;
        }

        private static void RequestArticleRetrieval(ref Endpoint endpoint)
        {
            if (endpoint.Request == null)
                throw new ArgumentNullException("endpoint.Request == null");

            var client = new HttpClient();            
            var uri = new Uri(endpoint.Request.ArticleRetrievalUrl);           
            var request = new HttpRequestMessage() { RequestUri = uri };

            var response = new Response();
            response.Content = client.Send(request).Content.ReadAsStringAsync().Result;

            endpoint.Response = response;
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

        private static SearchResult[]? ExtractSearchResults(object entity)
        {
            if (entity.GetType() == typeof(Endpoint))
            {
                var endpoint = (Endpoint)entity;
                return ExtractSearchResultsFromEndpoint(endpoint);
            }

            if (entity.GetType() == typeof(Search))
            {
                var search = (Search)entity;
                return ExtractSearchResultsFromWebpage(search);
            }

            return null;
        }

        private static SearchResult[]? ExtractSearchResultsFromEndpoint(Endpoint endpoint)
        {
            if (endpoint.Response == null)
                throw new ArgumentNullException("endpoint.Response == null");

            var response = endpoint.Response.ScienceDirectSearch;

            if (response == null) return null;
            if (response.Result == null) return null;
            if (response.Result.Entry == null) return null;

            var results = new List<SearchResult>();
            foreach (var entry in response.Result.Entry)
            {
                var result = new SearchResult();
                result.Identifier = entry.Identifier;
                result.Title = entry.Title;
                result.Creator = entry.Creator;
                result.Url = entry.Url;
                result.PublicationName = entry.PublicationName;
                result.Volume = entry.Url;
                result.CoverDate = entry.CoverDate;
                result.StartingPage = entry.StartingPage;
                result.EndingPage = entry.EndingPage;
                result.Doi = entry.Doi;
                result.OpenAccess = entry.OpenAccess;
                result.Pii = entry.Pii;

                if (entry.AuthorList != null)
                    if (entry.AuthorList.Authors != null)
                        if (entry.AuthorList.Authors != null)
                        {
                            var jsonString = Convert.ToString(entry.AuthorList.Authors);

                            if (jsonString != null)
                            {
                                try
                                {
                                    var authorDicts = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonString);
                                    result.Authors = (
                                        from authorDict in authorDicts
                                        select authorDict["$"]).ToArray();
                                }
                                catch (Exception) { result.Authors = new string[] { jsonString }; }
                            }
                        }

                results.Add(result);
            }
            return results.ToArray();
        }

        private static SearchResult[]? ExtractSearchResultsFromWebpage(Search search)
        {
            return new SearchResult[0];
        }

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
            if (endpoint.Response == null)
                throw new ArgumentNullException("endpoint.Response == null");

            var response = endpoint.Response.ArticleRetrieval;

            if (response == null) return null;
            if (response.ResponseText == null) return null;
            if (response.ResponseText.CoreData == null) return null;

            var article = new Article();
            article.Doi = response.ResponseText.CoreData.Doi;
            article.Eid = response.ResponseText.CoreData.Eid;
            article.Pii = response.ResponseText.CoreData.Pii;
            article.PubmedId = response.ResponseText.PubmedId;
            article.ScopusId = response.ResponseText.ScopusId;

            article.Title = response.ResponseText.CoreData.Title;
            article.Description = response.ResponseText.CoreData.Description;
            
            article.Keywords = (
                from subject in response.ResponseText.CoreData.Subjects
                select subject.Term).ToArray();

            article.Creators = (
                from creator in response.ResponseText.CoreData.CreatorList
                select creator.Name).ToArray();

            article.Publisher = response.ResponseText.CoreData.Publisher;
            article.Volume = response.ResponseText.CoreData.Volume;
            article.Type = response.ResponseText.CoreData.AggregationType;
            
            if (response.ResponseText.CoreData.CoverDate != null)
                article.CoverDate = DateTime.Parse(response.ResponseText.CoreData.CoverDate);
            
            article.PageRange = response.ResponseText.CoreData.PageRange;

            if (response.ResponseText.CoreData.OpenAccess == "1")
                article.OpenAccess = true;
            else
                article.OpenAccess = false;

            article.Url = response.ResponseText.CoreData.Url;

            return new Article[1] { article };
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

        private static void OutputSearchResults(string location, string fileName, SearchResult[] results, FileFormat? format)
        {
            Log.Info("OutputSearchResults");

            switch (format)
            {
                case FileFormat.Txt:
                    WriteTXT(location, fileName, results);
                    break;

                case FileFormat.Csv:
                    WriteCSV(location, fileName, results);
                    break;

                case FileFormat.Json:
                    WriteJSON(location, fileName, results);
                    break;

                case FileFormat.Xml:
                    WriteXML(location, fileName, results);
                    break;

                default:
                    Console.WriteLine(results);
                    break;
            }
        }

        private static void WriteTXT(string location, string fileName, SearchResult[] results)
        {
            var lines = new List<string>();
            foreach (var result in results)
            {
                lines.Add($"Identifier: {result.Identifier}");
                lines.Add($"Title: {result.Title}");
                lines.Add($"Creator: {result.Creator}");

                lines.Add($"Url: {result.Url}");
                lines.Add($"PublicationName: {result.PublicationName}");
                lines.Add($"Volume: {result.Volume}");
                lines.Add($"CoverDate: {result.CoverDate}");
                lines.Add($"StartingPage: {result.StartingPage}");
                lines.Add($"EndingPage: {result.EndingPage}");
                lines.Add($"Doi: {result.Doi}");

                lines.Add($"OpenAccess: {result.OpenAccess}");
                lines.Add($"Pii: {result.Pii}");
                lines.Add($"Authors: {result.Authors}");

                lines.Add($"\n");
            }

            var path = $"{location}\\Datadoc @{fileName} #-------------- .txt";
            File.WriteAllLines(path, lines, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void WriteCSV(string location, string fileName, SearchResult[] results)
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

            foreach (var result in results)
            {
                var dataRow = new List<KeyValuePair<string, object?>>()
                {
                    new KeyValuePair<string, object?>("Identifier", $"\"{result.Identifier}\""),
                    new KeyValuePair<string, object?>("Title", $"\"{result.Title}\""),
                    new KeyValuePair<string, object?>("Creator", $"\"{result.Creator}\""),
                    new KeyValuePair<string, object?>("Url", $"\"{result.Url}\""),
                    new KeyValuePair<string, object?>("Publication Name", $"\"{result.PublicationName}\""),
                    new KeyValuePair<string, object?>("Volume", $"\"{result.Volume}\""),
                    new KeyValuePair<string, object?>("Cover Date", $"\"{result.CoverDate}\""),
                    new KeyValuePair<string, object?>("Starting Page", $"\"{result.StartingPage}\""),
                    new KeyValuePair<string, object?>("Ending Page", $"\"{result.EndingPage}\""),
                    new KeyValuePair<string, object?>("DOI", $"\"{result.Doi}\""),
                };

                dataFrame.Append(dataRow, inPlace: true);
            }

            var path = $"{location}\\Dataset @{fileName} #-------------- .csv";
            DataFrame.WriteCsv(dataFrame, path, header: true, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void WriteJSON(string location, string fileName, SearchResult[] results)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(results, options);

            var path = $"{location}\\Datason @{fileName} #-------------- .json";
            File.WriteAllText(path, jsonString, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void WriteXML(string location, string fileName, SearchResult[] results)
        {
            var serializer = new XmlSerializer(typeof(SearchResult[]));

            var path = $"{location}\\Dataxml @{fileName} #-------------- .xml";
            using (var stream = new StreamWriter(path))
                serializer.Serialize(stream, results);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputArticles(string location, string fileName, Article[] results, FileFormat? format)
        {
            Log.Info("OutputArticles");

            switch (format)
            {
                case FileFormat.Txt:
                    WriteTXT(location, fileName, results);
                    break;

                case FileFormat.Csv:
                    WriteCSV(location, fileName, results);
                    break;

                case FileFormat.Json:
                    WriteJSON(location, fileName, results);
                    break;

                case FileFormat.Xml:
                    WriteXML(location, fileName, results);
                    break;

                default:
                    Console.WriteLine(results);
                    break;
            }
        }

        private static void WriteTXT(string location, string fileName, Article[] articles)
        {
            var lines = new List<string>();
            foreach (var article in articles)
            {
                lines.Add($"Title: {article.Title}");
                lines.Add($"Description: {article.Description}");

                if (article.Keywords != null)
                    lines.Add($"Keywords: {string.Join(", ", article.Keywords)}");
                else
                    lines.Add($"Keywords: ");

                if (article.Creators != null)
                    lines.Add($"Creators: {string.Join(", ", article.Creators)}");
                else
                    lines.Add($"Creators: ");

                lines.Add($"Publisher: {article.Publisher}");
                lines.Add($"Volume: {article.Volume}");
                lines.Add($"Type: {article.Type}");
                lines.Add($"Cover Date: {article.CoverDate}");
                lines.Add($"Page Range: {article.PageRange}");
                lines.Add($"Open Access: {article.OpenAccess}");

                lines.Add($"DOI: {article.Doi}");
                lines.Add($"EID: {article.Eid}");
                lines.Add($"PII: {article.Pii}");
                lines.Add($"Pubmed ID: {article.PubmedId}");
                lines.Add($"Scopus ID: {article.ScopusId}");

                lines.Add($"URL: {article.Url}");

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
                new StringDataFrameColumn("Title"),
                new StringDataFrameColumn("Description"),
                new StringDataFrameColumn("Keywords"),
                new StringDataFrameColumn("Creators"),
                new StringDataFrameColumn("Publisher"),
                new StringDataFrameColumn("Volume"),
                new StringDataFrameColumn("Type"),
                new StringDataFrameColumn("Cover Date"),
                new StringDataFrameColumn("Page Range"),
                new StringDataFrameColumn("Open Access"),
                new StringDataFrameColumn("DOI"),
                new StringDataFrameColumn("EID"),
                new StringDataFrameColumn("PII"),
                new StringDataFrameColumn("Pubmed ID"),
                new StringDataFrameColumn("Scopus ID"),
                new StringDataFrameColumn("URL"),
            });

            foreach (var article in articles)
            {
                var dataRow = new List<KeyValuePair<string, object?>>()
                {
                    new KeyValuePair<string, object?>("Title", $"\"{article.Title}\""),
                    new KeyValuePair<string, object?>("Description", $"\"{article.Description}\""),
                    new KeyValuePair<string, object?>("Keywords", article.Keywords != null ?
                        $"\"{string.Join(", ", article.Keywords)}\"" : null),
                    new KeyValuePair<string, object?>("Creators", article.Creators != null ?
                        $"\"{string.Join(", ", article.Creators)}\"" : null),
                    new KeyValuePair<string, object?>("Publisher", $"\"{article.Publisher}\""),
                    new KeyValuePair<string, object?>("Volume", $"\"{article.Volume}\""),
                    new KeyValuePair<string, object?>("Type", $"\"{article.Type}\""),
                    new KeyValuePair<string, object?>("Cover Date", $"\"{article.CoverDate}\""),
                    new KeyValuePair<string, object?>("Page Range", $"\"{article.PageRange}\""),
                    new KeyValuePair<string, object?>("Open Access", $"\"{article.OpenAccess}\""),
                    new KeyValuePair<string, object?>("DOI", $"\"{article.Doi}\""),
                    new KeyValuePair<string, object?>("EID", $"\"{article.Eid}\""),
                    new KeyValuePair<string, object?>("PII", $"\"{article.Pii}\""),
                    new KeyValuePair<string, object?>("Pubmed ID", $"\"{article.PubmedId}\""),
                    new KeyValuePair<string, object?>("Scopus ID", $"\"{article.ScopusId}\""),
                    new KeyValuePair<string, object?>("URL", $"\"{article.Url}\""),
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
}

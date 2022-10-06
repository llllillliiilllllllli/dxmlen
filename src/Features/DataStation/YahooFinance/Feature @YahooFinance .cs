using DxMLEngine.Features.GooglePatents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DxMLEngine.Attributes;
using Microsoft.Data.Analysis;
using System.IO;
using System.Text.Json;
using System.DirectoryServices;

namespace DxMLEngine.Features.YahooFinance
{
    [Feature]
    internal class YahooFinance
    {
        public static void CollectSearch()
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
            var endpoints = InputSearchEndpoints(inFile);
            
            ////2
            var client = new HttpClient();

            ////3
            foreach (var endpoint in endpoints)
            {
                Console.WriteLine($"\nCollect: {endpoint.SearchEndpoint}");

                var uri = new Uri(endpoint.SearchEndpoint);
                var request = new HttpRequestMessage() { RequestUri = uri };
                endpoint.Response = client.Send(request)
                    .Content.ReadAsStringAsync().Result;

                var search = JsonSerializer.Deserialize<Search>(endpoint.Response);
                
                OutputSearchData(outDir, endpoint);
            }
        }

        public static void CollectQuotes()
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
            var endpoints = InputQuotesEndpoints(inFile);

            ////2
            var client = new HttpClient();

            ////3
            foreach (var endpoint in endpoints)
            {
                Console.WriteLine($"\nCollect: {endpoint.QuotesEndpoint}");

                var uri = new Uri(endpoint.QuotesEndpoint);
                var request = new HttpRequestMessage() { RequestUri = uri };
                endpoint.Response = client.Send(request)
                    .Content.ReadAsStringAsync().Result;

                var quotes = JsonSerializer.Deserialize<Quotes>(endpoint.Response);

                OutputQuotesData(outDir, endpoint);
            }
        }

        public static void CollectHistory()
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
            var endpoints = InputHistoryEndpoints(inFile);

            ////2
            var client = new HttpClient();

            ////3
            foreach (var endpoint in endpoints)
            {
                Console.WriteLine($"\nCollect: {endpoint.HistoryEndpoint}");

                var uri = new Uri(endpoint.HistoryEndpoint);
                var request = new HttpRequestMessage() { RequestUri = uri };
                endpoint.Response = client.Send(request)
                    .Content.ReadAsStringAsync().Result;

                var history = JsonSerializer.Deserialize<History>(endpoint.Response);

                OutputHistoryData(outDir, endpoint);
            }
        }

        public static void CollectSummary()
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
            var endpoints = InputSummaryEndpoints(inFile);

            ////2
            var client = new HttpClient();

            ////3
            foreach (var endpoint in endpoints)
            {
                Console.WriteLine($"\nCollect: {endpoint.SummaryEndpoint}");

                var uri = new Uri(endpoint.SummaryEndpoint);
                var request = new HttpRequestMessage() { RequestUri = uri };
                endpoint.Response = client.Send(request)
                    .Content.ReadAsStringAsync().Result;

                var summary = JsonSerializer.Deserialize<Summary>(endpoint.Response);

                OutputSummaryData(outDir, endpoint);
            }
        }

        #region INPUT OUTPUT

        private static Endpoint[] InputSearchEndpoints(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);

            var endpoints = new List<Endpoint>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var endpoint = new Endpoint();
                endpoint.Id = Convert.ToString(dataFrame["ID"][i]);
                endpoint.Query = Convert.ToString(dataFrame["Query"][i]);

                endpoints.Add(endpoint);
            }

            return endpoints.ToArray();
        }

        private static Endpoint[] InputQuotesEndpoints(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);

            var endpoints = new List<Endpoint>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var endpoint = new Endpoint();

                endpoint.Id = dataFrame["ID"][i] != null ? dataFrame["ID"][i].ToString() : null;
                var symbols = dataFrame["Symbols"][i] != null ? dataFrame["Symbols"][i].ToString() : null;
                if (symbols != null)
                    endpoint.Symbols = symbols.Split(",", StringSplitOptions.TrimEntries);

                endpoints.Add(endpoint);
            }

            return endpoints.ToArray();
        }

        private static Endpoint[] InputHistoryEndpoints(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);

            var endpoints = new List<Endpoint>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var endpoint = new Endpoint();

                endpoint.Id = dataFrame["ID"][i] != null ? dataFrame["ID"][i].ToString() : null;
                endpoint.Symbol = dataFrame["Symbol"][i] != null ? dataFrame["Symbol"][i].ToString() : null;
                
                endpoint.Interval = dataFrame["Interval"][i] != null ? dataFrame["Interval"][i].ToString() : null;
                endpoint.Range = dataFrame["Range"][i] != null ? dataFrame["Range"][i].ToString() : null;
                endpoint.Period1 = dataFrame["Period1"][i] != null ? dataFrame["Period1"][i].ToString() : null;
                endpoint.Period2 = dataFrame["Period2"][i] != null ? dataFrame["Period2"][i].ToString() : null;
                endpoint.Close = dataFrame["Close"][i] != null ? Convert.ToBoolean(dataFrame["Close"][i]) : null;
                endpoint.Events = dataFrame["Events"][i] != null ? Convert.ToBoolean(dataFrame["Events"][i]) : null;
                endpoint.Prepost = dataFrame["Prepost"][i] != null ? Convert.ToBoolean(dataFrame["Prepost"][i]) : null;
                
                endpoints.Add(endpoint);
            }

            return endpoints.ToArray();
        }

        private static Endpoint[] InputSummaryEndpoints(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);

            var endpoints = new List<Endpoint>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var endpoint = new Endpoint();

                endpoint.Id = dataFrame["ID"][i] != null ? dataFrame["ID"][i].ToString() : null;
                endpoint.Symbol = dataFrame["Symbol"][i] != null ? dataFrame["Symbol"][i].ToString() : null;
                var modules = dataFrame["Modules"][i] != null ? dataFrame["Modules"][i].ToString() : null;
                if (modules != null)
                    endpoint.Modules = modules.Split(",", StringSplitOptions.TrimEntries);

                endpoints.Add(endpoint);
            }

            return endpoints.ToArray();
        }

        private static void OutputSearchData(string location, Endpoint endpoint)
        {
            var path = $"{location}\\Datason @{endpoint.Id}Search #-------------- .json";
            File.WriteAllText(path, endpoint.Response, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputQuotesData(string location, Endpoint endpoint)
        {
            var path = $"{location}\\Datason @{endpoint.Id}Quotes #-------------- .json";
            File.WriteAllText(path, endpoint.Response, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputHistoryData(string location, Endpoint endpoint)
        {
            var path = $"{location}\\Datason @{endpoint.Id}History #-------------- .json";
            File.WriteAllText(path, endpoint.Response, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputSummaryData(string location, Endpoint endpoint)
        {
            var path = $"{location}\\Datason @{endpoint.Id}Summary #-------------- .json";
            File.WriteAllText(path, endpoint.Response, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        #endregion INPUT OUTPUT
    }
}

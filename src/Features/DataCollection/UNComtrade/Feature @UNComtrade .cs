/// ====================================================================================
/// UN Comtrade Data Extraction API
/// This is a Public API by UN to provide data about trade by commodities and services.
/// It does not require authentication but registered users have higher usage limits. 
/// UN Comtrade data extraction API allows extracting result data in JSON or CSV format. 
/// 
/// https://comtrade.un.org/data/doc/api
/// ====================================================================================

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

using Microsoft.Data.Analysis;

using DxMLEngine.Attributes;
using DxMLEngine.Functions;

namespace DxMLEngine.Features.UNComtrade
{
    [Feature]
    internal class UNComtrade
    {
        public static void CollectDataAvailability()
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
            var endpoints = InputAvailabilityEndpoints(inFile);

            ////2
            var client = new HttpClient();

            ////3
            foreach (var endpoint in endpoints)
            {
                Console.WriteLine($"\nCollect: {endpoint.AvailabilityEndpoint}");

                var uri = new Uri(endpoint.AvailabilityEndpoint);
                var request = new HttpRequestMessage() { RequestUri = uri };
                var response = client.Send(request);
                endpoint.Reponse = response.Content.ReadAsStringAsync().Result;

                Console.WriteLine(endpoint.Reponse);
                Console.WriteLine();

                OutputAvailability(outDir, endpoint);
            }            
        }

        public static void CollectTradeData()
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
            var endpoints = InputTradeDataEndpoints(inFile);

            ////2
            var client = new HttpClient();

            ////3
            foreach (var endpoint in endpoints)
            {
                Console.WriteLine($"\nCollect: {endpoint.TradeDataEndpoint}");

                var uri = new Uri(endpoint.AvailabilityEndpoint);
                var request = new HttpRequestMessage() { RequestUri = uri };
                var response = client.Send(request);
                endpoint.Reponse = response.Content.ReadAsStringAsync().Result;

                Console.WriteLine(endpoint.Reponse);
                Console.WriteLine();

                OutputTradeData(outDir, endpoint);
            }
        }

        #region INPUT OUTPUT

        private static Endpoint[] InputAvailabilityEndpoints(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);

            var endpoints = new List<Endpoint>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var endpoint = new Endpoint();

                endpoint.Id = dataFrame["ID"][i] != null ? dataFrame["ID"][i].ToString() : null;
                endpoint.TradeType = dataFrame["Trade Type"][i] != null ? dataFrame["Trade Type"][i].ToString() : null;
                endpoint.Frequency = dataFrame["Data Frequency"][i] != null ? dataFrame["Data Frequency"][i].ToString() : null;
                endpoint.ReportingArea = dataFrame["Reporting Area"][i] != null ? dataFrame["Reporting Area"][i].ToString() : null;
                endpoint.TimePeriod = dataFrame["Time Period"][i] != null ? dataFrame["Time Period"][i].ToString() : null;
                endpoint.Classification = dataFrame["Classification"][i] != null ? dataFrame["Classification"][i].ToString() : null;
                endpoint.ApiToken = dataFrame["API Token"][i] != null ? dataFrame["API Token"][i].ToString() : null;

                endpoints.Add(endpoint);
            }

            return endpoints.ToArray();
        }

        private static Endpoint[] InputTradeDataEndpoints(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, separator: '\t', encoding: Encoding.UTF8);

            var endpoints = new List<Endpoint>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var endpoint = new Endpoint();

                endpoint.Id = dataFrame["ID"][i] != null ? dataFrame["ID"][i].ToString() : null;
                endpoint.TradeType = dataFrame["Trade Type"][i] != null ? dataFrame["Trade Type"][i].ToString() : null;
                endpoint.Frequency = dataFrame["Data Frequency"][i] != null ? dataFrame["Data Frequency"][i].ToString() : null;
                endpoint.ReportingArea = dataFrame["Reporting Area"][i] != null ? dataFrame["Reporting Area"][i].ToString() : null;
                endpoint.TimePeriod = dataFrame["Time Period"][i] != null ? dataFrame["Time Period"][i].ToString() : null;
                endpoint.Classification = dataFrame["Classification"][i] != null ? dataFrame["Classification"][i].ToString() : null;
                
                endpoint.ClassificationCode = dataFrame["Classification Code"][i] != null ? dataFrame["Classification Code"][i].ToString() : null;
                endpoint.PartnerArea = dataFrame["Partner Area"][i] != null ? dataFrame["Partner Area"][i].ToString() : null;
                endpoint.TradeFlow = dataFrame["Trade Flow"][i] != null ? dataFrame["Trade Flow"][i].ToString() : null;
                endpoint.OutputFormat = dataFrame["Output Format"][i] != null ? dataFrame["Output Format"][i].ToString() : null;
                endpoint.MaxRecords = dataFrame["Max Records"][i] != null ? dataFrame["Max Records"][i].ToString() : null;
                endpoint.HeadingStyle = dataFrame["Heading Style"][i] != null ? dataFrame["Heading Style"][i].ToString() : null;
                endpoint.IMTS = dataFrame["IMTS"][i] != null ? dataFrame["IMTS"][i].ToString() : null;               
                
                endpoint.ApiToken = dataFrame["API Token"][i] != null ? dataFrame["API Token"][i].ToString() : null;

                endpoints.Add(endpoint);
            }

            return endpoints.ToArray();
        }
    
        private static void OutputAvailability(string location, Endpoint endpoint)
        {
            var path = $"{location}\\Datason @{endpoint.Id}Availability #-------------- .json";
            File.WriteAllText(path, endpoint.Reponse, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputTradeData(string location, Endpoint endpoint)
        {
            var path = $"{location}\\Datason @{endpoint.Id}TradeData #-------------- .json";
            File.WriteAllText(path, endpoint.Reponse, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        #endregion INPUT OUTPUT
    }
}

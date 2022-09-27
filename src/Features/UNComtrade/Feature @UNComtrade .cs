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

            ////1
            var inputEndpoints = InputDataAvailabilityEndpoints(i_fil);

            ////2
            var client = new HttpClient();

            ////3
            foreach (var inputEndpoint in inputEndpoints)
            {
                var endpoint = inputEndpoint.ConfigureEndpoint();

                Console.WriteLine($"\nCollect: {endpoint}");

                var uri = new Uri(endpoint);
                var request = new HttpRequestMessage() { RequestUri = uri };
                var response = client.Send(request);
                var result = response.Content.ReadAsStringAsync();

                Console.WriteLine(result.Result);
                Console.WriteLine("\n\n\n");
            }            
        }

        public static void CollectTradeData()
        {
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

            ////1
            var inputEndpoints = InputTradeDataEndpoints(i_fil);

            ////2
            var client = new HttpClient();

            ////3
            foreach (var inputEndpoint in inputEndpoints)
            {
                var endpoint = inputEndpoint.ConfigureEndpoint();

                Console.WriteLine($"\nCollect: {endpoint}");

                var uri = new Uri(endpoint);
                var request = new HttpRequestMessage() { RequestUri = uri };
                var response = client.Send(request);
                var result = response.Content.ReadAsStringAsync();

                Console.WriteLine(result.Result);
                Console.WriteLine("\n\n\n");
            }
        }

        private static DataAvailabilityEnpoint[] InputDataAvailabilityEndpoints(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, encoding: Encoding.UTF8);

            var endpoints = new List<DataAvailabilityEnpoint>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var endpoint = new DataAvailabilityEnpoint();

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

        private static TradeDataEndpoint[] InputTradeDataEndpoints(string path)
        {
            var dataFrame = DataFrame.LoadCsv(path, header: true, encoding: Encoding.UTF8);

            var endpoints = new List<TradeDataEndpoint>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var endpoint = new TradeDataEndpoint();

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
    }
}

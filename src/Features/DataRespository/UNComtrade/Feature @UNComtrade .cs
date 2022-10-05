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
using System.Text.Json;

using Microsoft.Data.Analysis;

using DxMLEngine.Attributes;
using DxMLEngine.Functions;
using DxMLEngine.Features.GooglePatents;

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
            var dataList = new List<DataAvailability>();
            for (int i = 0; i < endpoints.Length; i++)
            {
                var endpoint = endpoints[i];
                Console.WriteLine($"\nCollect: {endpoint.AvailabilityEndpoint}");

                var uri = new Uri(endpoint.AvailabilityEndpoint);
                var request = new HttpRequestMessage() { RequestUri = uri };
                var response = client.Send(request);
                endpoint.Response = response.Content.ReadAsStringAsync().Result;

                var dataAvailability = ExtractDataAvailability(endpoint);
                if (dataAvailability != null) 
                { 
                    dataList.Add(dataAvailability);

                    //OutputOneTXT(outDir, $"DataAvailability{i+1}", dataAvailability);
                    //OutputOneCSV(outDir, $"DataAvailability{i+1}", dataAvailability);
                    //OutputOneJSON(outDir, $"DataAvailability{i+1}", dataAvailability);
                }
            }
            //OutputAllTXT(outDir, "DataAvailability", dataList.ToArray());
            //OutputAllCSV(outDir, "DataAvailability", dataList.ToArray());
            //OutputAllJSON(outDir, "DataAvailability", dataList.ToArray());
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
            var dataList = new List<TradeData>();
            for (int i = 0; i < endpoints.Length; i++)
            {
                var endpoint = endpoints[i];
                Console.WriteLine($"\nCollect: {endpoint.TradeDataEndpoint}");

                var uri = new Uri(endpoint.AvailabilityEndpoint);
                var request = new HttpRequestMessage() { RequestUri = uri };
                var response = client.Send(request);
                endpoint.Response = response.Content.ReadAsStringAsync().Result;

                var tradeData = ExtractTradeData(endpoint);
                if (tradeData != null)
                {
                    dataList.Add(tradeData);

                    //OutputOneTXT(outDir, $"TradeData{i+1}", tradeData);
                    //OutputOneCSV(outDir, $"TradeData{i+1}", tradeData);
                    //OutputOneJSON(outDir, $"TradeData{i+1}", tradeData);
                }
            }
            //OutputAllTXT(outDir, "DataAvailability", dataList.ToArray());
            //OutputAllCSV(outDir, "DataAvailability", dataList.ToArray());
            //OutputAllJSON(outDir, "DataAvailability", dataList.ToArray());
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
    
        private static void OutputEndpoint(string location, string fileName, Endpoint endpoint)
        {
            var path = $"{location}\\Datason @{fileName} #-------------- .json";
            File.WriteAllText(path, endpoint.Response, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
            File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
        }

        private static void OutputOneTXT(string location, string fileName, object entity)
        {
            if (entity.GetType() == typeof(DataAvailability))
            {
                var dataAvailability = (DataAvailability)entity;
                var lines = new string?[]
                {
                    dataAvailability.type,
                    dataAvailability.freq,
                    dataAvailability.px,
                    dataAvailability.ps,
                    dataAvailability.r,
                    dataAvailability.rDesc,
                    Convert.ToString(dataAvailability.TotalRecords),
                    Convert.ToString(dataAvailability.isOriginal),
                    Convert.ToString(dataAvailability.publicationDate),
                    Convert.ToString(dataAvailability.isPartnerDetail),
                };

                var path = $"{location}\\Datadoc @{fileName} #-------------- .txt";
                File.WriteAllLines(path, lines!, encoding: Encoding.UTF8);

                var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
                File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
            }

            if (entity.GetType() == typeof(TradeData))
            {
                var tradeData = (TradeData)entity;
                var lines = new string?[]
                {
                    //tradeData.validation,
                    //tradeData.dataset,
                };

                var path = $"{location}\\Datadoc @{fileName} #-------------- .txt";
                File.WriteAllLines(path, lines!, encoding: Encoding.UTF8);

                var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
                File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
            }
        }

        private static void OutputOneCSV(string location, string fileName, object entity)
        {
            if (entity.GetType() == typeof(DataAvailability))
            {
                throw new NotImplementedException();
            }

            if (entity.GetType() == typeof(TradeData))
            {
                throw new NotImplementedException();
            }

        }

        private static void OutputOneJSON(string location, string fileName, object entity)
        {
            if (entity.GetType() == typeof(DataAvailability))
            {
                var dataAvailability = (DataAvailability)entity;
                var options = new JsonSerializerOptions() { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(dataAvailability, options);

                Console.WriteLine($"{dataAvailability.publicationDate}");

                var path = $"{location}\\Datason @{fileName} #-------------- .json";
                File.WriteAllText(path, jsonString, encoding: Encoding.UTF8);

                var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
                File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);                
            }

            if (entity.GetType() == typeof(TradeData))
            {
                var tradeData = (TradeData)entity;
                var options = new JsonSerializerOptions() { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(tradeData, options);

                Console.WriteLine($"{tradeData.validation}");

                var path = $"{location}\\Datason @{fileName} #-------------- .json";
                File.WriteAllText(path, jsonString, encoding: Encoding.UTF8);

                var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
                File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
            }
        }

        private static void OutputAllTXT(string location, string fileName, object entity)
        {
            if (entity.GetType() == typeof(DataAvailability[]))
            {
                throw new NotImplementedException();
            }

            if (entity.GetType() == typeof(TradeData[]))
            {
                throw new NotImplementedException();
            }
        }

        private static void OutputAllCSV(string location, string fileName, object entity)
        {
            if (entity.GetType() == typeof(DataAvailability[]))
            {
                var dataAvailability = (DataAvailability[])entity;
                var dataFrame = new DataFrame(new List<DataFrameColumn>()
                    {
                        new StringDataFrameColumn("Trade Type"),
                        new StringDataFrameColumn("Frequency"),
                        new StringDataFrameColumn("Reporting Area"),
                        new StringDataFrameColumn("Time Period"),
                        new StringDataFrameColumn("Classification"),
                });

                foreach (var data in dataAvailability)
                {
                    var dataRow = new List<KeyValuePair<string, object?>>()
                    {
                        new KeyValuePair<string, object?>("Trade Type", data.type),
                        new KeyValuePair<string, object?>("Frequency", data.freq),
                        new KeyValuePair<string, object?>("Reporting Area", data.r),
                        new KeyValuePair<string, object?>("Time Period", data.ps),
                        new KeyValuePair<string, object?>("Classification", data.px),
                    };

                    Console.WriteLine($"{data.type}");
                    dataFrame.Append(dataRow, inPlace: true);
                }

                var path = $"{location}\\Dataset @{fileName} #-------------- .csv";
                DataFrame.WriteCsv(dataFrame, path, header: true, encoding: Encoding.UTF8);

                var timestamp = File.GetCreationTime(path).ToString("yyyyMMddHHmmss");
                File.Move(path, path.Replace("#--------------", $"#{timestamp}"), overwrite: true);
            }

            if (entity.GetType() == typeof(TradeData[]))
            {
                throw new NotImplementedException();
            }
        }

        private static void OutputAllJSON(string location, string fileName, object entity)
        {
            if (entity.GetType() == typeof(DataAvailability[]))
            {
                throw new NotImplementedException();
            }

            if (entity.GetType() == typeof(TradeData[]))
            {
                throw new NotImplementedException();
            }
        }

        #endregion INPUT OUTPUT

        #region DATA EXTRACTION

        private static DataAvailability? ExtractDataAvailability(Endpoint endpoint)
        {
            if (endpoint.Response == null)
                throw new ArgumentNullException("endpoint.Response == null");

            var options = new JsonSerializerOptions() { WriteIndented = true };
            var dataAvailability = JsonSerializer.Deserialize<DataAvailability>(endpoint.Response, options);

            return dataAvailability;
        }

        private static TradeData? ExtractTradeData(Endpoint endpoint)
        {
            if (endpoint.Response == null)
                throw new ArgumentNullException("endpoint.Response == null");

            var options = new JsonSerializerOptions() { WriteIndented = true };
            var tradeData = JsonSerializer.Deserialize<TradeData>(endpoint.Response, options);

            return tradeData;
        }

        #endregion DATA EXTRACTION
    }
}

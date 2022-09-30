using DxMLEngine.Attributes;
using DxMLEngine.Features.YahooFinance;
using DxMLEngine.Features.UNComtrade;

using System.Text.Json;

namespace DxMLEngine.Features
{
    [Feature]
    internal class Sandbox
    {
        public static void DeserializeSearchAPI()
        {
            /// https://query1.finance.yahoo.com/v1/finance/search?q={query}

            var endpoint = new YahooFinance.Endpoint();
            endpoint.Query = "Microsoft";

            Console.WriteLine($"\nCollect: {endpoint.SearchEndpoint}");
            
            var uri = new Uri(endpoint.SearchEndpoint);
            var client = new HttpClient();
            var request = new HttpRequestMessage() { RequestUri = uri };
            endpoint.Response = client.Send(request).Content.ReadAsStringAsync().Result;

            var search = JsonSerializer.Deserialize<Search>(endpoint.Response);

            if (search == null)
                throw new NullReferenceException("search == null");

            Console.WriteLine(search?.explains);
            Console.WriteLine(search?.count);
            Console.WriteLine(search?.quotes);
            Console.WriteLine(search?.news);
            Console.WriteLine(search?.nav);
            Console.WriteLine(search?.lists);

            if (search!.quotes != null)
            {
                foreach (var quote in search.quotes)
                {
                    Console.WriteLine(quote.Exchange);
                    Console.WriteLine(quote.shortname);
                    Console.WriteLine(quote.quoteType);
                    Console.WriteLine(quote.symbol);
                    Console.WriteLine(quote.index);
                    Console.WriteLine(quote.score);
                    Console.WriteLine(quote.typeDisp);
                    Console.WriteLine(quote.longname);
                    Console.WriteLine(quote.exchDisp);
                    Console.WriteLine(quote.sector);
                    Console.WriteLine(quote.industry);
                    Console.WriteLine(quote.dispSecIndFlag);
                    Console.WriteLine(quote.isYahooFinance); 
                }   
            }
        }

        public static void DeserializeOptionsAPI()
        {
            /// https://query1.finance.yahoo.com/v7/finance/options/{symbol}
            
            var endpoint = new YahooFinance.Endpoint();
            endpoint.Symbol = "MSFT";

            Console.WriteLine($"\nCollect: {endpoint.OptionsEndpoint}");

            var uri = new Uri(endpoint.OptionsEndpoint);
            var client = new HttpClient();
            var request = new HttpRequestMessage() { RequestUri = uri };
            var response = client.Send(request);
            var jsonString = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine(jsonString);
            Console.WriteLine();

            var options = JsonSerializer.Deserialize<Options>(jsonString);

            if (options == null)
                throw new NullReferenceException("options == null");

            Console.WriteLine(options.optionChain);
            Console.WriteLine();

            if (options!.optionChain!.result != null)
            {
                foreach (var result in options.optionChain.result)
                {
                    Console.WriteLine(result.underlyingSymbol);
                    Console.WriteLine(result.expirationDates);
                    Console.WriteLine(result.strikes);
                    Console.WriteLine(result.hasMiniOptions);
                }
            }
        }

        public static void DeserializeSummaryAPI()
        {
            /// https://query1.finance.yahoo.com/v10/finance/quoteSummary/{symbol}?modules={modules}

            var endpoint = new YahooFinance.Endpoint();
            endpoint.Symbol = "MSFT";
            endpoint.Modules = new string[3] { "assetProfile", "defaultKeyStatistics", "recommendationTrend" };

            Console.WriteLine($"\nCollect: {endpoint.SummaryEndpoint}");

            var uri = new Uri(endpoint.SummaryEndpoint);
            var client = new HttpClient();
            var request = new HttpRequestMessage() { RequestUri = uri };
            var response = client.Send(request);
            var jsonString = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine(jsonString);
            Console.WriteLine();

            var summary = JsonSerializer.Deserialize<Summary>(jsonString);

            if (summary == null)
                throw new NullReferenceException("summary == null");

            Console.WriteLine(summary.quoteSummary);
            Console.WriteLine();

            if (summary!.quoteSummary!.result != null)
            {
                foreach (var result in summary.quoteSummary.result)
                {
                    Console.WriteLine(result.assetProfile);
                    Console.WriteLine(result.defaultKeyStatistics);
                    Console.WriteLine(result.recommendationTrend);
                    Console.WriteLine(result.earnings);
                    Console.WriteLine(result.earningsTrend);
                }
            }
        }

        public static void DeserializeHistoryAPI()
        {
            /// https://query1.finance.yahoo.com/v8/finance/chart/{symbol}{parameters}

            var endpoint = new YahooFinance.Endpoint();
            endpoint.Symbol = "MSFT";
            endpoint.Interval = "1mo";
            endpoint.Range = "10y";
            endpoint.Prepost = true; 
            endpoint.Events = true; 
            endpoint.Close = true; 

            Console.WriteLine($"\nCollect: {endpoint.HistoryEndpoint}");

            var uri = new Uri(endpoint.HistoryEndpoint);
            var client = new HttpClient();
            var request = new HttpRequestMessage() { RequestUri = uri };
            var response = client.Send(request);
            var jsonString = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine(jsonString);
            Console.WriteLine();

            var history = JsonSerializer.Deserialize<History>(jsonString);

            if (history == null)
                throw new NullReferenceException("history == null");

            Console.WriteLine(history.chart);
            Console.WriteLine();

            if (history!.chart!.result != null)
            {
                foreach (var result in history.chart.result)
                {
                    Console.WriteLine(result.timestamp);
                    Console.WriteLine(result.indicators);
                    Console.WriteLine(result.meta);
                }
            }
        }

        public static void DeserializeAvailabilityAPI()
        {
            var endpoint = "https://comtrade.un.org/api/refs/da/view?type=C&freq=A&r=826";
            Console.WriteLine($"\nCollect: {endpoint}");

            var uri = new Uri(endpoint);
            var client = new HttpClient();
            var request = new HttpRequestMessage() { RequestUri = uri };
            var response = client.Send(request);
            var jsonString = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine(jsonString);
            Console.WriteLine();

            var results = JsonSerializer.Deserialize<DataAvailability[]>(jsonString);
            foreach (var availability in results!)
            {
                Console.WriteLine(availability.type);
                Console.WriteLine(availability.freq);
                Console.WriteLine(availability.ps);
                Console.WriteLine(availability.px);
                Console.WriteLine(availability.r);
                Console.WriteLine(availability.rDesc);
                Console.WriteLine(availability.TotalRecords);
                Console.WriteLine(availability.isOriginal);
                Console.WriteLine(availability.publicationDate);
                Console.WriteLine(availability.isPartnerDetail);
            }
        }

        public static void DeserializeTradeDAtaAPI()
        {
            var endpoint = "https://comtrade.un.org/api/get?max=50000&type=C&freq=A&px=HS&ps=2013&r=826&p=0&rg=all&cc=AG2&fmt=json";
            Console.WriteLine($"\nCollect: {endpoint}");

            var uri = new Uri(endpoint);
            var client = new HttpClient();
            var request = new HttpRequestMessage() { RequestUri = uri };
            var response = client.Send(request);
            var jsonString = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine(jsonString);
            Console.WriteLine();

            var results = JsonSerializer.Deserialize<TradeData>(jsonString);
            Console.WriteLine(results!.validation);
            foreach (var dataset in results!.dataset!)
            {
                Console.WriteLine(dataset.IsLeaf);
                Console.WriteLine(dataset.pfCode);
                Console.WriteLine(dataset.qtCode);
                Console.WriteLine(dataset.rgCode);
                Console.WriteLine(dataset.cstCode);
            }
        }
    }
}

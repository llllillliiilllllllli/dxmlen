using DxMLEngine.Attributes;
using DxMLEngine.Features.YahooFinance;
using DxMLEngine.Features.UNComtrade;
using DxMLEngine.Features.ScienceDirect;
using System.Text;
using System.Text.Json;

using Microsoft.ML.Probabilistic;
using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Probabilistic.Models;
using Range = Microsoft.ML.Probabilistic.Models.Range;
using Microsoft.ML.Probabilistic.Collections;
using CodeGenerator;

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

            var search = JsonSerializer.Deserialize<YahooFinance.Search>(endpoint.Response);

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

        public static void DeserializeTradeDataAPI()
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

        public static void InquireParameters(string inFile, string inDir, int outInt, float outFloat)
        {
            Console.WriteLine(inFile);
            Console.WriteLine(inDir);
            Console.WriteLine(outInt);
            Console.WriteLine(outFloat);
        }

        public static void FindRSquaredCoefficient()
        {
            var actualValues = new float[] 
            { 
                15.000f,
                14.000f,
                18.000f,
                14.000f,
                18.000f,
                15.000f,
                12.000f,
                15.000f,
                12.000f,
                12.000f,
                17.000f,
                15.000f,
                14.000f,
                15.000f,
                16.000f,
                14.000f,
                18.000f,
                19.000f,
                17.000f,
                17.000f,
            };

            var predictedValues = new float[] 
            {
                15.361f,
                14.080f,
                18.429f,
                14.888f,
                18.061f,
                15.111f,
                12.735f,
                15.900f,
                12.094f,
                12.441f,
                17.422f,
                15.956f,
                14.522f,
                15.938f,
                16.462f,
                14.864f,
                18.073f,
                19.355f,
                17.541f,
                17.064f,
            };

            var norminator = actualValues.Zip(predictedValues, (x, y) => (x - actualValues.Average()) * (y - actualValues.Average())).Sum();
            var denominator = Math.Sqrt(actualValues.Sum(x => Math.Pow(x - actualValues.Average(), 2)) * predictedValues.Sum(y => Math.Pow(y - predictedValues.Average(), 2)));
            var RSQ = norminator / denominator;

            Console.WriteLine($"R-Squared : {RSQ}");
        }

        public static void InferPlayerSkills()
        {
            var winnerData = new[] { 0, 0, 0, 1, 3, 4 };
            var loserData = new[] { 1, 3, 4, 2, 1, 2 };

            var game = new Range(winnerData.Length);
            var player = new Range(winnerData.Concat(loserData).Max() + 1);
            var playerSkills = Variable.Array<double>(player);
            playerSkills[player] = Variable.GaussianFromMeanAndVariance(6, 9).ForEach(player);

            var winners = Variable.Array<int>(game);
            var losers = Variable.Array<int>(game);

            using (Variable.ForEach(game))
            {
                var winnerPerformance = Variable.GaussianFromMeanAndVariance(playerSkills[winners[game]], 1.0);
                var loserPerformance = Variable.GaussianFromMeanAndVariance(playerSkills[losers[game]], 1.0);

                Variable.ConstrainTrue(winnerPerformance > loserPerformance);
            }

            winners.ObservedValue = winnerData;
            losers.ObservedValue = loserData;

            var inferenceEngine = new InferenceEngine();
            var inferredSkills = inferenceEngine.Infer<Gaussian[]>(playerSkills);

            var orderedPlayerSkills =
                from inferredSkill in inferredSkills
                let playerSkill = new {Player = inferredSkills.IndexOf(inferredSkill), Skill = inferredSkill }
                orderby playerSkill.Skill.GetMean() descending
                select playerSkill;

            foreach (var playerSkill in orderedPlayerSkills)
                Console.WriteLine($"Player {playerSkill.Player} skill: {playerSkill.Skill}");
        }

        public static void GenerateMyClassSource()
        {
            var sourceCode = Generator.GenerateMyClass("CustomizedName");
            Console.WriteLine(sourceCode);  
        }
    }
}

namespace MyNamespace
{
    public class MyClass
    {
        #region Fields
        public int Field1; 
        public int Field2; 
        public int Field3;
        #endregion Fields

        #region Properties
        public int Property1 { set; get; }
        public int Property2 { set; get; }
        public int Property3 { set; get; }
        #endregion Properties

        #region Methods
        public string Method1() { return "This is Method1"; }
        public string Method2() { return "This is Method2"; }
        public string Method3() { return "This is Method3"; }
        #endregion Methods

        #region Delegates
        public delegate double Delegate1(float[] xs, float[] ys);
        public delegate double Delegate2(float[] xs, float[] ys);
        public delegate double Delegate3(float[] xs, float[] ys);
        #endregion Delegates

        #region Actions
        public delegate void Action1<in T>(T arg);
        public delegate void Action2<in T>(T arg);
        public delegate void Action3<in T>(T arg);
        #endregion Actions

        #region Functions
        public delegate TResult Func1<in T, out TResult>(T arg);
        public delegate TResult Func2<in T, out TResult>(T arg);
        public delegate TResult Func3<in T, out TResult>(T arg);
        #endregion Functions

        #region Events        
        public event EventHandler? EventHandler1;
        public event EventHandler? EventHandler2;
        public event EventHandler? EventHandler3;

        protected virtual void RaiseEvent1() { EventHandler1?.Invoke(this, new MyEventArgs(true, true, true, true)); }
        protected virtual void RaiseEvent2() { EventHandler2?.Invoke(this, new MyEventArgs(true, true, true, true)); }
        protected virtual void RaiseEvent3() { EventHandler3?.Invoke(this, new MyEventArgs(true, true, true, true)); }
        #endregion Events
    }

    public class MyEventArgs : EventArgs
    {
        public bool ItemCreated { set; get; } 
        public bool ItemLoaded { set; get; } 
        public bool ItemUpdated { set; get; } 
        public bool ItemDeleted { set; get; } 
        public MyEventArgs(bool created, bool loaded, bool updated, bool deleted) 
        {
            ItemCreated = created;
            ItemLoaded = loaded;
            ItemUpdated = updated;
            ItemDeleted = deleted; 
        }
    }
}

namespace CodeGenerator
{
    public static class Generator
    {
        public static string GenerateMyClass(string className)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"namespace Namespace\r\n");
            stringBuilder.Append($"{{\r\n");
            stringBuilder.Append($"    public class {className}\r\n");
            stringBuilder.Append($"    {{\r\n");
            stringBuilder.Append($"        #region Fields\r\n");
            stringBuilder.Append($"        public int Field1; \r\n");
            stringBuilder.Append($"        public int Field2; \r\n");
            stringBuilder.Append($"        public int Field3; \r\n");
            stringBuilder.Append($"        #endregion Fields\r\n\r\n");
            stringBuilder.Append($"        #region Properties\r\n");
            stringBuilder.Append($"        public int Property1 {{ set; get; }}\r\n");
            stringBuilder.Append($"        public int Property2 {{ set; get; }}\r\n");
            stringBuilder.Append($"        public int Property3 {{ set; get; }}\r\n");
            stringBuilder.Append($"        #endregion Properties\r\n\r\n");
            stringBuilder.Append($"        #region Methods\r\n");
            stringBuilder.Append($"        public string Method1() {{ return \"This is Method1\"; }}\r\n");
            stringBuilder.Append($"        public string Method2() {{ return \"This is Method2\"; }}\r\n");
            stringBuilder.Append($"        public string Method3() {{ return \"This is Method3\"; }}\r\n");
            stringBuilder.Append($"        #endregion Methods\r\n\r\n");
            stringBuilder.Append($"        #region Delegates\r\n");
            stringBuilder.Append($"        public delegate double Delegate1(float[] xs, float[] ys);\r\n");
            stringBuilder.Append($"        public delegate double Delegate2(float[] xs, float[] ys);\r\n");
            stringBuilder.Append($"        public delegate double Delegate3(float[] xs, float[] ys);\r\n");
            stringBuilder.Append($"        #endregion Delegates\r\n\r\n");
            stringBuilder.Append($"        #region Actions\r\n");
            stringBuilder.Append($"        public delegate void Action1<in T>(T arg);\r\n");
            stringBuilder.Append($"        public delegate void Action2<in T>(T arg);\r\n");
            stringBuilder.Append($"        public delegate void Action3<in T>(T arg);\r\n");
            stringBuilder.Append($"        #endregion Actions\r\n\r\n");
            stringBuilder.Append($"        #region Functions\r\n");
            stringBuilder.Append($"        public delegate TResult Func1<in T, out TResult>(T arg);\r\n");
            stringBuilder.Append($"        public delegate TResult Func2<in T, out TResult>(T arg);\r\n");
            stringBuilder.Append($"        public delegate TResult Func3<in T, out TResult>(T arg);\r\n");
            stringBuilder.Append($"        #endregion Functions\r\n\r\n");
            stringBuilder.Append($"        #region Events\r\n");
            stringBuilder.Append($"        public event EventHandler? EventHandler1;\r\n");
            stringBuilder.Append($"        public event EventHandler? EventHandler2;\r\n");
            stringBuilder.Append($"        public event EventHandler? EventHandler3;\r\n\r\n");
            stringBuilder.Append($"        protected virtual void RaiseEvent1() {{ EventHandler1?.Invoke(this, new MyEventArgs(true, true, true, true)); }}\r\n");
            stringBuilder.Append($"        protected virtual void RaiseEvent2() {{ EventHandler2?.Invoke(this, new MyEventArgs(true, true, true, true)); }}\r\n");
            stringBuilder.Append($"        protected virtual void RaiseEvent3() {{ EventHandler3?.Invoke(this, new MyEventArgs(true, true, true, true)); }}\r\n");
            stringBuilder.Append($"        #endregion Events\r\n");
            stringBuilder.Append($"    }}\r\n\r\n");
            stringBuilder.Append($"    public class MyEventArgs : EventArgs\r\n");
            stringBuilder.Append($"    {{\r\n");
            stringBuilder.Append($"        public bool ItemCreated {{ set; get; }}\r\n");
            stringBuilder.Append($"        public bool ItemLoaded {{ set; get; }}\r\n");
            stringBuilder.Append($"        public bool ItemUpdated {{ set; get; }}\r\n");
            stringBuilder.Append($"        public bool ItemDeleted {{ set; get; }}\r\n");
            stringBuilder.Append($"        public MyEventArgs(bool created, bool loaded, bool updated, bool deleted)\r\n");
            stringBuilder.Append($"        {{\r\n");
            stringBuilder.Append($"            ItemCreated = created;\r\n");
            stringBuilder.Append($"            ItemLoaded = loaded;\r\n");
            stringBuilder.Append($"            ItemUpdated = updated;\r\n");
            stringBuilder.Append($"            ItemDeleted = deleted;\r\n");
            stringBuilder.Append($"        }}\r\n");
            stringBuilder.Append($"    }}\r\n");
            stringBuilder.Append($"}}\r\n");

            return stringBuilder.ToString();
        }
    }
}
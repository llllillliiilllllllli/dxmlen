/// ====================================================================================
/// Data availability enpoint:
///     http://comtrade.un.org/api//refs/da/view?{parameters}
///     
/// Parameters:     
///     type            : trade data type (default = any)
///         type=C      : commodities 
///         type=S      : services 
///     
///     freq            : data frequency (default = any)
///         freq=A      : annual
///         freq=M      : monthly        
///     
///     r               : reporting area (default = any)    
///         r=id        : country id
///     
///     ps              : time period (default = any)
///         ps=2015     : year
///         ps=201512   : month
///         
///     px              : classification (default = any)
///         ps=HS       : HS classes
///         ps=BEC      : BEC classes
///         ps=EB02     : services classes
///         
///     token           : authorization
///         token=code  : API key
/// ====================================================================================

using DxMLEngine.Features.GooglePatents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.UNComtrade
{
    internal class Endpoint
    {
        public const string EP_AVAILABILITY = "http://comtrade.un.org/api//refs/da/view?{parameters}";
        public const string EP_UNTRADE_DATA = "http://comtrade.un.org/api/get?{parameters}";

        public string? TradeType { set; get; }
        public string? Frequency { set; get; }
        public string? ReportingArea { set; get; }
        public string? TimePeriod { set; get; }
        public string? Classification { set; get; }
        public string? ClassificationCode { set; get; }
        public string? PartnerArea { set; get; }
        public string? TradeFlow { set; get; }
        public string? OutputFormat { set; get; }
        public string? MaxRecords { set; get; }
        public string? HeadingStyle { set; get; }
        public string? IMTS { set; get; }
        public string? ApiToken { set; get; }

        public Dictionary<object, object?> Parameters { set; get; }

        public Endpoint()
        {
            Parameters = new Dictionary<object, object?>()
            {
                { "type", null },
                { "freq", null },
                { "r", null },
                { "ps", null },
                { "px", null },
                { "cc", null },
                { "p", null },
                { "rg", null },
                { "fmt", null },
                { "max", null },
                { "head", null },
                { "IMTS", null },
                { "token", null },
            };
        }

        public string ConfigureEndpoint()
        {
            Parameters["type"] = TradeType;
            Parameters["freq"] = Frequency;
            Parameters["r"] = ReportingArea;
            Parameters["ps"] = TimePeriod;
            Parameters["px"] = Classification;
            Parameters["token"] = ApiToken;

            var parameters = "";
            if (Parameters["type"] != null) parameters += $"&type={Parameters["type"]}";
            if (Parameters["freq"] != null) parameters += $"&freq={Parameters["freq"]}";
            if (Parameters["r"] != null) parameters += $"&r={Parameters["r"]}";
            if (Parameters["ps"] != null) parameters += $"&ps={Parameters["ps"]}";
            if (Parameters["px"] != null) parameters += $"&px={Parameters["px"]}";
            if (Parameters["token"] != null) parameters += $"&token={Parameters["token"]}";

            return EP_AVAILABILITY.Replace("{parameters}", parameters).Replace("?&", "?");
        }

        public string ConfigureTradeDataEndpoint()
        {
            Parameters["type"] = TradeType;
            Parameters["freq"] = Frequency;
            Parameters["r"] = ReportingArea;
            Parameters["ps"] = TimePeriod;
            Parameters["px"] = Classification;
            Parameters["cc"] = ClassificationCode;
            Parameters["p"] = PartnerArea;
            Parameters["rg"] = TradeFlow;
            Parameters["fmt"] = OutputFormat;
            Parameters["max"] = MaxRecords;
            Parameters["head"] = HeadingStyle;
            Parameters["IMTS"] = IMTS;
            Parameters["token"] = ApiToken;

            var parameters = "";
            if (Parameters["type"] != null) parameters += $"&type={Parameters["type"]}";
            if (Parameters["freq"] != null) parameters += $"&freq={Parameters["freq"]}";
            if (Parameters["r"] != null) parameters += $"&r={Parameters["r"]}";
            if (Parameters["ps"] != null) parameters += $"&ps={Parameters["ps"]}";
            if (Parameters["px"] != null) parameters += $"&px={Parameters["px"]}";
            if (Parameters["cc"] != null) parameters += $"&cc={Parameters["cc"]}";
            if (Parameters["p"] != null) parameters += $"&p={Parameters["p"]}";
            if (Parameters["rg"] != null) parameters += $"&rg={Parameters["rg"]}";
            if (Parameters["fmt"] != null) parameters += $"&fmt={Parameters["fmt"]}";
            if (Parameters["max"] != null) parameters += $"&max={Parameters["max"]}";
            if (Parameters["head"] != null) parameters += $"&head={Parameters["head"]}";
            if (Parameters["IMTS"] != null) parameters += $"&IMTS={Parameters["IMTS"]}";
            if (Parameters["token"] != null) parameters += $"&token={Parameters["token"]}";

            return EP_UNTRADE_DATA.Replace("{parameters}", parameters).Replace("?&", "?"); ;
        }
    }
}

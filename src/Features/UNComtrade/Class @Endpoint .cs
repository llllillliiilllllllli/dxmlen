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
        public string BaseEndpoint { get { return "http://comtrade.un.org/api"; } } 
        public string AvailabilityEndpoint { get { return ConfigureAvailabilityEndpoint(); } } 
        public string TradeDataEndpoint { get { return ConfigureTradeDataEndpoint(); } } 

        public string? Id { get; set; }
        public string? Reponse { set; get; }

        public const string EP_AVAILABILITY = "http://comtrade.un.org/api/refs/da/view?{parameters}";
        public const string EP_UNTRADE_DATA = "http://comtrade.un.org/api/get?{parameters}";

        public Dictionary<object, object?> AvailabilityParameters { set; get; }
        public Dictionary<object, object?> TradeDataParameters { set; get; }
        
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

        public Endpoint()
        {
            AvailabilityParameters = new Dictionary<object, object?>()
            {
                { "type", null },
                { "freq", null },
                { "r", null },
                { "ps", null },
                { "px", null },
                { "token", null },
            };

            TradeDataParameters = new Dictionary<object, object?>()
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

        public string ConfigureAvailabilityEndpoint()
        {
            AvailabilityParameters["type"] = TradeType;
            AvailabilityParameters["freq"] = Frequency;
            AvailabilityParameters["r"] = ReportingArea;
            AvailabilityParameters["ps"] = TimePeriod;
            AvailabilityParameters["px"] = Classification;
            AvailabilityParameters["token"] = ApiToken;

            var parameters = "";
            if (AvailabilityParameters["type"] != null) parameters += $"&type={AvailabilityParameters["type"]}";
            if (AvailabilityParameters["freq"] != null) parameters += $"&freq={AvailabilityParameters["freq"]}";
            if (AvailabilityParameters["r"] != null) parameters += $"&r={AvailabilityParameters["r"]}";
            if (AvailabilityParameters["ps"] != null) parameters += $"&ps={AvailabilityParameters["ps"]}";
            if (AvailabilityParameters["px"] != null) parameters += $"&px={AvailabilityParameters["px"]}";
            if (AvailabilityParameters["token"] != null) parameters += $"&token={AvailabilityParameters["token"]}";

            return EP_AVAILABILITY.Replace("{parameters}", parameters).Replace("?&", "?");
        }

        public string ConfigureTradeDataEndpoint()
        {
            TradeDataParameters["type"] = TradeType;
            TradeDataParameters["freq"] = Frequency;
            TradeDataParameters["r"] = ReportingArea;
            TradeDataParameters["ps"] = TimePeriod;
            TradeDataParameters["px"] = Classification;
            TradeDataParameters["cc"] = ClassificationCode;
            TradeDataParameters["p"] = PartnerArea;
            TradeDataParameters["rg"] = TradeFlow;
            TradeDataParameters["fmt"] = OutputFormat;
            TradeDataParameters["max"] = MaxRecords;
            TradeDataParameters["head"] = HeadingStyle;
            TradeDataParameters["IMTS"] = IMTS;
            TradeDataParameters["token"] = ApiToken;

            var parameters = "";
            if (TradeDataParameters["type"] != null) parameters += $"&type={TradeDataParameters["type"]}";
            if (TradeDataParameters["freq"] != null) parameters += $"&freq={TradeDataParameters["freq"]}";
            if (TradeDataParameters["r"] != null) parameters += $"&r={TradeDataParameters["r"]}";
            if (TradeDataParameters["ps"] != null) parameters += $"&ps={TradeDataParameters["ps"]}";
            if (TradeDataParameters["px"] != null) parameters += $"&px={TradeDataParameters["px"]}";
            if (TradeDataParameters["cc"] != null) parameters += $"&cc={TradeDataParameters["cc"]}";
            if (TradeDataParameters["p"] != null) parameters += $"&p={TradeDataParameters["p"]}";
            if (TradeDataParameters["rg"] != null) parameters += $"&rg={TradeDataParameters["rg"]}";
            if (TradeDataParameters["fmt"] != null) parameters += $"&fmt={TradeDataParameters["fmt"]}";
            if (TradeDataParameters["max"] != null) parameters += $"&max={TradeDataParameters["max"]}";
            if (TradeDataParameters["head"] != null) parameters += $"&head={TradeDataParameters["head"]}";
            if (TradeDataParameters["IMTS"] != null) parameters += $"&IMTS={TradeDataParameters["IMTS"]}";
            if (TradeDataParameters["token"] != null) parameters += $"&token={TradeDataParameters["token"]}";

            return EP_UNTRADE_DATA.Replace("{parameters}", parameters).Replace("?&", "?"); ;
        }
    }
}

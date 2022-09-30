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
        public string? Response { set; get; }

        private const string EP_AVAILABILITY = "http://comtrade.un.org/api/refs/da/view?{parameters}";
        private const string EP_UNTRADE_DATA = "http://comtrade.un.org/api/get?{parameters}";
        
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
        }

        public string ConfigureAvailabilityEndpoint()
        {
            var parameters = "";
            if (TradeType != null) parameters += $"&type={TradeType}";
            if (Frequency != null) parameters += $"&freq={Frequency}";
            if (ReportingArea != null) parameters += $"&r={ReportingArea}";
            if (TimePeriod != null) parameters += $"&ps={TimePeriod}";
            if (Classification != null) parameters += $"&px={Classification}";
            if (ApiToken != null) parameters += $"&token={ApiToken}";

            return EP_AVAILABILITY.Replace("{parameters}", parameters).Replace("?&", "?");
        }

        public string ConfigureTradeDataEndpoint()
        {
            var parameters = "";
            if (TradeType != null) parameters += $"&type={TradeType}";
            if (Frequency != null) parameters += $"&freq={Frequency}";
            if (ReportingArea != null) parameters += $"&r={ReportingArea}";
            if (TimePeriod != null) parameters += $"&ps={TimePeriod}";
            if (Classification != null) parameters += $"&px={Classification}";
            if (ClassificationCode != null) parameters += $"&px={Classification}";
            if (PartnerArea != null) parameters += $"&px={Classification}";
            if (TradeFlow != null) parameters += $"&px={Classification}";
            if (OutputFormat != null) parameters += $"&px={Classification}";
            if (MaxRecords != null) parameters += $"&px={Classification}";
            if (HeadingStyle != null) parameters += $"&px={Classification}";
            if (IMTS != null) parameters += $"&px={Classification}";          
            if (ApiToken != null) parameters += $"&token={ApiToken}";

            return EP_UNTRADE_DATA.Replace("{parameters}", parameters).Replace("?&", "?"); ;
        }
    }
}

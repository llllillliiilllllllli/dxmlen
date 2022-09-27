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
    internal class DataAvailabilityEnpoint
    {
        public const string EP_BASE = "http://comtrade.un.org/api//refs/da/view?{parameters}";

        public string? TradeType { set; get; }
        public string? Frequency { set; get; }
        public string? ReportingArea { set; get; }
        public string? TimePeriod { set; get; }
        public string? Classification { set; get; }
        public string? ApiToken { set; get; }

        public Dictionary<object, object?> Parameters { set; get; }

        public DataAvailabilityEnpoint()
        {
            Parameters = new Dictionary<object, object?>() 
            {
                { "type", null },
                { "freq", null },
                { "r", null },
                { "ps", null },
                { "px", null },
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

            return EP_BASE.Replace("{parameters}", parameters).Replace("?&", "?");
        }
    }
}

/// ====================================================================================
/// UN Comtrade data enpoint:
///     http://comtrade.un.org/api/get?{parameters}
///     
/// Parameters:         
///     type            : trade data type (default = C)
///         type=C      : commodities 
///         type=S      : services   
///         
///     freq            : data frequency (default = any)
///         freq=A      : annual
///         freq=M      : monthly        
///     
///     r               : reporting area (default = 0)     
///         r=id        : country id
///     
///     ps              : time period
///         ps=2020     : year
///         ps=202012   : month
///         
///     px              : classification
///         ps=HS       : HS classes
///         ps=BEC      : BEC classes
///         ps=EB02     : services classes
///         
///     p               : partner area
///         p=id        : country id
///       
///     rg              : trade flow (default = all)
///         rg=id       : trade regime id
///         
///     cc              : classification code (default = AG2)
///         cc=ALL      : total trade no detail breakdown
///         cc=AG1      : ...
///         cc=AG2      : ...
///         cc=AG3      : ...
///         cc=AG4      : ...
///         cc=AG5      : ...
///         cc=AG6      : ...
///         cc=TOTAL    : all codes in the classification
///     
///     fmt             : output format (default = header) 
///         fmt=json    : JavaScript Object Notation (JSON)
///         fmt=csv     : Comma-separated Values (CSV)
///         
///     max             : maximum records returned (default = 500)  
///         max=number  : 
///         
///     head            : head heading style (default = H)   
///         head=H      : human readable headings        
///         head=M      : machine readable headings         
///     
///     IMTS            : data defined by IMTS (default = 2010)
///         IMTS=2010   : data that comply with IMTS 2010
///         IMTS=orig   : data that comply with earlier version of IMTS
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
    internal class TradeDataEndpoint
    {
        public const string EP_BASE = "http://comtrade.un.org/api/get?{parameters}";

        public Dictionary<object, object?> Parameters { set; get; }

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

        public TradeDataEndpoint()
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

            return EP_BASE.Replace("{parameters}", parameters).Replace("?&", "?"); ;
        }
    }
}

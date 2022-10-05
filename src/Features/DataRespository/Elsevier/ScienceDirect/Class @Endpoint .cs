/// ====================================================================================
/// ScienceDirect API by Elsevier
/// ScienceDirect is Elsevier's premier scientific platform for millions of articles.
/// ScienceDirect API shows peer-reviewed full-text content from scholarly publications.
/// Non-commercial use of the API is free of charge in accordance to Elsevier policies.
/// 
/// The API provides three basic functions: search, metadata, retrieval.
/// Data from ScienceDirect API can be used for several purposes:
/// . IRs and CRIS systems (including VIVO)
/// . Text mining across published contents
/// . Federated search using search engines
/// . Obtained and process article info
/// . Obtained and process journal info
/// 
/// Source: https://dev.elsevier.com/sd_apis.html
/// Source: https://dev.elsevier.com/sd_apis_use_cases.html
/// Source: https://dev.elsevier.com/sciencedirect.html
/// Source: https://dev.elsevier.com/sd_api_spec.html
/// ====================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace DxMLEngine.Features.ScienceDirect
{
    internal class Endpoint
    {
        public Request? Request { set; get; }
        public Response? Response { set; get; }
    }
}

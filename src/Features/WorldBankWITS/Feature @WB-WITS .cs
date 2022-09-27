/// ====================================================================================
/// WITS-API USER GUIDE
/// The API provides trade data from two data sources: UNCTAD Trains and TRADE Stats
/// Data can be assessed by limited requests using url-based or SDMX call structures.
/// Response includes metadata and tariffs by countries, products, or time periods.
/// 
/// http://wits.worldbank.org/data/public/WITSAPI_UserGuide.pdf
/// ====================================================================================

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DxMLEngine.Features.WorldBankWITS
{
    internal class Endpoint
    {
        internal string? Domain { set; get; }
        internal string? QueryKey { set; get; }
        internal string? QueryValue { set; get; }
        internal Dictionary<string, string?>? Parameters { set; get; }
        internal string? Pagination { set; get; }

        public const string EP_URL_BASE = "http://wits.worldbank.org/API/V1/WITS";
        public const string EP_SDMX_BASE = "http://wits.worldbank.org/API/V1/SDMX/V21/rest/";

        #region UNCTAD TRAINS DATA

        public const string EP_META_DATA_ALL = "http://wits.worldbank.org/API/V1/wits/datasource/trn/country/{all}";
        public const string EP_META_DATA_CODE = "http://wits.worldbank.org/API/V1/wits/datasource/trn/country/{countryCode}";
        public const string EP_META_DATA_CODES = "http://wits.worldbank.org/API/V1/wits/datasource/trn/country/{countryCodes}";
        
        public const string EP_NOMENCLATURE_ALL = "http://wits.worldbank.org/API/V1/wits/datasource/trn/nomenclature/{all}";
        public const string EP_NOMENCLATURE_CODE = "http://wits.worldbank.org/API/V1/wits/datasource/trn/nomenclature/{nomenclatureCode}";
        public const string EP_NOMENCLATURE_CODES = "http://wits.worldbank.org/API/V1/wits/datasource/trn/nomenclature/{nomenclatureCodes}";
    
        public const string EP_PRODUCT_ALL = "http://wits.worldbank.org/API/V1/wits/datasource/trn/product/{all}";
        public const string EP_PRODUCT_CODE = "http://wits.worldbank.org/API/V1/wits/datasource/trn/product/{productCode}";
        public const string EP_PRODUCT_CODES = "http://wits.worldbank.org/API/V1/wits/datasource/trn/product/{productCodes}";

        public const string EP_AVAILABILITY_ALL = "http://wits.worldbank.org/API/V1/wits/datasource/trn/dataavailability/";
        public const string EP_AVAILABILITY_BY_COUNTRY_ALL = "http://wits.worldbank.org/API/V1/wits/datasource/trn/dataavailability/country/{all}";
        public const string EP_AVAILABILITY_BY_COUNTRY_CODE = "http://wits.worldbank.org/API/V1/wits/datasource/trn/dataavailability/country/{countryCode}";
        public const string EP_AVAILABILITY_BY_COUNTRY_CODES = "http://wits.worldbank.org/API/V1/wits/datasource/trn/dataavailability/country/{countryCodes}";
        public const string EP_AVAILABILITY_BY_YEAR_ALL = "http://wits.worldbank.org/API/V1/wits/datasource/trn/dataavailability/year/{all}";
        public const string EP_AVAILABILITY_BY_YEAR_ONE = "http://wits.worldbank.org/API/V1/wits/datasource/trn/dataavailability/year/{year}";
        public const string EP_AVAILABILITY_BY_YEAR_MANY = "http://wits.worldbank.org/API/V1/wits/datasource/trn/dataavailability/year/{years}";
        public const string EP_AVAILABILITY_BY_COUNTRY_AND_YEAR_ALL = "http://wits.worldbank.org/API/V1/wits/datasource/trn/dataavailability/country/{all}/year/{all}";
        public const string EP_AVAILABILITY_BY_COUNTRY_AND_YEAR_ONE = "http://wits.worldbank.org/API/V1/wits/datasource/trn/dataavailability/country/{countryCode}/year/{year}";
        public const string EP_AVAILABILITY_BY_COUNTRY_AND_YEAR_MANY = "http://wits.worldbank.org/API/V1/wits/datasource/trn/dataavailability/country/{countryCodes}/year/{years}";

        public const string EP_TARIFF = "http://wits.worldbank.org/API/V1/SDMX/V21/datasource/TRN/reporter/{ALL|Reporter|Reporters}/partner/{ALL|Partner|Partners}/product/{ALL|Product|Products}/year/{ALL|Year|Years}/datatype/{reported|aveestimated}[?format=JSON]";

        public const string EP_DATAFLOW = "http://wits.worldbank.org/API/V1/SDMX/V21/rest/dataflow/wbg_wits/";
        public const string EP_CODELIST = "http://wits.worldbank.org/API/V1/SDMX/V21/rest/codelist/all/";
        public const string EP_DATA_STRUCTURE = "http://wits.worldbank.org/API/V1/SDMX/V21/rest/datastructure/WBG_WITS/TARIFF_TRAINS/";
        public const string EP_TARIFF_DATA = "http://wits.worldbank.org/API/V1/SDMX/V21/rest/datastructure/WBG_WITS/TARIFF_TRAINS/";

        #endregion UNCTAD TRAINS DATA

        #region TRADE STATS DATA
        #endregion TRADE STATS DATA
    }

    internal class WorldBankWITS
    {

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

using System.Text.RegularExpressions;
using Microsoft.Data.Analysis;

using DxMLEngine.Functions;
using Microsoft.ML.Data;
using System.Xml.Linq;

namespace DxMLEngine.Features
{
    internal class SearchUrls
    {
        public const string URL_KEYWORD = "https://patents.google.com/?q={keyword}";
        public const string URL_CLASS_CODE = "https://patents.google.com/?q={classCode}";
        public const string URL_PARTENT_CODE = "https://patents.google.com/patent/{patentCode}";

        public const string PARAM_BEFORE = "&before={before}";
        public const string PARAM_AFTER = "&after={after}";
        public const string PARAM_INVENTOR = "&inventor={inventor}";
        public const string PARAM_ASSIGNEE = "&assignee={assignee}";
        public const string PARAM_COUNTRY = "&country={country}";
        public const string PARAM_LANGUAGE = "&language={language}";
        public const string PARAM_STATUS = "&status={status}";
        public const string PARAM_TYPE = "&type={type}";
        public const string PARAM_LITIGATION = "&litigation={litigation}";
    }

    internal class SearchQuery
    {
        internal string? Keyword { set; get; }
        internal string? ClassCode { set; get; }
        internal string? PatentCode { set; get; }

        internal string? Before { set; get; }
        internal string? After { set; get; }

        internal string? Inventor { set; get; }
        internal string? Assignee { set; get; }

        internal string? Country { set; get; }
        internal string? Language { set; get; }
        internal string? Status { set; get; }
        internal string? Type { set; get; }
        internal string? Litigation { set; get; }

        internal bool SearchByKeyword { set; get; }
        internal bool SearchByClassCode { set; get; }
        internal bool SearchByPatentCode { set; get; }

        public SearchQuery() { }
        public SearchQuery(string? keyword, string? classCode, string? patentCode, 
            string? before, string? after, string? inventor, string? assignee,
            string? country, string? language, string? status, string? litigation)
        {
            this.Keyword = keyword;
            this.ClassCode = classCode;
            this.PatentCode = patentCode;
            this.Before = before;
            this.After = after;
            this.Inventor = inventor;
            this.Assignee = assignee;
            this.Country = country;
            this.Language = language;
            this.Status = status;
            this.Litigation = litigation;
        }
    
        public string ConfigureSearchUrl()
        {
            var url = "";

            if (SearchByKeyword == true) url += SearchUrls.URL_KEYWORD.Replace("{keyword}", Keyword);
            if (SearchByClassCode == true) url += SearchUrls.URL_CLASS_CODE.Replace("{classCode}", Keyword);
            if (SearchByPatentCode == true) url += SearchUrls.URL_PARTENT_CODE.Replace("{patentCode}", Keyword);
    
            if (Before != null) url += SearchUrls.PARAM_BEFORE.Replace("{before}", Before);
            if (After != null) url += SearchUrls.PARAM_AFTER.Replace("{after}", After);
            if (Inventor != null) url += SearchUrls.PARAM_INVENTOR.Replace("{inventor}", Inventor);
            if (Assignee != null) url += SearchUrls.PARAM_ASSIGNEE.Replace("{assignee}", Assignee);
            if (Country != null) url += SearchUrls.PARAM_COUNTRY.Replace("{country}", Country);
            if (Language != null) url += SearchUrls.PARAM_LANGUAGE.Replace("{language}", Language);
            if (Status != null) url += SearchUrls.PARAM_STATUS.Replace("{status}", Status);
            if (Type != null) url += SearchUrls.PARAM_TYPE.Replace("{type}", Type);
            if (Litigation != null) url += SearchUrls.PARAM_LITIGATION.Replace("{litigation}", Litigation);

            return url + "&page={page}";
        }        
    }

    internal class PatentCodePatterns
    {
        public const string WO_CODE = @"NotImplemented";
        public const string US_CODE = @"[U][S][\d\w]+";
        public const string EP_CODE = @"NotImplemented";
        public const string JP_CODE = @"NotImplemented";
        public const string KR_CODE = @"NotImplemented";
        public const string CN_CODE = @"NotImplemented";
        public const string AE_CODE = @"NotImplemented";
        public const string AG_CODE = @"NotImplemented";
        public const string AL_CODE = @"NotImplemented";
        public const string AM_CODE = @"NotImplemented";
        public const string AO_CODE = @"NotImplemented";
        public const string AP_CODE = @"NotImplemented";
        public const string AR_CODE = @"NotImplemented";
        public const string AT_CODE = @"NotImplemented";
        public const string AU_CODE = @"NotImplemented";
        public const string AW_CODE = @"NotImplemented";
        public const string AZ_CODE = @"NotImplemented";
        public const string BA_CODE = @"NotImplemented";
        public const string BB_CODE = @"NotImplemented";
        public const string BD_CODE = @"NotImplemented";
        public const string BE_CODE = @"NotImplemented";
        public const string BF_CODE = @"NotImplemented";
        public const string BG_CODE = @"NotImplemented";
        public const string BH_CODE = @"NotImplemented";
        public const string BJ_CODE = @"NotImplemented";
        public const string BN_CODE = @"NotImplemented";
        public const string BO_CODE = @"NotImplemented";
        public const string BR_CODE = @"NotImplemented";
        public const string BW_CODE = @"NotImplemented";
        public const string BX_CODE = @"NotImplemented";
        public const string BY_CODE = @"NotImplemented";
        public const string BZ_CODE = @"NotImplemented";
        public const string CA_CODE = @"NotImplemented";
        public const string CF_CODE = @"NotImplemented";
        public const string CG_CODE = @"NotImplemented";
        public const string CH_CODE = @"NotImplemented";
        public const string CI_CODE = @"NotImplemented";
        public const string CL_CODE = @"NotImplemented";
        public const string CM_CODE = @"NotImplemented";
        public const string CO_CODE = @"NotImplemented";
        public const string CR_CODE = @"NotImplemented";
        public const string CS_CODE = @"NotImplemented";
        public const string CU_CODE = @"NotImplemented";
        public const string CY_CODE = @"NotImplemented";
        public const string CZ_CODE = @"NotImplemented";
        public const string DD_CODE = @"NotImplemented";
        public const string DE_CODE = @"NotImplemented";
        public const string DJ_CODE = @"NotImplemented";
        public const string DK_CODE = @"NotImplemented";
        public const string DM_CODE = @"NotImplemented";
        public const string DO_CODE = @"NotImplemented";
        public const string DZ_CODE = @"NotImplemented";
        public const string EA_CODE = @"NotImplemented";
        public const string EC_CODE = @"NotImplemented";
        public const string EE_CODE = @"NotImplemented";
        public const string EG_CODE = @"NotImplemented";
        public const string EM_CODE = @"NotImplemented";
        public const string ES_CODE = @"NotImplemented";
        public const string FI_CODE = @"NotImplemented";
        public const string FR_CODE = @"NotImplemented";
        public const string GA_CODE = @"NotImplemented";
        public const string GB_CODE = @"NotImplemented";
        public const string GC_CODE = @"NotImplemented";
        public const string GD_CODE = @"NotImplemented";
        public const string GE_CODE = @"NotImplemented";
        public const string GH_CODE = @"NotImplemented";
        public const string GM_CODE = @"NotImplemented";
        public const string GN_CODE = @"NotImplemented";
        public const string GQ_CODE = @"NotImplemented";
        public const string GR_CODE = @"NotImplemented";
        public const string GT_CODE = @"NotImplemented";
        public const string GW_CODE = @"NotImplemented";
        public const string HK_CODE = @"NotImplemented";
        public const string HN_CODE = @"NotImplemented";
        public const string HR_CODE = @"NotImplemented";
        public const string HU_CODE = @"NotImplemented";
        public const string IB_CODE = @"NotImplemented";
        public const string ID_CODE = @"NotImplemented";
        public const string IE_CODE = @"NotImplemented";
        public const string IL_CODE = @"NotImplemented";
        public const string IN_CODE = @"NotImplemented";
        public const string IR_CODE = @"NotImplemented";
        public const string IS_CODE = @"NotImplemented";
        public const string IT_CODE = @"NotImplemented";
        public const string JO_CODE = @"NotImplemented";
        public const string KE_CODE = @"NotImplemented";
        public const string KG_CODE = @"NotImplemented";
        public const string KH_CODE = @"NotImplemented";
        public const string KM_CODE = @"NotImplemented";
        public const string KN_CODE = @"NotImplemented";
        public const string KP_CODE = @"NotImplemented";
        public const string KW_CODE = @"NotImplemented";
        public const string KZ_CODE = @"NotImplemented";
        public const string LA_CODE = @"NotImplemented";
        public const string LC_CODE = @"NotImplemented";
        public const string LI_CODE = @"NotImplemented";
        public const string LK_CODE = @"NotImplemented";
        public const string LR_CODE = @"NotImplemented";
        public const string LS_CODE = @"NotImplemented";
        public const string LT_CODE = @"NotImplemented";
        public const string LU_CODE = @"NotImplemented";
        public const string LV_CODE = @"NotImplemented";
        public const string LY_CODE = @"NotImplemented";
        public const string MA_CODE = @"NotImplemented";
        public const string MC_CODE = @"NotImplemented";
        public const string MD_CODE = @"NotImplemented";
        public const string ME_CODE = @"NotImplemented";
        public const string MG_CODE = @"NotImplemented";
        public const string MK_CODE = @"NotImplemented";
        public const string ML_CODE = @"NotImplemented";
        public const string MN_CODE = @"NotImplemented";
        public const string MO_CODE = @"NotImplemented";
        public const string MR_CODE = @"NotImplemented";
        public const string MT_CODE = @"NotImplemented";
        public const string MW_CODE = @"NotImplemented";
        public const string MX_CODE = @"NotImplemented";
        public const string MY_CODE = @"NotImplemented";
        public const string MZ_CODE = @"NotImplemented";
        public const string NA_CODE = @"NotImplemented";
        public const string NE_CODE = @"NotImplemented";
        public const string NG_CODE = @"NotImplemented";
        public const string NI_CODE = @"NotImplemented";
        public const string NL_CODE = @"NotImplemented";
        public const string NO_CODE = @"NotImplemented";
        public const string NZ_CODE = @"NotImplemented";
        public const string OA_CODE = @"NotImplemented";
        public const string OM_CODE = @"NotImplemented";
        public const string PA_CODE = @"NotImplemented";
        public const string PE_CODE = @"NotImplemented";
        public const string PG_CODE = @"NotImplemented";
        public const string PH_CODE = @"NotImplemented";
        public const string PL_CODE = @"NotImplemented";
        public const string PT_CODE = @"NotImplemented";
        public const string PY_CODE = @"NotImplemented";
        public const string QA_CODE = @"NotImplemented";
        public const string RO_CODE = @"NotImplemented";
        public const string RS_CODE = @"NotImplemented";
        public const string RU_CODE = @"NotImplemented";
        public const string RW_CODE = @"NotImplemented";
        public const string SA_CODE = @"NotImplemented";
        public const string SC_CODE = @"NotImplemented";
        public const string SD_CODE = @"NotImplemented";
        public const string SE_CODE = @"NotImplemented";
        public const string SG_CODE = @"NotImplemented";
        public const string SI_CODE = @"NotImplemented";
        public const string SK_CODE = @"NotImplemented";
        public const string SL_CODE = @"NotImplemented";
        public const string SM_CODE = @"NotImplemented";
        public const string SN_CODE = @"NotImplemented";
        public const string ST_CODE = @"NotImplemented";
        public const string SU_CODE = @"NotImplemented";
        public const string SV_CODE = @"NotImplemented";
        public const string SY_CODE = @"NotImplemented";
        public const string SZ_CODE = @"NotImplemented";
        public const string TD_CODE = @"NotImplemented";
        public const string TG_CODE = @"NotImplemented";
        public const string TH_CODE = @"NotImplemented";
        public const string TJ_CODE = @"NotImplemented";
        public const string TM_CODE = @"NotImplemented";
        public const string TN_CODE = @"NotImplemented";
        public const string TR_CODE = @"NotImplemented";
        public const string TT_CODE = @"NotImplemented";
        public const string TW_CODE = @"NotImplemented";
        public const string TZ_CODE = @"NotImplemented";
        public const string UA_CODE = @"NotImplemented";
        public const string UG_CODE = @"NotImplemented";
        public const string UY_CODE = @"NotImplemented";
        public const string UZ_CODE = @"NotImplemented";
        public const string VC_CODE = @"NotImplemented";
        public const string VE_CODE = @"NotImplemented";
        public const string VN_CODE = @"NotImplemented";
        public const string YU_CODE = @"NotImplemented";
        public const string ZA_CODE = @"NotImplemented";
        public const string ZM_CODE = @"NotImplemented";
        public const string ZW_CODE = @"NotImplemented";
    }

    internal class GooglePatents
    {
        public static void SearchPatents()
        {
            /// ====================================================================================
            /// Collect patent codes from Google Patents for given assignees input by user 
            /// 
            /// >>> param:  string  # path to input file containing list of search queries 
            /// >>> param:  string  # path to output folder for storing collected data 
            /// >>> param:  string  # name of file to be saved as output after searching 
            ///             
            /// >>> funct:  0       # 
            /// >>> funct:  1       #
            /// >>> funct:  2       #
            /// >>> funct:  3       #
            /// >>> funct:  4       #
            /// >>> funct:  5       #
            /// ====================================================================================

            ////0
            Console.Write("\nEnter input file path: ");
            var i_fil = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(i_fil))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("\nEnter output folder path: ");
            var o_fol = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(o_fol))
                throw new ArgumentNullException("path is null or empty");           
            
            Console.Write("\nEnter output file name: ");
            var o_fil = Console.ReadLine()?.Replace(" ", "");

            if (string.IsNullOrEmpty(o_fil))
                throw new ArgumentNullException("path is null or empty");            

            Console.WriteLine("\nSelect search method: ");
            Console.WriteLine("1 Search by keyword");
            Console.WriteLine("2 Search by class code");
            Console.WriteLine("3 Search by patent code");
            Console.Write("\nSelection: ");

            var selection = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(selection))
                throw new ArgumentNullException("selection is null or empty");

            bool searchByKeyword;
            bool searchByClassCode;
            bool searchByPatentCode;
            switch (selection)
            {
                case "1":
                    searchByKeyword = true;
                    searchByClassCode = false;
                    searchByPatentCode = false;
                    break;                
                
                case "2":
                    searchByKeyword = false;
                    searchByClassCode = true;
                    searchByPatentCode = false;
                    break;                
                
                case "3":
                    searchByKeyword = false;
                    searchByClassCode = false;
                    searchByPatentCode = true;
                    break;

                default:
                    searchByKeyword = true;
                    searchByClassCode = false;
                    searchByPatentCode = false;
                    break;
            }

            ////1
            var dataFrame = DataFrame.LoadCsv(i_fil, header: true, encoding: Encoding.UTF8);

            var queries = new List<SearchQuery>();
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var query = new SearchQuery();

                var keyword = dataFrame.GetColumn<string>("Keyword").ToArray()[i].Replace(" ", "+");
                var classCode = dataFrame.GetColumn<string>("Class Code").ToArray()[i].Replace(" ", "+");
                var before = dataFrame.GetColumn<float>("Before").ToArray()[i].ToString().Replace(" ", "+");
                var after = dataFrame.GetColumn<float>("After").ToArray()[i].ToString().Replace(" ", "+");
                var inventor = dataFrame.GetColumn<string>("Inventor").ToArray()[i].Replace(" ", "+");
                var assignee = dataFrame.GetColumn<string>("Assignee").ToArray()[i].Replace(" ", "+");
                var country = dataFrame.GetColumn<string>("Country").ToArray()[i].Replace(" ", "+");
                var language = dataFrame.GetColumn<string>("Language").ToArray()[i].Replace(" ", "+");
                var status = dataFrame.GetColumn<string>("Status").ToArray()[i].Replace(" ", "+");
                var litigation = dataFrame.GetColumn<string>("Litigation").ToArray()[i].Replace(" ", "+");

                query.Keyword = string.IsNullOrEmpty(keyword) ? null : keyword;
                query.ClassCode = string.IsNullOrEmpty(classCode) ? null : classCode;
                query.Before = string.IsNullOrEmpty(before) ? null : before;
                query.After = string.IsNullOrEmpty(after) ? null : after;
                query.Inventor = string.IsNullOrEmpty(inventor) ? null : inventor;
                query.Assignee = string.IsNullOrEmpty(assignee) ? null : assignee;
                query.Country = string.IsNullOrEmpty(country) ? null : country;
                query.Language = string.IsNullOrEmpty(language) ? null : language;
                query.Status = string.IsNullOrEmpty(status) ? null : status;
                query.Litigation = string.IsNullOrEmpty(litigation) ? null : litigation;

                query.SearchByKeyword = searchByKeyword;
                query.SearchByClassCode = searchByClassCode;
                query.SearchByPatentCode = searchByPatentCode;

                queries.Add(query);
            }

            ////1
            var browser = Process.Start("MicrosoftEdge.exe", "edge://version/");
            Thread.Sleep(100);

            ////2
            var dataColumns = new List<DataFrameColumn>()
            {
                new StringDataFrameColumn("Keyword"),
                new StringDataFrameColumn("Class Code"),
                new StringDataFrameColumn("Before"),
                new StringDataFrameColumn("After"),
                new StringDataFrameColumn("Inventor"),
                new StringDataFrameColumn("Assignee"),
                new StringDataFrameColumn("Country"),
                new StringDataFrameColumn("Language"),
                new StringDataFrameColumn("Status"),
                new StringDataFrameColumn("Litigation"),
                new StringDataFrameColumn("Patent Code"),
            };

            dataFrame = new DataFrame(dataColumns);

            ////3            
            foreach (var query in queries)
            {
                var url = query.ConfigureSearchUrl();

            ////4
                var process = Process.Start("MicrosoftEdge.exe", url.Replace("{page}", "0"));

                Thread.Sleep(5000);
                Keyboard.SendKeys(process, "CTRL+A", 100);
                Keyboard.SendKeys(process, "CTRL+C", 100);

                var text = Clipboard.GetText();
                var pattern = @"(About) [\d,]+ (results)";
                var regex = new Regex(pattern);
                var match = regex.Match(text!);
                var numResults = int.Parse(match.Value.Split(" ")[1].Replace(",", ""));

                var numPages = 99;
                if (numResults < 1000)
                {
                    if (numResults < 100)
                    {
                        var digits = numResults.ToString();
                        numPages = int.Parse(digits[0].ToString());
                    }
                    else
                    {
                        var digits = numResults.ToString();
                        numPages = int.Parse(digits[0..2]);
                    }
                }

                Keyboard.SendKeys(process, "CTRL+W", 100);

            ////5
                var patentCodes = new List<string>();
                for (int page = 0; page < numPages; page++)
                {
                    Console.WriteLine($"Download: {url.Replace("{page}", page.ToString())}");
                    process = Process.Start("MicrosoftEdge.exe", url.Replace("{page}", page.ToString()));

                    Thread.Sleep(5000);
                    Keyboard.SendKeys(process, "CTRL+A", 100);
                    Keyboard.SendKeys(process, "CTRL+C", 100);

                    var pageText = Clipboard.GetText();

                    var patentCodePatterns =
                        from field in typeof(PatentCodePatterns).GetFields()
                        select (string?)field.GetValue(null);

                    foreach (var patentCodePattern in patentCodePatterns.ToArray())
                    {
                        var codeRegex = new Regex(patentCodePattern);
                        var codeMatches = codeRegex.Matches(pageText!);
                        foreach (var code in codeMatches.ToArray())
                            patentCodes.Add(code.Value);
                    }

                    Keyboard.SendKeys(process, "CTRL+W", 100);
                }

            ////6
                foreach (var patentCode in patentCodes) 
                { 
                    var dataRow = new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("Keyword", query.Keyword != null ? query.Keyword : "Null"),
                        new KeyValuePair<string, object>("Class Code", query.ClassCode != null ? query.ClassCode : "Null"),
                        new KeyValuePair<string, object>("Before", query.Before != null ? query.Before : "Null"),
                        new KeyValuePair<string, object>("After", query.After != null ? query.After : "Null"),
                        new KeyValuePair<string, object>("Inventor", query.Inventor != null ? query.Inventor : "Null"),
                        new KeyValuePair<string, object>("Assignee", query.Assignee != null ? query.Assignee : "Null"),
                        new KeyValuePair<string, object>("Country", query.Country != null ? query.Country : "Null"),
                        new KeyValuePair<string, object>("Language", query.Language != null ? query.Language : "Null"),
                        new KeyValuePair<string, object>("Status", query.Status != null ? query.Status : "Null"),
                        new KeyValuePair<string, object>("Litigation", query.Litigation != null ? query.Litigation : "Null"),
                        new KeyValuePair<string, object>("Patent Code", patentCode),
                    };

                    dataFrame.Append(dataRow, inPlace: true);
                }

            }

            ////7
            Keyboard.SendKeys(browser, "ALT+F4", 100);

            ////8
            o_fil = $"{o_fol}\\Dataset @{o_fil}Patents #-------------- .csv";
            DataFrame.WriteCsv(dataFrame, o_fil, encoding: Encoding.UTF8);

            var timestamp = File.GetCreationTime(o_fil).ToString("yyyyMMddHHmmss");
            File.Move(o_fil, o_fil.Replace("#--------------", $"#{timestamp}"));
        }
    
        public static void CollectPatents()
        {

        }
    }
}

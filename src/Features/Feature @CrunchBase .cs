using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using HtmlAgilityPack;

namespace DxMLEngine.Features
{
    internal class CrunchBase
    {
        private static readonly string _CompanyTwitterXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[16]/div/field-formatter/link-formatter/a";
        private static readonly string _CompanyFacebookXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[17]/div/field-formatter/link-formatter/a";
        private static readonly string _CompanyLinkedInXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[18]/div/field-formatter/link-formatter/a";

        public static HtmlDocument ReadWebpage(string path)
        {
            if (!Path.IsPathFullyQualified(path))
                throw new ArgumentNullException("path is not fully qualified");

            var document = new HtmlDocument();
            try
            {
                document.Load(path);
            }
            catch (Exception)
            {
                throw;
            }

            return document;
        }

        public static string[] ConfigureCompaniesHeader()
        {
            var header = new List<string>()
            {
                "Organization Name",
                "Founded Date",
                "Industries",
                "Headquarters Location",
                "Description",
                "CB Rank (Company)",
                "Headerquarters Regions",
                "Diversity Spotlight (US Only)",
                "Estimated Revenue Range",
                "Operating Status",
                "Exit Date",
                "Closed Date",
                "Company Type",
                "Website",
                "Twitter",
                "Facebook",
                "LinkedIn",
                "Contact Email",
                "Phone Number",
                "Number of Articles",
                "Hub Tags",
                "Full Description",
                "Actively Hiring",
                "Investor Type",
                "Investment Stage",
                "School Type",
                "School Program",
                "Number of Enrollments",
                "School Method",
                "Number of Founders (Alumni)",
                "Industry Groups",
                "Number of Founders",
                "Founders",
                "Number of Employees",
                "Number of Funding Rounds",
                "Funding Status",
                "Last Funding Date",
                "Last Funding Amount",
                "Last Funding Type",
                "Last Equity Funding Amount",
                "Last Equity Funding Type",
                "Total Equity Funding Amount",
                "Total Funding Amount",
                "Top Five Investors",
                "Number of Lead Investors",
                "Number of Investors",
                "Number of Acquisitions",
                "Acquisition Status",
                "Transaction Name",
                "Acquired by",
                "Announced Date",
                "Price",
                "Acquisition Type",
                "Acquisition Terms",
                "IPO Status",
                "IPO Date",
                "Delisted Date",
                "Money Raised at IPO",
                "Valuation at IPO",
                "Stock Symbol",
                "Stock Exchange",
                "Last Leadership Hiring Date",
                "Last Layoff Mention Date",
                "Number of Events",
                "CB Rank (Organization)",
                "CB Rank (School)",
                "Trend Score (7 Days)",
                "Trend Score (30 Days)",
                "Trend Score (90 Days)",
                "Similar Companies",
                "Contact Job Departments",
                "Number of Contacts",
                "Monthly Visits",
                "Average Visits (6 Months)",
                "Monthly Visits Growth",
                "Visit Duration",
                "Visit Duration Growth",
                "Page Views / Visit",
                "Page Views / Visit Growth",
                "Bounce Rate",
                "Bounce Rate Growth",
                "Global Traffic Rank",
                "Monthly Rank Change (#)",
                "Monthly Rank Growth",
                "Active Tech Count",
                "Number of Apps",
                "Download Last 30 Days",
                "Total Product Active",
                "Patents Granted",
                "Trademarks Registered",
                "Most Popular Patent Class",
                "Most Popular Trademark Class",
                "IT Spend",
                "Most Recent Valuation Range",
                "Date of Most Recent Valuation",
                "Number of Portfolio Organizations",
                "Number of Investment",
                "Number of Lead Investments",
                "Number of Diversity Investments",
                "Number of Exits",
                "Number of Exits (IPO)",
                "Accelerator Program Type",
                "Accelerator Application Deadline",
                "Accelerator Duration (in Weeks)",
                "Number of Alumni",
                "Number of Private Contacts",
                "Number of Private Notes",
                "Tags",
            };

            return header.ToArray();
        }

        public static string[] ConfigureCompaniesXPaths()
        {
            var _NameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[2]/div/field-formatter/identifier-formatter/a/div/div";
            var _FoundedXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[3]/div/field-formatter/span";
            var _IndustriesXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[4]/div/field-formatter/identifier-multi-formatter/span";
            var _HQLocationXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[5]/div/field-formatter/identifier-multi-formatter/span";
            var _DescriptionXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[6]/div/field-formatter/span";
            var _CBRankCompanyXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[7]/div/field-formatter/a";
            var _HQRegionXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[8]/div/field-formatter/identifier-multi-formatter/span";
            var _DiversityXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[9]/div/field-formatter/identifier-multi-formatter/span";
            var _EstimatedRevenueXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[10]/div/field-formatter/a";
            var _OperatingStatusXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[11]/div/field-formatter/span";
            var _ExitDateXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[12]/div/field-formatter/span";
            var _ClosedDateXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[13]/div/field-formatter/span";
            var _CompanyTypeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[14]/div/field-formatter/span";
            var _WebsiteXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[15]/div/field-formatter/link-formatter/a";
            var _TwitterXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[16]/div/field-formatter/link-formatter/a";
            var _FacebookXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[17]/div/field-formatter/link-formatter/a";
            var _LinkedInXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[18]/div/field-formatter/link-formatter/a";
            var _EmailXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[19]/div/field-formatter/blob-formatter/span";
            var _PhoneXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[20]/div/field-formatter/blob-formatter/span";
            var _NumArticlesXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[21]/div/field-formatter/a";
            var _HubTagsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[22]/div/field-formatter/enum-multi-formatter/span";
            var _FullDescriptionXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[23]/div/field-formatter/span";
            var _ActiveHiringXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[24]/div/field-formatter/span";
            var _InvestorTypeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[25]/div/field-formatter/enum-multi-formatter/span";
            var _InvestorStageXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[26]/div/field-formatter/enum-multi-formatter/span";
            var _SchoolTypeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[27]/div/field-formatter/span";
            var _SchoolProgramXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[28]/div/field-formatter/span";
            var _NumEnrollmentsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[29]/div/field-formatter/span";
            var _SchoolMethodXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[30]/div/field-formatter/span";
            var _NumAlumFoundersXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[31]/div/field-formatter/a";
            var _IndustryGroupXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[32]/div/field-formatter/identifier-multi-formatter/span";
            var _NumFoundersXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[33]/div/field-formatter/a";
            var _FoundersXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[34]/div/field-formatter/identifier-multi-formatter/span";
            var _NumEmployeesXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[35]/div/field-formatter/a";
            var _NumFundingRoundsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[36]/div/field-formatter/a";
            var _FoundingStatusXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[37]/div/field-formatter/span";
            var _LastFundingDateXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[38]/div/field-formatter/a";
            var _LastFundingAmountXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[39]/div/field-formatter/a";
            var _LastFundingTypeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[40]/div/field-formatter/a";
            var _LastEquityFundingAmountXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[41]/div/field-formatter/a";
            var _LastEquityFundingTypeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[42]/div/field-formatter/a";
            var _TotalEquityFundingAmountXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[43]/div/field-formatter/a";
            var _TotalFundingAmountXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[44]/div/field-formatter/a";
            var _TopFiveInvestorsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[45]/div/field-formatter/identifier-multi-formatter/span";
            var _NumLeadInvestorsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[46]/div/field-formatter/a";
            var _NumInvestorsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[47]/div/field-formatter/a";
            var _NumAcquisitionsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[48]/div/field-formatter/a";
            var _AcquisitionStatusXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[49]/div/field-formatter/enum-multi-formatter/span";
            var _TransactionNameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[50]/div/field-formatter/identifier-formatter/span";
            var _AcquiredByXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[51]/div/field-formatter/identifier-formatter/span";
            var _AnnouncedDateXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[52]/div/field-formatter/span";
            var _PriceXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[53]/div/field-formatter/span";
            var _AcquisitionTypeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[54]/div/field-formatter/span";
            var _AcquisitionTermsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[55]/div/field-formatter/span";
            var _IPOStatusXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[56]/div/field-formatter/span";
            var _IPODateXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[57]/div/field-formatter/span";
            var _DelistedDateXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[58]/div/field-formatter/span";
            var _MoneyRaisedXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[59]/div/field-formatter/span";
            var _ValuationXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[60]/div/field-formatter/span";
            var _StockSymbolXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[61]/div/field-formatter/identifier-formatter/a/div/div";
            var _StockExchangeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[62]/div/field-formatter/span";
            var _LastLeaderHireXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[63]/div/field-formatter/span";
            var _LastLayoffMentionXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[64]/div/field-formatter/span";
            var _NumEventsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[65]/div/field-formatter/a";
            var _CBRankOrganizationXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[66]/div/field-formatter/a";
            var _CBRankSchoolXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[67]/div/field-formatter/span";
            var _TrendScore7DXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[68]/div/field-formatter/span";
            var _TrendScore30DXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[69]/div/field-formatter/span";
            var _TrendScore90DXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[70]/div/field-formatter/span";
            var _SimilarCompaniesXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[71]/div/field-formatter/span";
            var _JobDepartmentsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[72]/div/field-formatter/enum-multi-formatter/span";
            var _NumContactsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[73]/div/field-formatter/a";
            var _MonthlyVisitsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[74]/div/field-formatter/span";
            var _AverageVisitsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[75]/div/field-formatter/span";
            var _MonthlyVisitGrowthXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[76]/div/field-formatter/span";
            var _VisitDurationXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[77]/div/field-formatter/span";
            var _VisitDurationGrowthXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[78]/div/field-formatter/span";
            var _PageViewsVisitXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[79]/div/field-formatter/span";
            var _PageViewsVisitGrowthXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[80]/div/field-formatter/span";
            var _BounceRateXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[81]/div/field-formatter/span";
            var _BounceRateGrowthXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[82]/div/field-formatter/span";
            var _GlobalTrafficRankXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[83]/div/field-formatter/span";
            var _MonthlyRankChangeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[84]/div/field-formatter/span";
            var _MonthlyRankGrowthXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[85]/div/field-formatter/span";
            var _ActiveTechCountsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[86]/div/field-formatter/span";
            var _NumAppsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[87]/div/field-formatter/span";
            var _DownloadsLast30DXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[88]/div/field-formatter/span";
            var _TotalProductsActiveXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[89]/div/field-formatter/span";
            var _PatentsGrantedXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[90]/div/field-formatter/a";
            var _TrademarksRegisteredXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[91]/div/field-formatter/a";
            var _MostPopularPatentClassXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[92]/div/field-formatter/a";
            var _MostPopularTrademarkClassXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[93]/div/field-formatter/a";
            var _ITSpendXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[94]/div/field-formatter/span";
            var _MostRecentValuationRangeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[95]/div/field-formatter/span";
            var _MostRecentValuationDateXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[96]/div/field-formatter/span";
            var _NumPortfoliosXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[97]/div/field-formatter/a";
            var _NumInvestmentsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[98]/div/field-formatter/a";
            var _NumLeadInvestmentsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[99]/div/field-formatter/a";
            var _NumDiversityInvestmentsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[100]/div/field-formatter/a";
            var _NumExitsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[101]/div/field-formatter/a";
            var _NumExitsIPOXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[102]/div/field-formatter/a";
            var _AccProgramTypeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[103]/div/field-formatter/span";
            var _AccApplicationDeadlineXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[104]/div/field-formatter/span";
            var _AccDurationXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[105]/div/field-formatter/span";
            var _NumAlumniXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[106]/div/field-formatter/a";
            var _NumPrivateContactsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[107]/div/field-formatter/span";
            var _NumPrivateNotesXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[108]/div/field-formatter/span";
            var _TagsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[109]/div/field-formatter/identifier-multi-formatter/span";

            string[] xpaths = {
                _NameXPath, _FoundedXPath, _IndustriesXPath, _HQLocationXPath, _DescriptionXPath, _CBRankCompanyXPath, _HQRegionXPath,
                _DiversityXPath, _EstimatedRevenueXPath, _OperatingStatusXPath, _ExitDateXPath, _ClosedDateXPath, _CompanyTypeXPath,
                _WebsiteXPath, _TwitterXPath, _FacebookXPath, _LinkedInXPath, _EmailXPath, _PhoneXPath, _NumArticlesXPath, _HubTagsXPath,
                _FullDescriptionXPath, _ActiveHiringXPath, _InvestorTypeXPath, _InvestorStageXPath, _SchoolTypeXPath, _SchoolProgramXPath,
                _NumEnrollmentsXPath, _SchoolMethodXPath, _NumAlumFoundersXPath, _IndustryGroupXPath, _NumFoundersXPath, _FoundersXPath,
                _NumEmployeesXPath, _NumFundingRoundsXPath, _FoundingStatusXPath, _LastFundingDateXPath, _LastFundingAmountXPath,
                _LastFundingTypeXPath, _LastEquityFundingAmountXPath, _LastEquityFundingTypeXPath, _TotalEquityFundingAmountXPath,
                _TotalFundingAmountXPath, _TopFiveInvestorsXPath, _NumLeadInvestorsXPath, _NumInvestorsXPath, _NumAcquisitionsXPath,
                _AcquisitionStatusXPath, _TransactionNameXPath, _AcquiredByXPath, _AnnouncedDateXPath, _PriceXPath, _AcquisitionTypeXPath,
                _AcquisitionTermsXPath, _IPOStatusXPath, _IPODateXPath, _DelistedDateXPath, _MoneyRaisedXPath, _ValuationXPath,
                _StockSymbolXPath, _StockExchangeXPath, _LastLeaderHireXPath, _LastLayoffMentionXPath, _NumEventsXPath,
                _CBRankOrganizationXPath, _CBRankSchoolXPath, _TrendScore7DXPath, _TrendScore30DXPath, _TrendScore90DXPath,
                _SimilarCompaniesXPath, _JobDepartmentsXPath, _NumContactsXPath, _MonthlyVisitsXPath, _AverageVisitsXPath, _MonthlyVisitGrowthXPath,
                _VisitDurationXPath, _VisitDurationGrowthXPath, _PageViewsVisitXPath, _PageViewsVisitGrowthXPath, _BounceRateXPath, _BounceRateGrowthXPath,
                _GlobalTrafficRankXPath, _MonthlyRankChangeXPath, _MonthlyRankGrowthXPath, _ActiveTechCountsXPath, _NumAppsXPath,
                _DownloadsLast30DXPath, _TotalProductsActiveXPath, _PatentsGrantedXPath, _TrademarksRegisteredXPath,
                _MostPopularPatentClassXPath, _MostPopularTrademarkClassXPath, _ITSpendXPath, _MostRecentValuationRangeXPath, _MostRecentValuationDateXPath,
                _NumPortfoliosXPath, _NumInvestmentsXPath, _NumLeadInvestmentsXPath, _NumDiversityInvestmentsXPath, _NumExitsXPath, _NumExitsIPOXPath,
                _AccProgramTypeXPath, _AccApplicationDeadlineXPath, _AccDurationXPath, _NumAlumniXPath, 
                _NumPrivateContactsXPath, _NumPrivateNotesXPath, _TagsXPath
            };

            return xpaths;
        }

        public static void ExtractCompanies()
        {
            Console.Write("Enter input file: ");
            var i_fil = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(i_fil))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output folder: ");
            var o_fol = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(o_fol))
                throw new ArgumentNullException("path is null or empty");

            var paths = File.ReadAllLines(i_fil);
            paths = (from path in paths select path.Replace("\"", "")).ToArray();

            var header = ConfigureCompaniesHeader();
            var xpaths = ConfigureCompaniesXPaths();

            var dataframe = new List<string[]>() { header };
            foreach (var path in paths)
            {
                Console.WriteLine($"Extract: {Path.GetFileName(path)}");

                var document = ReadWebpage(path);
                for (int i = 1; i <= 50; i++)
                {
                    var dataline = new List<string>();
                    foreach (var xpath in xpaths)
                    {
                        var fullXPath = xpath.Replace("{index}", $"{i}");
                        var node = document.DocumentNode.SelectSingleNode(fullXPath);

                        if (xpath == _CompanyTwitterXPath || xpath == _CompanyFacebookXPath || xpath == _CompanyLinkedInXPath)
                        {
                            try 
                            {
                                dataline.Add(node.GetAttributeValue("href", "—"));                                
                            }
                            catch
                            {
                                dataline.Add("—");                                
                            }
                            continue;
                        }

                        try
                        {
                            var text = node.InnerText.Trim().Replace("\"", "\"\"");
                            dataline.Add("\"" + text + "\"");
                        }
                        catch
                        {
                            dataline.Add("—");
                        }
                    }

                    dataframe.Add(dataline.ToArray());
                }
            }

            var datalines =
                from dataline in dataframe
                select string.Join(",", dataline);

            var o_fil = $"{o_fol}\\Dataset @CrunchBaseComapnies #-------------- .csv";
            File.WriteAllLines(o_fil, datalines, encoding: Encoding.UTF8);
            var timestamp = File.GetCreationTime(o_fil).ToString("yyyyMMddHHmmss");
            File.Move(o_fil, o_fil.Replace("#--------------", timestamp));
        }
    }
}

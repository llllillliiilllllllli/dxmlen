using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;

using DxMLEngine.Attributes;
using DxMLEngine.Functions;

using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace DxMLEngine.Features.DataCollection
{
    [Feature]
    internal class CrunchBaseCompany
    {
        private static readonly string _CompanyTwitterXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[16]/div/field-formatter/link-formatter/a";
        private static readonly string _CompanyFacebookXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[17]/div/field-formatter/link-formatter/a";
        private static readonly string _CompanyLinkedInXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[18]/div/field-formatter/link-formatter/a";

        private static HtmlDocument ReadWebpage(string path)
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

        private static string[] ConfigureHeader()
        {
            var header = new List<string>()
            {
                "Organization Name",
                "Founded Year",
                "Industry",
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
                "Industry Group",
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
                "Page Views Visit",
                "Page Views Visit Growth",
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
                "Number of Investments",
                "Number of Lead Investments",
                "Number of Diversity Investments",
                "Number of Exits",
                "Number of Exits (IPO)",
                "Accelerator Program Type",
                "Accelerator Application Deadline",
                "Accelerator Duration (Week)",
                "Number of Alumni",
                "Number of Private Contacts",
                "Number of Private Notes",
                "Tags",
            };

            return header.ToArray();
        }

        private static string[] ConfigureXPaths()
        {
            var _NameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[2]/div/field-formatter/identifier-formatter/a/div/div";
            var _FoundedXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[3]/div/field-formatter/span";
            var _IndustryXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[4]/div/field-formatter/identifier-multi-formatter/span";
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
                _NameXPath, _FoundedXPath, _IndustryXPath, _HQLocationXPath, _DescriptionXPath, _CBRankCompanyXPath, _HQRegionXPath,
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

        public static void CollectData()
        {
            Console.Write("Enter input file path: ");
            var i_fil = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(i_fil))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output folder path: ");
            var o_fol = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(o_fol))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output file name: ");
            var o_fil = Console.ReadLine()?.Replace(" ", "");

            var paths = File.ReadAllLines(i_fil);
            paths = (from path in paths select path.Replace("\"", "")).ToArray();

            var header = ConfigureHeader();
            var xpaths = ConfigureXPaths();

            var dataframe = new List<string[]>() { header };
            foreach (var path in paths)
            {
                if (!Path.IsPathFullyQualified(path)) continue;
                Console.WriteLine($"Collect: {Path.GetFileName(path)}");

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
                                var href = node.GetAttributeValue("href", "—");
                                href = href.Replace(",", "%2C");
                                dataline.Add(href);
                            }
                            catch
                            {
                                dataline.Add("—");                                
                            }
                            continue;
                        }

                        try
                        {
                            var text = node.InnerText.Replace("\"", "\"\"").Trim();
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

            o_fil = $"{o_fol}\\Dataset @{o_fil} #-------------- .csv";
            File.WriteAllLines(o_fil, datalines, encoding: Encoding.UTF8);
            var timestamp = File.GetCreationTime(o_fil).ToString("yyyyMMddHHmmss");
            File.Move(o_fil, o_fil.Replace("#--------------", $"#{timestamp}"));
        }
    }

    [Feature]
    internal class CrunchBaseAcquisition
    {
        private static HtmlDocument ReadWebpage(string path)
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

        private static string[] ConfigureHeader()
        {
            var header = new List<string>()
            {
                "Transaction Name",
                "Acquiree Name",
                "Acquirer Name",
                "Announced Date",
                "Price",
                "Acquisition Type",
                "Acquisition Terms",

                "Acquiree Description",
                "Acquiree Last Funding Type",
                "Acquiree Industry",
                "Acquiree Headquarters Location",
                "Acquiree Website",
                "Acquiree Estimated Revenue Range",
                "Acquiree Total Funding Amount",
                "Acquiree Funding Status",
                "Acquiree Number of Funding Rounds",

                "Acquirer Description",
                "Acquirer Industry",
                "Acquirer Headquarters Location",
                "Acquirer Website",
                "Acquirer Estimated Revenue Range",
                "Acquirer Total Funding Amount",
                "Acquirer Funding Status",
                "Acquirer Number of Funding Rounds",

                "CB Rank (Acquisition)"
            };

            return header.ToArray();
        }

        private static string[] ConfigureXPaths()
        {
            var _TransactionNameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[2]/div/field-formatter/identifier-formatter/a/div/div";
            var _AcquireeNameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[3]/div/field-formatter/identifier-formatter/a/div/div";
            var _AcquirerNameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[4]/div/field-formatter/identifier-formatter/a/div/div";
            var _AnnouncedDateXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[5]/div/field-formatter/span";
            var _PriceXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[6]/div/field-formatter/span";
            var _AcquisitionTypeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[7]/div/field-formatter/span";
            var _AcquisitionTermsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[8]/div/field-formatter/span";
            
            var _AcquireeDescriptionXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[9]/div/field-formatter/span";
            var _AcquireeLastFundingTypeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[10]/div/field-formatter/a";
            var _AcquireeIndustryXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[11]/div/field-formatter/identifier-multi-formatter/span";
            var _AcquireeHQLocationXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[12]/div/field-formatter/identifier-multi-formatter/span";
            var _AcquireeWebsiteXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[13]/div/field-formatter/link-formatter/a";
            var _AcquireeEstRevenueRangeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[14]/div/field-formatter/a";
            var _AcquireeTotalFundingAmountXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[15]/div/field-formatter/span";
            var _AcquireeFundingStatusXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[16]/div/field-formatter/span";
            var _AcquireeNumFundingRoundsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[17]/div/field-formatter/span";

            var _AcquirerDecriptionXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[18]/div/field-formatter/span";
            var _AcquirerIndustryXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[19]/div/field-formatter/identifier-multi-formatter/span";
            var _AcquirerHQLocationXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[20]/div/field-formatter/identifier-multi-formatter/span";
            var _AcquirerWebsiteXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[21]/div/field-formatter/link-formatter/a";
            var _AcquirerEstRevenueRangeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[22]/div/field-formatter/a";
            var _AcquirerTotalFundingAmountXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[23]/div/field-formatter/span";
            var _AcquirerFundingStatusXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[24]/div/field-formatter/span";
            var _AcquirerNumFundingRoundsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[25]/div/field-formatter/span";

            var _CBRankAcquisitionXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[26]/div/field-formatter/a"; 

            string[] xpaths =
            {
                _TransactionNameXPath,
                _AcquireeNameXPath,
                _AcquirerNameXPath,
                _AnnouncedDateXPath,
                _PriceXPath,
                _AcquisitionTypeXPath,
                _AcquisitionTermsXPath,
                _AcquireeDescriptionXPath,
                _AcquireeLastFundingTypeXPath,
                _AcquireeIndustryXPath,
                _AcquireeHQLocationXPath,
                _AcquireeWebsiteXPath,
                _AcquireeEstRevenueRangeXPath,
                _AcquireeTotalFundingAmountXPath,
                _AcquireeFundingStatusXPath,
                _AcquireeNumFundingRoundsXPath,
                _AcquirerDecriptionXPath,
                _AcquirerIndustryXPath,
                _AcquirerHQLocationXPath,
                _AcquirerWebsiteXPath,
                _AcquirerEstRevenueRangeXPath,
                _AcquirerTotalFundingAmountXPath,
                _AcquirerFundingStatusXPath,
                _AcquirerNumFundingRoundsXPath,
                _CBRankAcquisitionXPath,
            };

            return xpaths;
        }

        public static void CollectData()
        {
            Console.Write("Enter input file path: ");
            var i_fil = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(i_fil))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output folder path: ");
            var o_fol = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(o_fol))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output file name: ");
            var o_fil = Console.ReadLine()?.Replace(" ", "");

            var paths = File.ReadAllLines(i_fil);
            paths = (from path in paths select path.Replace("\"", "")).ToArray();

            var header = ConfigureHeader();
            var xpaths = ConfigureXPaths();

            var dataframe = new List<string[]>() { header };
            foreach (var path in paths)
            {
                if (!Path.IsPathFullyQualified(path)) continue;
                Console.WriteLine($"Collect: {Path.GetFileName(path)}");

                var document = ReadWebpage(path);
                for (int i = 1; i <= 50; i++)
                {
                    var dataline = new List<string>();
                    foreach (var xpath in xpaths)
                    {
                        var fullXPath = xpath.Replace("{index}", $"{i}");
                        var node = document.DocumentNode.SelectSingleNode(fullXPath);

                        try
                        {
                            var text = node.InnerText.Replace("\"", "\"\"").Trim();
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

            o_fil = $"{o_fol}\\Dataset @{o_fil} #-------------- .csv";
            File.WriteAllLines(o_fil, datalines, encoding: Encoding.UTF8);
            var timestamp = File.GetCreationTime(o_fil).ToString("yyyyMMddHHmmss");
            File.Move(o_fil, o_fil.Replace("#--------------", $"#{timestamp}"));
        }
    }

    [Feature]
    internal class CrunchBaseContact
    {
        private static readonly string _ContactLinkedInXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[6]/div/field-formatter/blob-formatter/external-link/a";

        private static HtmlDocument ReadWebpage(string path)
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

        private static string[] ConfigureHeader()
        {
            var header = new List<string>()
            {
                "Name",
                "Organization",
                "Job Titles",
                "Organization HQ Location",
                "LinkedIn",
                "Job Departments",
                "Job Levels",
                "Work Emails",
                "Personal Emails",
                "Work Phone Numbers",
                "Personal Phone Numbers",
                "Number of Private Contacts",
                "Number of Contact Emails",
                "Number of Contact Phones",
                "Organization HQ Region",
                "Organization Industry",
                "Organization Number of Employees",
                "Organization Founded Date",
                "Organization Last Funding Date",
                "Organization Last Funding Amount",
                "Organization Last Funding Type",
                "Organization Last Funding Status",
                "Organization Total Funding Amount",
                "Organization Estimated Revenue Range",
                "Organization Operating Status",
                "Organization Acquisition Status",
                "Organization IPO Status",
                "Organization Tags",
            };

            return header.ToArray();
        }

        private static string[] ConfigureXPaths()
        {
            var _NameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[2]/div/field-formatter/blob-formatter/span";
            var _OrganizationXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[3]/div/field-formatter/identifier-formatter/a/div/div";
            var _JobTitlesXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[4]/div/field-formatter/identifier-multi-formatter/span";
            var _OrganizationHQLocationXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[5]/div/field-formatter/identifier-multi-formatter/span";
            var _LinkedInXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[6]/div/field-formatter/blob-formatter/external-link/a";
            var _JobDepartmentsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[7]/div/field-formatter/enum-multi-formatter";
            var _JobLevelsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[8]/div/field-formatter/enum-multi-formatter/a";
            var _WorkEmailsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[9]/div/field-formatter/span";
            var _PersonalEmailsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[10]/div/field-formatter/span";
            var _WorkPhoneNumbersXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[11]/div/field-formatter/span";
            var _PersonalPhoneNumbersXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[12]/div/field-formatter/span";
            var _NumberofPrivateContactsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[13]/div/field-formatter/span";
            var _NumberofContactsEmailsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[14]/div/field-formatter/span";
            var _NumberofContactsPhonesXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[15]/div/field-formatter/span";
            var _OrganizationHQRegionXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[16]/div/field-formatter/identifier-multi-formatter/span";
            var _OrganizationIndustryXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[17]/div/field-formatter/identifier-multi-formatter/span";
            var _OrganizationNumberofEmployeesXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[18]/div/field-formatter/a";
            var _OrganizationFoundedDateXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[19]/div/field-formatter/a";
            var _OrganizationLastFundingDateXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[20]/div/field-formatter/a";
            var _OrganizationLastFundingAmountXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[21]/div/field-formatter/a";
            var _OrganizationLastFundingTypeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[22]/div/field-formatter/a";
            var _OrganizationLastFundingStatusXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[23]/div/field-formatter/a";
            var _OrganizationTotalFundingAmountXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[24]/div/field-formatter/a";
            var _OrganizationEstimatedRevenueRangeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[25]/div/field-formatter/a";
            var _OrganizationOperatingStatusXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[26]/div/field-formatter/a";
            var _OrganizationAcquisitionStatusXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[27]/div/field-formatter/enum-multi-formatter/a";
            var _OrganizationIPOStatusXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[28]/div/field-formatter/a";
            var _OrganizationTagsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[29]/div/field-formatter/identifier-multi-formatter/span";

            string[] xpaths =
            {
                _NameXPath,
                _OrganizationXPath,
                _JobTitlesXPath,
                _OrganizationHQLocationXPath,
                _LinkedInXPath,
                _JobDepartmentsXPath,
                _JobLevelsXPath,
                _WorkEmailsXPath,
                _PersonalEmailsXPath,
                _WorkPhoneNumbersXPath,
                _PersonalPhoneNumbersXPath,
                _NumberofPrivateContactsXPath,
                _NumberofContactsEmailsXPath,
                _NumberofContactsPhonesXPath,
                _OrganizationHQRegionXPath,
                _OrganizationIndustryXPath,
                _OrganizationNumberofEmployeesXPath,
                _OrganizationFoundedDateXPath,
                _OrganizationLastFundingDateXPath,
                _OrganizationLastFundingAmountXPath,
                _OrganizationLastFundingTypeXPath,
                _OrganizationLastFundingStatusXPath,
                _OrganizationTotalFundingAmountXPath,
                _OrganizationEstimatedRevenueRangeXPath,
                _OrganizationOperatingStatusXPath,
                _OrganizationAcquisitionStatusXPath,
                _OrganizationIPOStatusXPath,
                _OrganizationTagsXPath,
            };

            return xpaths;

        }

        public static void CollectData()
        {
            Console.Write("Enter input file path: ");
            var i_fil = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(i_fil))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output folder path: ");
            var o_fol = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(o_fol))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output file name: ");
            var o_fil = Console.ReadLine()?.Replace(" ", "");

            var paths = File.ReadAllLines(i_fil);
            paths = (from path in paths select path.Replace("\"", "")).ToArray();

            var header = ConfigureHeader();
            var xpaths = ConfigureXPaths();

            var dataframe = new List<string[]>() { header };
            foreach (var path in paths)
            {
                if (!Path.IsPathFullyQualified(path)) continue;
                Console.WriteLine($"Collect: {Path.GetFileName(path)}");

                var document = ReadWebpage(path);
                for (int i = 1; i <= 50; i++)
                {
                    var dataline = new List<string>();
                    foreach (var xpath in xpaths)
                    {
                        var fullXPath = xpath.Replace("{index}", $"{i}");
                        var node = document.DocumentNode.SelectSingleNode(fullXPath);

                        if (xpath == _ContactLinkedInXPath)
                        {
                            try
                            {
                                var href = node.GetAttributeValue("href", "—");
                                href = href.Replace(",", "%2C");
                                dataline.Add(href);
                            }
                            catch
                            {
                                dataline.Add("—");
                            }
                            continue;
                        }

                        try
                        {
                            var text = node.InnerText.Replace("\"", "\"\"").Trim();
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

            o_fil = $"{o_fol}\\Dataset @{o_fil} #-------------- .csv";
            File.WriteAllLines(o_fil, datalines, encoding: Encoding.UTF8);
            var timestamp = File.GetCreationTime(o_fil).ToString("yyyyMMddHHmmss");
            File.Move(o_fil, o_fil.Replace("#--------------", $"#{timestamp}"));
        }
    }

    [Feature]
    internal class CrunchBaseEvent
    {
        private static readonly string _EventRegistrationURLXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[10]/div/field-formatter/link-formatter/a";

        private static HtmlDocument ReadWebpage(string path)
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

        private static string[] ConfigureHeader()
        {
            var header = new List<string>()
            {
                "Event Name",
                "Start Date",
                "End Date",
                "Location",
                "Regions",
                "Description",
                "Event Type",
                "Venue Name",
                "Registration URL",
                "Event URL",
                "Full Description",
                "Industry Group",
                "Industry",
                "Number of Organizers",
                "Number of Speakers",
                "Number of Sponsors",
                "Number of Contestants",
                "Number of Exhibitors",
                "CB Rank (Event)",
            };

            return header.ToArray();
        }

        private static string[] ConfigureXPaths()
        {
            var _EventNameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[2]/div/field-formatter/identifier-formatter/a/div/div";
            var _StartDateXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[3]/div/field-formatter/span";
            var _EndDateXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[4]/div/field-formatter/span";
            var _LocationXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[5]/div/field-formatter/identifier-multi-formatter/span";
            var _RegionsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[6]/div/field-formatter/identifier-multi-formatter/span";
            var _DescriptionXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[7]/div/field-formatter/span";
            var _EventTypeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[8]/div/field-formatter/enum-multi-formatter/span";
            var _VenueNameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[9]/div/field-formatter/span";
            var _RegistrationURLXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[10]/div/field-formatter/link-formatter/a";
            var _EventURLXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[11]/div/field-formatter/link-formatter/a";
            var _FullDescriptionXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[12]/div/field-formatter/span";
            var _IndustryGroupXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[13]/div/field-formatter/identifier-multi-formatter/span";
            var _IndustryXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[14]/div/field-formatter/identifier-multi-formatter/span";
            var _NumOrganizersXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[15]/div/field-formatter/a";
            var _NumSpeakersXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[16]/div/field-formatter/span";
            var _NumSponsorsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[17]/div/field-formatter/span";
            var _NumContestantsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[18]/div/field-formatter/span";
            var _NumExhibitorsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[19]/div/field-formatter/span";
            var _CBRankEventXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[20]/div/field-formatter/a";

            string[] xpaths =
            {
                _EventNameXPath,
                _StartDateXPath,
                _EndDateXPath,
                _LocationXPath,
                _RegionsXPath,
                _DescriptionXPath,
                _EventTypeXPath,
                _VenueNameXPath,
                _RegistrationURLXPath,
                _EventURLXPath,
                _FullDescriptionXPath,
                _IndustryGroupXPath,
                _IndustryXPath,
                _NumOrganizersXPath,
                _NumSpeakersXPath,
                _NumSponsorsXPath,
                _NumContestantsXPath,
                _NumExhibitorsXPath,
                _CBRankEventXPath,
            };

            return xpaths;

        }

        public static void CollectData()
        {
            Console.Write("Enter input file path: ");
            var i_fil = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(i_fil))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output folder path: ");
            var o_fol = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(o_fol))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output file name: ");
            var o_fil = Console.ReadLine()?.Replace(" ", "");

            var paths = File.ReadAllLines(i_fil);
            paths = (from path in paths select path.Replace("\"", "")).ToArray();

            var header = ConfigureHeader();
            var xpaths = ConfigureXPaths();

            var dataframe = new List<string[]>() { header };
            foreach (var path in paths)
            {
                if (!Path.IsPathFullyQualified(path)) continue;
                Console.WriteLine($"Collect: {Path.GetFileName(path)}");

                var document = ReadWebpage(path);
                for (int i = 1; i <= 50; i++)
                {
                    var dataline = new List<string>();
                    foreach (var xpath in xpaths)
                    {
                        var fullXPath = xpath.Replace("{index}", $"{i}");
                        var node = document.DocumentNode.SelectSingleNode(fullXPath);

                        if (xpath == _EventRegistrationURLXPath)
                        {
                            try
                            {
                                var href = node.GetAttributeValue("href", "—");
                                href = href.Replace(",", "%2C");
                                dataline.Add(href);
                            }
                            catch
                            {
                                dataline.Add("—");
                            }
                            continue;
                        }

                        try
                        {
                            var text = node.InnerText.Replace("\"", "\"\"").Trim();
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

            o_fil = $"{o_fol}\\Dataset @{o_fil} #-------------- .csv";
            File.WriteAllLines(o_fil, datalines, encoding: Encoding.UTF8);
            var timestamp = File.GetCreationTime(o_fil).ToString("yyyyMMddHHmmss");
            File.Move(o_fil, o_fil.Replace("#--------------", $"#{timestamp}"));
        }
    }

    [Feature]
    internal class CrunchBaseFunding
    {
        private static HtmlDocument ReadWebpage(string path)
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

        private static string[] ConfigureHeader()
        {
            var header = new List<string>()
            {
                "Transaction Name",
                "Organization Name",
                "Funding Type",
                "Money Raised",
                "Announced Date",
                "Funding Stage",
                "Pre-Money Valuation",
                "Equity Only Funding",

                "Organization Description",
                "Organization Industry",
                "Diversity Spotlight (US Only)",
                "Organization Website",
                "Organization Location",
                "Organization Revenue Range",

                "Total Funding Amount",
                "Funding Status",
                "Number of Funding Rounds",

                "Lead Investors",
                "Investor Names",
                "Number of Investors",
                "Number of Partner Investors",

                "CB Rank (Funding Rounds)",
            };

            return header.ToArray();
        }

        private static string[] ConfigureXPaths()
        {
            var _TransactionNameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[2]/div/field-formatter/identifier-formatter/a/div/div";
            var _OrganizationNameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[3]/div/field-formatter/identifier-formatter/a/div/div";
            var _FundingTypeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[4]/div/field-formatter/span";
            var _MoneyRaisedXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[5]/div/field-formatter/span";
            var _AnnouncedDateXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[6]/div/field-formatter/span";
            var _FundingStageXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[7]/div/field-formatter/span";
            var _PreMoneyValuationXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[8]/div/field-formatter/span";
            var _EquityOnlyFundingXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[9]/div/field-formatter/span";
            
            var _OrganizationDescriptionXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[10]/div/field-formatter/span";
            var _OrganizationIndustryXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[11]/div/field-formatter/identifier-multi-formatter/span";
            var _DiversitySpotlightXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[12]/div/field-formatter/identifier-multi-formatter/span";
            var _OrganizationWebsiteXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[13]/div/field-formatter/link-formatter/a";
            var _OrganizationLocationXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[14]/div/field-formatter/identifier-multi-formatter/span";
            var _OrganizationRevenueRangeXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[15]/div/field-formatter/span";
            
            var _TotalFundingAmountXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[16]/div/field-formatter/span";
            var _FundingStatusXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[17]/div/field-formatter/span";
            var _NumberofFundingRoundsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[18]/div/field-formatter/span";
            
            var _LeadInvestorsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[19]/div/field-formatter/identifier-multi-formatter/span";
            var _InvestorNamesXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[20]/div/field-formatter/identifier-multi-formatter/span";
            var _NumberofInvestorsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[21]/div/field-formatter/a";
            var _NumberofPartnerInvestorsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[22]/div/field-formatter/span";
            
            var _CBRankFundingRoundsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[23]/div/field-formatter/a";

            string[] xpaths =
            {
                _TransactionNameXPath,
                _OrganizationNameXPath,
                _FundingTypeXPath,
                _MoneyRaisedXPath,
                _AnnouncedDateXPath,
                _FundingStageXPath,
                _PreMoneyValuationXPath,
                _EquityOnlyFundingXPath,
                _OrganizationDescriptionXPath,
                _OrganizationIndustryXPath,
                _DiversitySpotlightXPath,
                _OrganizationWebsiteXPath,
                _OrganizationLocationXPath,
                _OrganizationRevenueRangeXPath,
                _TotalFundingAmountXPath,
                _FundingStatusXPath,
                _NumberofFundingRoundsXPath,
                _LeadInvestorsXPath,
                _InvestorNamesXPath,
                _NumberofInvestorsXPath,
                _NumberofPartnerInvestorsXPath,
                _CBRankFundingRoundsXPath,
            };

            return xpaths;
        }

        public static void CollectData()
        {
            Console.Write("Enter input file path: ");
            var i_fil = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(i_fil))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output folder path: ");
            var o_fol = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(o_fol))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output file name: ");
            var o_fil = Console.ReadLine()?.Replace(" ", "");

            var paths = File.ReadAllLines(i_fil);
            paths = (from path in paths select path.Replace("\"", "")).ToArray();

            var header = ConfigureHeader();
            var xpaths = ConfigureXPaths();

            var dataframe = new List<string[]>() { header };
            foreach (var path in paths)
            {
                if (!Path.IsPathFullyQualified(path)) continue;
                Console.WriteLine($"Collect: {Path.GetFileName(path)}");

                var document = ReadWebpage(path);
                for (int i = 1; i <= 50; i++)
                {
                    var dataline = new List<string>();
                    foreach (var xpath in xpaths)
                    {
                        var fullXPath = xpath.Replace("{index}", $"{i}");
                        var node = document.DocumentNode.SelectSingleNode(fullXPath);

                        try
                        {
                            var text = node.InnerText.Replace("\"", "\"\"").Trim();
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

            o_fil = $"{o_fol}\\Dataset @{o_fil} #-------------- .csv";
            File.WriteAllLines(o_fil, datalines, encoding: Encoding.UTF8);
            var timestamp = File.GetCreationTime(o_fil).ToString("yyyyMMddHHmmss");
            File.Move(o_fil, o_fil.Replace("#--------------", $"#{timestamp}"));
        }
    }

    [Feature]
    internal class CrunchBaseHub
    {
        private static HtmlDocument ReadWebpage(string path)
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

        private static string[] ConfigureHeader()
        {
            var header = new List<string>()
            {
                "Hub Name",
                "Number of Organizations",
                "Number of People",
                "Number of Events",
                "CB Rank (Hub)",
                "Description",
                "Last Updated",
                "Hub Type",
                "Parent Organization",
                "Disverity Spotlight (US Only)",
                "Industry",
                "Industry Group",
                "Location",
                "Location Type",
                "Closed Year",
                "Founded Year",
                "Funding Status",
                "Investment Stage",
                "Investor Type",
                "Number of Employees",
                "Estimated Revenue Range",
                "Number of Investors",
                "Number of Lead Investors",
                "Average Founded Date",
                "Number of Non-Profit Companies",
                "Number of For-Profit Companies",
                "Percentage Non-Profit",
                "Percentage Public Organizations",
                "Number of IPOs",
                "Average IPO Date",
                "Percentage Delisted",
                "Top Funding Types",
                "Number of Funding Rounds",
                "Median Number of Funding Rounds",
                "Total Funding Amount",
                "Median Total Funding Amount",
                "Total Equity Funding Amount",
                "Average Last Funding Date",
                "Total Amount Raised in IPO",
                "Median Amount Raised in IPO",
                "Total IPO Valuation",
                "Median IPO Valuation",
                "Percentage Acquired",
                "Number of Acquired Organizations",
                "Number of Acquisitions",
                "Total Acquired Price",
                "Median Acquired Price",
                "Top Investor Types",
                "Number of Portfolio Companies",
                "Number of Investments",
                "Median Number of Investments",
                "Number of Lead Investors",
                "Median Number of Lead Investors",
                "Number of Exits",
                "Median Number of Exits",
                "Number of Funds",
                "Total Fund Raised",
                "Number of Founders",
                "Number of Founder Alumni",
                "Number of Alumni",
                "Average Rank",
                "Average Trend Score (7D)",
                "Average Trend Score (30D)",
                "Average Trend Score (90D)",
            };

            return header.ToArray();
        }

        private static string[] ConfigureXPaths()
        {
            var _HubName = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[2]/div/field-formatter/identifier-formatter/a/div";
            var _NumberOfOrganizations = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[3]/div/field-formatter/a";
            var _NumberOfPeople = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[4]/div/field-formatter/a";
            var _NumberOfEvents = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[5]/div/field-formatter/a";
            var _CBRankHub = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[6]/div/field-formatter/a";
            var _Description = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[7]/div/field-formatter/span";
            var _LastUpdated = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[8]/div/field-formatter/span";
            var _HubType = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[9]/div/field-formatter/enum-multi-formatter/span";
            var _ParentOrganization = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[10]/div/field-formatter/identifier-formatter/span";
            var _DisveritySpotlight = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[11]/div/field-formatter/identifier-multi-formatter/span";
            var _Industry = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[12]/div/field-formatter/identifier-multi-formatter/span";
            var _IndustryGroups = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[13]/div/field-formatter/identifier-multi-formatter/span";
            var _Location = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[14]/div/field-formatter/identifier-multi-formatter/span";
            var _LocationType = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[15]/div/field-formatter/span";
            var _ClosedYear = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[16]/div/field-formatter/span";
            var _FoundedYear = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[17]/div/field-formatter/span";
            var _FundingStatus = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[18]/div/field-formatter/span";
            var _InvestmentStage = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[19]/div/field-formatter/span";
            var _InvestorType = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[20]/div/field-formatter/span";
            var _NumberOfEmployees = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[21]/div/field-formatter/span";
            var _EstimatedRevenueRange = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[22]/div/field-formatter/a";
            var _NumberOfInvestors = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[23]/div/field-formatter/a";
            var _NumberOfLeadInvestors = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[24]/div/field-formatter/a";
            var _AverageFoundedDate = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[25]/div/field-formatter/a";
            var _NumberOfNonProfitCompanies = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[26]/div/field-formatter/a";
            var _NumberOfForProfitCompanies = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[27]/div/field-formatter/a";
            var _PercentageNonProfit = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[28]/div/field-formatter/a";
            var _PercentagePublicOrganizations = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[29]/div/field-formatter/a";
            var _NumberOfIPOs = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[30]/div/field-formatter/a";
            var _AverageIPODate = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[31]/div/field-formatter/a";
            var _PercentageDelisted = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[32]/div/field-formatter/a";
            var _TopFundingTypes = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[33]/div/field-formatter/enum-multi-formatter";
            var _NumberOfFundingRounds = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[34]/div/field-formatter/a";
            var _MedianNumberOfFundingRounds = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[35]/div/field-formatter/a";
            var _TotalFundingAmount = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[36]/div/field-formatter/a";
            var _MedianTotalFundingAmount = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[37]/div/field-formatter/a";
            var _TotalEquityFundingAmount = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[38]/div/field-formatter/a";
            var _AverageLastFundingDate = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[39]/div/field-formatter/a";
            var _TotalAmountRaisedinIPO = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[40]/div/field-formatter/a";
            var _MedianAmountRaisedinIPO = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[41]/div/field-formatter/a";
            var _TotalIPOValuation = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[42]/div/field-formatter/a";
            var _MedianIPOValuation = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[43]/div/field-formatter/a";
            var _PercentageAcquired = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[44]/div/field-formatter/a";
            var _NumberOfAcquiredOrganizations = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[45]/div/field-formatter/a";
            var _NumberOfAcquisitions = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[46]/div/field-formatter/a";
            var _TotalAcquiredPrice = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[47]/div/field-formatter/a";
            var _MedianAcquiredPrice = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[48]/div/field-formatter/a";
            var _TopInvestorTypes = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[49]/div/field-formatter/enum-multi-formatter";
            var _NumberOfPortfolioCompanies = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[50]/div/field-formatter/a";
            var _NumberOfInvestments = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[51]/div/field-formatter/span";
            var _MedianNumberOfInvestments = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[52]/div/field-formatter/span";
            var _NumberOfLeadInvestments = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[53]/div/field-formatter/span";
            var _MedianNumberOfLeadInvestments = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[54]/div/field-formatter/span";
            var _NumberOfExits = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[55]/div/field-formatter/a";
            var _MedianNumberOfExits = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[56]/div/field-formatter/a";
            var _NumberOfFunds = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[57]/div/field-formatter/a";
            var _TotalFundRaised = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[58]/div/field-formatter/a";
            var _NumberOfFounders = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[59]/div/field-formatter/a";
            var _NumberOfFounderAlumni = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[60]/div/field-formatter/a";
            var _NumberOfAlumni = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[61]/div/field-formatter/a";
            var _AverageRank = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[62]/div/field-formatter/a";
            var _AverageTrendScore7D = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[63]/div/field-formatter/a";
            var _AverageTrendScore30D = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[64]/div/field-formatter/a";
            var _AverageTrendScore90D = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[65]/div/field-formatter/a";

            string[] xpaths =
            {
                _HubName,
                _NumberOfOrganizations,
                _NumberOfPeople,
                _NumberOfEvents,
                _CBRankHub,
                _Description,
                _LastUpdated,
                _HubType,
                _ParentOrganization,
                _DisveritySpotlight,
                _Industry,
                _IndustryGroups,
                _Location,
                _LocationType,
                _ClosedYear,
                _FoundedYear,
                _FundingStatus,
                _InvestmentStage,
                _InvestorType,
                _NumberOfEmployees,
                _EstimatedRevenueRange,
                _NumberOfInvestors,
                _NumberOfLeadInvestors,
                _AverageFoundedDate,
                _NumberOfNonProfitCompanies,
                _NumberOfForProfitCompanies,
                _PercentageNonProfit,
                _PercentagePublicOrganizations,
                _NumberOfIPOs,
                _AverageIPODate,
                _PercentageDelisted,
                _TopFundingTypes,
                _NumberOfFundingRounds,
                _MedianNumberOfFundingRounds,
                _TotalFundingAmount,
                _MedianTotalFundingAmount,
                _TotalEquityFundingAmount,
                _AverageLastFundingDate,
                _TotalAmountRaisedinIPO,
                _MedianAmountRaisedinIPO,
                _TotalIPOValuation,
                _MedianIPOValuation,
                _PercentageAcquired,
                _NumberOfAcquiredOrganizations,
                _NumberOfAcquisitions,
                _TotalAcquiredPrice,
                _MedianAcquiredPrice,
                _TopInvestorTypes,
                _NumberOfPortfolioCompanies,
                _NumberOfInvestments,
                _MedianNumberOfInvestments,
                _NumberOfLeadInvestments,
                _MedianNumberOfLeadInvestments,
                _NumberOfExits,
                _MedianNumberOfExits,
                _NumberOfFunds,
                _TotalFundRaised,
                _NumberOfFounders,
                _NumberOfFounderAlumni,
                _NumberOfAlumni,
                _AverageRank,
                _AverageTrendScore7D,
                _AverageTrendScore30D,
                _AverageTrendScore90D,
            };

            return xpaths;
        }

        public static void CollectData()
        {
            Console.Write("Enter input file path: ");
            var i_fil = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(i_fil))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output folder path: ");
            var o_fol = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(o_fol))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output file name: ");
            var o_fil = Console.ReadLine()?.Replace(" ", "");

            var paths = File.ReadAllLines(i_fil);
            paths = (from path in paths select path.Replace("\"", "")).ToArray();

            var header = ConfigureHeader();
            var xpaths = ConfigureXPaths();

            var dataframe = new List<string[]>() { header };
            foreach (var path in paths)
            {
                if (!Path.IsPathFullyQualified(path)) continue;
                Console.WriteLine($"Collect: {Path.GetFileName(path)}");

                var document = ReadWebpage(path);
                for (int i = 1; i <= 50; i++)
                {
                    var dataline = new List<string>();
                    foreach (var xpath in xpaths)
                    {
                        var fullXPath = xpath.Replace("{index}", $"{i}");
                        var node = document.DocumentNode.SelectSingleNode(fullXPath);

                        try
                        {
                            var text = node.InnerText.Replace("\"", "\"\"").Trim();
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

            o_fil = $"{o_fol}\\Dataset @{o_fil} #-------------- .csv";
            File.WriteAllLines(o_fil, datalines, encoding: Encoding.UTF8);
            var timestamp = File.GetCreationTime(o_fil).ToString("yyyyMMddHHmmss");
            File.Move(o_fil, o_fil.Replace("#--------------", $"#{timestamp}"));
        }
    }

    [Feature]
    internal class CrunchBaseInvestor
    {
        private static readonly string _InvestorTwitterXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[13]/div/field-formatter/link-formatter/a";
        private static readonly string _InvestorLinkedInXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[14]/div/field-formatter/link-formatter/a";
        private static readonly string _InvestorFacebookXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[15]/div/field-formatter/link-formatter/a";

        private static HtmlDocument ReadWebpage(string path)
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

        private static string[] ConfigureHeader()
        {
            var header = new List<string>()
            {
                "Investor Name",
                "Number of Investments",
                "Number of Exits",
                "Location",
                "Description",
                "First Name",
                "Last Name",
                "Regions",
                "Gender",
                "Full Description",
                "Number of Articles",
                "Twitter",
                "LinkedIn",
                "Facebook",
                "Primary Job Title",
                "Primary Organization",
                "Number of Organization Founded",
                "Number of Portfolio Organizations",
                "Number of Partner Investments",
                "Number of Lead Investments",
                "Number of Diversity Investments",
                "Number of Exits (IPO)",
                "Number of Events",
                "CB Rank (Person)",
                "Trend Score (7D)",
                "Trend Score (30D)",
                "Trend Score (90D)",
                "Diversity Spotlight (US Only)",
                "Estimated Revenue Range",
                "Operating Status",
                "Founded Date",
                "Exit Date",
                "Closed Date",
                "Company Type",
                "Website",
                "Contact Email",
                "Phone Number",
                "Hub Tags",
                "Actively Hiring",
                "Investment Stage",
                "Investor Type",
                "Accelerator Next Application Deadline",
                "Accelerator Program Type",
                "Accelerator Duration (Week)",
                "Most Recent Valuation Range",
                "Date of Most Recent Valuation",
                "IT Spend",
                "Patents Granted",
                "Trademark Registerd",
                "Most Popular Patent Class",
                "Most Popular Trademark Class",
                "Total Products Active",
                "Number of Apps",
                "Downloads Last 30D",
                "Active Tech Count",
                "Monthly Visits",
                "Average Visits (6M)",
                "Monthly Visits Growth",
                "Visit Duration",
                "Visit Duration Growth",
                "Page Views Visit",
                "Page Views Visit Growth",
                "Bounce Rate",
                "Bounce Rate Growth",
                "Global Traffic Rank",
                "Monthly Rank Change",
                "Monthly Rank Growth",
                "Contact Job Departments",
                "Number of Contacts",
                "CB Rank (Investor)",
                "CB Rank (School)",
                "CB Rank (Organization)",
                "IPO Status",
                "IPO Date",
                "Delisted Date",
                "Monthly Raised IPO",
                "Valuation at IPO",
                "Stock Symbol",
                "Stock Exchange",
                "Acquisition Status",
                "Top 5 Investors",
                "Number of Lead Investors",
                "Number of Investors",
                "Number of Funding Rounds",
                "Funding Status",
                "Last Funding Date",
                "Last Funding Amount",
                "Last Funding Type",
                "Last Equity Funding Amount",
                "Last Equity Funding Type",
                "Total Equity Funding Amount",
                "Total Funding Amount",
                "Number of Founders",
                "Founders",
                "Number of Employees",
                "Industry Group",
                "Industry",
                "School Type",
                "School Program",
                "Number of Enrollments",
                "School Method",
                "Number of Alumni",
            };

            return header.ToArray();
        }

        private static string[] ConfigureXPaths()
        {       
            var _InvestorName = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[2]/div/field-formatter/identifier-formatter/a/div/div";
            var _NumberOfInvestments = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[3]/div/field-formatter/a";
            var _NumberOfExits = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[4]/div/field-formatter/a";
            var _Location = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[5]/div/field-formatter/identifier-multi-formatter/span";
            var _Description = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[6]/div/field-formatter/span";
            var _FirstName = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[7]/div/field-formatter/span";
            var _LastName = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[8]/div/field-formatter/span";
            var _Regions = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[9]/div/field-formatter/identifier-multi-formatter/span";
            var _Gender = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[10]/div/field-formatter/span";
            var _FullDescription = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[11]/div/field-formatter/span";
            var _NumberOfArticles = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[12]/div/field-formatter/a";
            var _Twitter = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[13]/div/field-formatter/link-formatter/a";
            var _LinkedIn = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[14]/div/field-formatter/link-formatter/a";
            var _Facebook = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[15]/div/field-formatter/link-formatter/a";
            var _PrimaryJobTitle = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[16]/div/field-formatter/span";
            var _PrimaryOrganization = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[17]/div/field-formatter/identifier-formatter/span";
            var _NumberOfOrganizationFounded = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[18]/div/field-formatter/span";
            var _NumberOfPortfolioOrganizations = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[19]/div/field-formatter/a";
            var _NumberOfPartnerInvestments = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[20]/div/field-formatter/span";
            var _NumberOfLeadInvestments = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[21]/div/field-formatter/a";
            var _NumberOfDiversityInvestments = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[22]/div/field-formatter/a";
            var _NumberOfExitsIPO = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[23]/div/field-formatter/a";
            var _NumberOfEvents = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[24]/div/field-formatter/a";
            var _CBRankPerson = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[25]/div/field-formatter/span";
            var _TrendScore7D = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[26]/div/field-formatter/span";
            var _TrendScore30D = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[27]/div/field-formatter/span";
            var _TrendScore90D = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[28]/div/field-formatter/span";
            var _DiversitySpotlight = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[29]/div/field-formatter/identifier-multi-formatter/span";
            var _EstimatedRevenueRange = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[30]/div/field-formatter/a";
            var _OperatingStatus = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[31]/div/field-formatter/span";
            var _FoundedDate = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[32]/div/field-formatter/span";
            var _ExitDate = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[33]/div/field-formatter/span";
            var _ClosedDate = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[34]/div/field-formatter/span";
            var _CompanyType = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[35]/div/field-formatter/span";
            var _Website = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[36]/div/field-formatter/link-formatter/a";
            var _ContactEmail = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[37]/div/field-formatter/blob-formatter/span";
            var _PhoneNumber = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[38]/div/field-formatter/blob-formatter/span";
            var _HubTags = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[39]/div/field-formatter/enum-multi-formatter/span";
            var _ActivelyHiring = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[40]/div/field-formatter/span";
            var _InvestmentStage = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[41]/div/field-formatter/enum-multi-formatter/span";
            var _InvestorType = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[42]/div/field-formatter/enum-multi-formatter/span";
            var _AcceleratorNextApplicationDeadline = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[43]/div/field-formatter/span";
            var _AcceleratorProgramType = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[44]/div/field-formatter/span";
            var _AcceleratorDuration = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[45]/div/field-formatter/span";
            var _MostRecentValuationRange = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[46]/div/field-formatter/span";
            var _DateOfMostRecentValuation = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[47]/div/field-formatter/span";
            var _ITSpend = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[48]/div/field-formatter/a";
            var _PatentsGranted = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[49]/div/field-formatter/a";
            var _TrademarkRegisterd = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[50]/div/field-formatter/a";
            var _MostPopularPatentClass = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[51]/div/field-formatter/span";
            var _MostPopularTrademarkClass = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[52]/div/field-formatter/a";
            var _TotalProductsActive = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[53]/div/field-formatter/span";
            var _NumberOfApps = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[54]/div/field-formatter/span";
            var _DownloadsLast30D = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[55]/div/field-formatter/span";
            var _ActiveTechCount = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[56]/div/field-formatter/span";
            var _MonthlyVisits = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[57]/div/field-formatter/span";
            var _AverageVisits6M = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[58]/div/field-formatter/span";
            var _MonthlyVisitsGrowth = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[59]/div/field-formatter/span";
            var _VisitDuration = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[60]/div/field-formatter/span";
            var _VisitDurationGrowth = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[61]/div/field-formatter/span";
            var _PageViewsVisit = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[62]/div/field-formatter/span";
            var _PageViewsVisitGrowth = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[63]/div/field-formatter/span";
            var _BounceRate = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[64]/div/field-formatter/span";
            var _BounceRateGrowth = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[65]/div/field-formatter/span";
            var _GlobalTrafficRank = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[66]/div/field-formatter/span";
            var _MonthlyRankChange = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[67]/div/field-formatter/span";
            var _MonthlyRankGrowth = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[68]/div/field-formatter/span";
            var _ContactJobDepartments = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[69]/div/field-formatter/enum-multi-formatter/span";
            var _NumberOfContacts = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[70]/div/field-formatter/a";
            var _CBRankInvestor = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[71]/div/field-formatter/a";
            var _CBRankSchool = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[72]/div/field-formatter/a";
            var _CBRankOrganization = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[74]/div/field-formatter/a";
            var _IPOStatus = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[75]/div/field-formatter/span";
            var _IPODate = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[76]/div/field-formatter/span";
            var _DelistedDate = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[77]/div/field-formatter/span";
            var _MonthlyRaisedIPO = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[78]/div/field-formatter/span";
            var _ValuationatIPO = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[79]/div/field-formatter/span";
            var _StockSymbol = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[80]/div/field-formatter/identifier-formatter/span";
            var _StockExchange = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[81]/div/field-formatter/span";
            var _AcquisitionStatus = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[82]/div/field-formatter/enum-multi-formatter/span";
            var _Top5Investors = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[83]/div/field-formatter/identifier-multi-formatter/span";
            var _NumberOfLeadInvestors = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[84]/div/field-formatter/span";
            var _NumberOfInvestors = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[85]/div/field-formatter/span";
            var _NumberOfFundingRounds = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[86]/div/field-formatter/a";
            var _FundingStatus = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[87]/div/field-formatter/span";
            var _LastFundingDate = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[88]/div/field-formatter/a";
            var _LastFundingAmount = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[89]/div/field-formatter/span";
            var _LastFundingType = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[90]/div/field-formatter/a";
            var _LastEquityFundingAmount = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[91]/div/field-formatter/span";
            var _LastEquityFundingType = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[92]/div/field-formatter/a";
            var _TotalEquityFundingAmount = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[93]/div/field-formatter/span";
            var _TotalFundingAmount = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[94]/div/field-formatter/span";
            var _NumberOfFounders = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[95]/div/field-formatter/a";
            var _Founders = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[96]/div/field-formatter/identifier-multi-formatter/span";
            var _NumberOfEmployees = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[97]/div/field-formatter/a";
            var _IndustryGroups = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[98]/div/field-formatter/identifier-multi-formatter/span";
            var _Industry = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[99]/div/field-formatter/identifier-multi-formatter/span";
            var _SchoolType = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[100]/div/field-formatter/span";
            var _SchoolProgram = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[101]/div/field-formatter/span";
            var _NumberOfEnrollments = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[102]/div/field-formatter/span";
            var _SchoolMethod = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[103]/div/field-formatter/span";
            var _NumberOfAlumni = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[104]/div/field-formatter/span";

            string[] xpaths =
            {
                _InvestorName,
                _NumberOfInvestments,
                _NumberOfExits,
                _Location,
                _Description,
                _FirstName,
                _LastName,
                _Regions,
                _Gender,
                _FullDescription,
                _NumberOfArticles,
                _Twitter,
                _LinkedIn,
                _Facebook,
                _PrimaryJobTitle,
                _PrimaryOrganization,
                _NumberOfOrganizationFounded,
                _NumberOfPortfolioOrganizations,
                _NumberOfPartnerInvestments,
                _NumberOfLeadInvestments,
                _NumberOfDiversityInvestments,
                _NumberOfExitsIPO,
                _NumberOfEvents,
                _CBRankPerson,
                _TrendScore7D,
                _TrendScore30D,
                _TrendScore90D,
                _DiversitySpotlight,
                _EstimatedRevenueRange,
                _OperatingStatus,
                _FoundedDate,
                _ExitDate,
                _ClosedDate,
                _CompanyType,
                _Website,
                _ContactEmail,
                _PhoneNumber,
                _HubTags,
                _ActivelyHiring,
                _InvestmentStage,
                _InvestorType,
                _AcceleratorNextApplicationDeadline,
                _AcceleratorProgramType,
                _AcceleratorDuration,
                _MostRecentValuationRange,
                _DateOfMostRecentValuation,
                _ITSpend,
                _PatentsGranted,
                _TrademarkRegisterd,
                _MostPopularPatentClass,
                _MostPopularTrademarkClass,
                _TotalProductsActive,
                _NumberOfApps,
                _DownloadsLast30D,
                _ActiveTechCount,
                _MonthlyVisits,
                _AverageVisits6M,
                _MonthlyVisitsGrowth,
                _VisitDuration,
                _VisitDurationGrowth,
                _PageViewsVisit,
                _PageViewsVisitGrowth,
                _BounceRate,
                _BounceRateGrowth,
                _GlobalTrafficRank,
                _MonthlyRankChange,
                _MonthlyRankGrowth,
                _ContactJobDepartments,
                _NumberOfContacts,
                _CBRankInvestor,
                _CBRankSchool,
                _CBRankOrganization,
                _IPOStatus,
                _IPODate,
                _DelistedDate,
                _MonthlyRaisedIPO,
                _ValuationatIPO,
                _StockSymbol,
                _StockExchange,
                _AcquisitionStatus,
                _Top5Investors,
                _NumberOfLeadInvestors,
                _NumberOfInvestors,
                _NumberOfFundingRounds,
                _FundingStatus,
                _LastFundingDate,
                _LastFundingAmount,
                _LastFundingType,
                _LastEquityFundingAmount,
                _LastEquityFundingType,
                _TotalEquityFundingAmount,
                _TotalFundingAmount,
                _NumberOfFounders,
                _Founders,
                _NumberOfEmployees,
                _IndustryGroups,
                _Industry,
                _SchoolType,
                _SchoolProgram,
                _NumberOfEnrollments,
                _SchoolMethod,
                _NumberOfAlumni,
            };

            return xpaths;
        }

        public static void CollectData()
        {
            Console.Write("Enter input file path: ");
            var i_fil = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(i_fil))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output folder path: ");
            var o_fol = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(o_fol))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output file name: ");
            var o_fil = Console.ReadLine()?.Replace(" ", "");

            var paths = File.ReadAllLines(i_fil);
            paths = (from path in paths select path.Replace("\"", "")).ToArray();

            var header = ConfigureHeader();
            var xpaths = ConfigureXPaths();

            var dataframe = new List<string[]>() { header };
            foreach (var path in paths)
            {
                if (!Path.IsPathFullyQualified(path)) continue;
                Console.WriteLine($"Collect: {Path.GetFileName(path)}");

                var document = ReadWebpage(path);
                for (int i = 1; i <= 50; i++)
                {
                    var dataline = new List<string>();
                    foreach (var xpath in xpaths)
                    {
                        var fullXPath = xpath.Replace("{index}", $"{i}");
                        var node = document.DocumentNode.SelectSingleNode(fullXPath);

                        if (xpath == _InvestorTwitterXPath || xpath == _InvestorFacebookXPath || xpath == _InvestorLinkedInXPath)
                        {
                            try
                            {
                                var href = node.GetAttributeValue("href", "—");
                                href = href.Replace(",", "%2C");
                                dataline.Add(href);
                            }
                            catch
                            {
                                dataline.Add("—");
                            }
                            continue;
                        }

                        try
                        {
                            var text = node.InnerText.Replace("\"", "\"\"").Trim();
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

            o_fil = $"{o_fol}\\Dataset @{o_fil} #-------------- .csv";
            File.WriteAllLines(o_fil, datalines, encoding: Encoding.UTF8);
            var timestamp = File.GetCreationTime(o_fil).ToString("yyyyMMddHHmmss");
            File.Move(o_fil, o_fil.Replace("#--------------", $"#{timestamp}"));
        }
    }

    [Feature]
    internal class CrunchBasePeople
    {
        private static readonly string _PersonalTwitterXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[12]/div/field-formatter/link-formatter/a";
        private static readonly string _PersonalLinkedInXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[13]/div/field-formatter/link-formatter/a";
        private static readonly string _PersonalFacebookXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[14]/div/field-formatter/link-formatter/a";

        private static HtmlDocument ReadWebpage(string path)
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

        private static string[] ConfigureHeader()
        {
            var header = new List<string>()
            {
                "Full Name",
                "Primary Job Title",
                "Primary Organization",
                "Location",
                "CB Rank (Person)",
                "First Name",
                "Last Name",
                "Gender",
                "Regions",
                "Biography",
                "Twitter",
                "LinkedIn",
                "Facebook",
                "Number of News Article",
                "School Attended",
                "Number of Founded Organizations",
                "Current Organizations",
                "Number of Portfolio Companies",
                "Number of Investments",
                "Number of Partner Investments",
                "Number of Diversity Investments",
                "Number of Lead Investments",
                "Number of Exits",
                "Number of Exits (IPO)",
                "Number of Events",
                "Number of Trend Score (7D)",
                "Number of Trend Score (30D)",
                "Number of Trend Score (90D)",
            };

            return header.ToArray();
        }

        private static string[] ConfigureXPaths()
        {
            var _FullNameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[2]/div/field-formatter/identifier-formatter/a/div/div";
            var _PrimaryJobTitleXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[3]/div/field-formatter/span";
            var _PrimaryOrganizationXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[4]/div/field-formatter/identifier-formatter/a/div/div";
            var _LocationXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[5]/div/field-formatter/identifier-multi-formatter/span";
            var _CBRankPersonXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[6]/div/field-formatter/a";
            var _FirstNameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[7]/div/field-formatter/span";
            var _LastNameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[8]/div/field-formatter/span";
            var _GenderXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[9]/div/field-formatter/span";
            var _RegionsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[10]/div/field-formatter/identifier-multi-formatter/span";
            var _BiographyXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[11]/div/field-formatter/span";
            var _TwitterXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[12]/div/field-formatter/link-formatter/a";
            var _LinkedInXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[13]/div/field-formatter/link-formatter/a";
            var _FacebookXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[14]/div/field-formatter/link-formatter/a";
            var _NumberOfNewsArticleXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[15]/div/field-formatter/a";
            var _SchoolAttendedXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[16]/div/field-formatter/identifier-multi-formatter/span/a";
            var _NumberOfFoundedOrganizationsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[17]/div/field-formatter/a";
            var _CurrentOrganizationsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[18]/div/field-formatter/identifier-multi-formatter/span";
            var _NumberOfPortfolioCompaniesXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[19]/div/field-formatter/a";
            var _NumberOfInvestmentsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[20]/div/field-formatter/a";
            var _NumberOfPartnerInvestmentsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[21]/div/field-formatter/a";
            var _NumberOfDiversityInvestmentsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[22]/div/field-formatter/a";
            var _NumberOfLeadInvestmentsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[23]/div/field-formatter/a";
            var _NumberOfExitsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[24]/div/field-formatter/a";
            var _NumberOfExitsIPOXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[25]/div/field-formatter/a";
            var _NumberOfEventsXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[26]/div/field-formatter/span";
            var _NumberOfTrendScore7DXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[27]/div/field-formatter/span";
            var _NumberOfTrendScore30DXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[28]/div/field-formatter/span";
            var _NumberOfTrendScore90DXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[29]/div/field-formatter/span";

            string[] xpaths =
            {
                _FullNameXPath,
                _PrimaryJobTitleXPath,
                _PrimaryOrganizationXPath,
                _LocationXPath,
                _CBRankPersonXPath,
                _FirstNameXPath,
                _LastNameXPath,
                _GenderXPath,
                _RegionsXPath,
                _BiographyXPath,
                _TwitterXPath,
                _LinkedInXPath,
                _FacebookXPath,
                _NumberOfNewsArticleXPath,
                _SchoolAttendedXPath,
                _NumberOfFoundedOrganizationsXPath,
                _CurrentOrganizationsXPath,
                _NumberOfPortfolioCompaniesXPath,
                _NumberOfInvestmentsXPath,
                _NumberOfPartnerInvestmentsXPath,
                _NumberOfDiversityInvestmentsXPath,
                _NumberOfLeadInvestmentsXPath,
                _NumberOfExitsXPath,
                _NumberOfExitsIPOXPath,
                _NumberOfEventsXPath,
                _NumberOfTrendScore7DXPath,
                _NumberOfTrendScore30DXPath,
                _NumberOfTrendScore90DXPath,
            };

            return xpaths;
        }

        public static void CollectData()
        {
            Console.Write("Enter input file path: ");
            var i_fil = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(i_fil))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output folder path: ");
            var o_fol = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(o_fol))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output file name: ");
            var o_fil = Console.ReadLine()?.Replace(" ", "");

            var paths = File.ReadAllLines(i_fil);
            paths = (from path in paths select path.Replace("\"", "")).ToArray();

            var header = ConfigureHeader();
            var xpaths = ConfigureXPaths();

            var dataframe = new List<string[]>() { header };
            foreach (var path in paths)
            {
                if (!Path.IsPathFullyQualified(path)) continue;
                Console.WriteLine($"Collect: {Path.GetFileName(path)}");

                var document = ReadWebpage(path);
                for (int i = 1; i <= 50; i++)
                {
                    var dataline = new List<string>();
                    foreach (var xpath in xpaths)
                    {
                        var fullXPath = xpath.Replace("{index}", $"{i}");
                        var node = document.DocumentNode.SelectSingleNode(fullXPath);

                        if (xpath == _PersonalTwitterXPath || xpath == _PersonalFacebookXPath || xpath == _PersonalLinkedInXPath)
                        {
                            try
                            {
                                var href = node.GetAttributeValue("href", "—");
                                href = href.Replace(",", "%2C");
                                dataline.Add(href);
                            }
                            catch
                            {
                                dataline.Add("—");
                            }
                            continue;
                        }

                        try
                        {
                            var text = node.InnerText.Replace("\"", "\"\"").Trim();
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

            o_fil = $"{o_fol}\\Dataset @{o_fil} #-------------- .csv";
            File.WriteAllLines(o_fil, datalines, encoding: Encoding.UTF8);
            var timestamp = File.GetCreationTime(o_fil).ToString("yyyyMMddHHmmss");
            File.Move(o_fil, o_fil.Replace("#--------------", $"#{timestamp}"));
        }
    }

    [Feature]
    internal class CrunchBaseSchool
    {
        private static readonly string _SchoolTwitterXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[16]/div/field-formatter/link-formatter/a";
        private static readonly string _SchoolFacebookXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[17]/div/field-formatter/link-formatter/a";
        private static readonly string _SchoolLinkedInXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[18]/div/field-formatter/link-formatter/a";

        private static HtmlDocument ReadWebpage(string path)
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

        private static string[] ConfigureHeader()
        {
            var header = new List<string>()
            {
                "Organization Name",
                "Founded Year",
                "Industry",
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
                "Industry Group",
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
                "Page Views Visit",
                "Page Views Visit Growth",
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
                "Number of Investments",
                "Number of Lead Investments",
                "Number of Diversity Investments",
                "Number of Exits",
                "Number of Exits (IPO)",
                "Accelerator Program Type",
                "Accelerator Application Deadline",
                "Accelerator Duration (Week)",
                "Number of Alumni",
                "Number of Private Contacts",
                "Number of Private Notes",
                "Tags",
            };

            return header.ToArray();
        }

        private static string[] ConfigureXPaths()
        {
            var _NameXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[2]/div/field-formatter/identifier-formatter/a/div/div";
            var _FoundedXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[3]/div/field-formatter/span";
            var _IndustryXPath = "/html/body/chrome/div/mat-sidenav-container/mat-sidenav-content/div/discover/page-layout/div/div/div[2]/section[2]/results/div/div/div[2]/sheet-grid/div/div/grid-body/div/grid-row[{index}]/grid-cell[4]/div/field-formatter/identifier-multi-formatter/span";
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
                _NameXPath, _FoundedXPath, _IndustryXPath, _HQLocationXPath, _DescriptionXPath, _CBRankCompanyXPath, _HQRegionXPath,
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

        public static void CollectData()
        {
            Console.Write("Enter input file path: ");
            var i_fil = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(i_fil))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output folder path: ");
            var o_fol = Console.ReadLine()?.Replace("\"", "");

            if (string.IsNullOrEmpty(o_fol))
                throw new ArgumentNullException("path is null or empty");

            Console.Write("Enter output file name: ");
            var o_fil = Console.ReadLine()?.Replace(" ", "");

            var paths = File.ReadAllLines(i_fil);
            paths = (from path in paths select path.Replace("\"", "")).ToArray();

            var header = ConfigureHeader();
            var xpaths = ConfigureXPaths();

            var dataframe = new List<string[]>() { header };
            foreach (var path in paths)
            {
                if (!Path.IsPathFullyQualified(path)) continue;
                Console.WriteLine($"Collect: {Path.GetFileName(path)}");

                var document = ReadWebpage(path);
                for (int i = 1; i <= 50; i++)
                {
                    var dataline = new List<string>();
                    foreach (var xpath in xpaths)
                    {
                        var fullXPath = xpath.Replace("{index}", $"{i}");
                        var node = document.DocumentNode.SelectSingleNode(fullXPath);

                        if (xpath == _SchoolTwitterXPath || xpath == _SchoolFacebookXPath || xpath == _SchoolLinkedInXPath)
                        {
                            try
                            {
                                var href = node.GetAttributeValue("href", "—");
                                href = href.Replace(",", "%2C");
                                dataline.Add(href);
                            }
                            catch
                            {
                                dataline.Add("—");
                            }
                            continue;
                        }

                        try
                        {
                            var text = node.InnerText.Replace("\"", "\"\"").Trim();
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

            o_fil = $"{o_fol}\\Dataset @{o_fil} #-------------- .csv";
            File.WriteAllLines(o_fil, datalines, encoding: Encoding.UTF8);
            var timestamp = File.GetCreationTime(o_fil).ToString("yyyyMMddHHmmss");
            File.Move(o_fil, o_fil.Replace("#--------------", $"#{timestamp}"));
        }
    }
}

using DxMLEngine.Features.GooglePatents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.YahooFinance
{
    public class Summary
    {
        public Quotesummary? quoteSummary { get; set; }

        public class Quotesummary
        {
            public Result[]? result { get; set; }
            public object? error { get; set; }
        }

        public class Result
        {
            public Assetprofile? assetProfile { get; set; }
            public Recommendationtrend? recommendationTrend { get; set; }
            public Majorholdersbreakdown? majorHoldersBreakdown { get; set; }
            public Earningshistory? earningsHistory { get; set; }
            public Indextrend? indexTrend { get; set; }
            public Defaultkeystatistics? defaultKeyStatistics { get; set; }
            public Industrytrend? industryTrend { get; set; }
            public Netsharepurchaseactivity? netSharePurchaseActivity { get; set; }
            public Sectortrend? sectorTrend { get; set; }
            public Insiderholders? insiderHolders { get; set; }
            public Earnings? earnings { get; set; }
            public Upgradedowngradehistory? upgradeDowngradeHistory { get; set; }
            public Earningstrend? earningsTrend { get; set; }
            public Financialdata? financialData { get; set; }
        }

        public class Assetprofile
        {
            public string? address1 { get; set; }
            public string? city { get; set; }
            public string? state { get; set; }
            public string? zip { get; set; }
            public string? country { get; set; }
            public string? phone { get; set; }
            public string? fax { get; set; }
            public string? website { get; set; }
            public string? industry { get; set; }
            public string? sector { get; set; }
            public string? longBusinessSummary { get; set; }
            public int fullTimeEmployees { get; set; }
            public Companyofficer[]? companyOfficers { get; set; }
            public int auditRisk { get; set; }
            public int boardRisk { get; set; }
            public int compensationRisk { get; set; }
            public int shareHolderRightsRisk { get; set; }
            public int overallRisk { get; set; }
            public int governanceEpochDate { get; set; }
            public int compensationAsOfEpochDate { get; set; }
            public int maxAge { get; set; }
        }

        public class Companyofficer
        {
            public int maxAge { get; set; }
            public string? name { get; set; }
            public int age { get; set; }
            public string? title { get; set; }
            public int yearBorn { get; set; }
            public int fiscalYear { get; set; }
            public Totalpay? totalPay { get; set; }
            public Exercisedvalue? exercisedValue { get; set; }
            public Unexercisedvalue? unexercisedValue { get; set; }
        }

        public class Totalpay
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Exercisedvalue
        {
            public int raw { get; set; }
            public object? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Unexercisedvalue
        {
            public int raw { get; set; }
            public object? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Recommendationtrend
        {
            public Trend[]? trend { get; set; }
            public int maxAge { get; set; }
        }

        public class Trend
        {
            public string? period { get; set; }
            public int strongBuy { get; set; }
            public int buy { get; set; }
            public int hold { get; set; }
            public int sell { get; set; }
            public int strongSell { get; set; }
        }

        public class Majorholdersbreakdown
        {
            public int maxAge { get; set; }
            public Insiderspercentheld? insidersPercentHeld { get; set; }
            public Institutionspercentheld? institutionsPercentHeld { get; set; }
            public Institutionsfloatpercentheld? institutionsFloatPercentHeld { get; set; }
            public Institutionscount? institutionsCount { get; set; }
        }

        public class Insiderspercentheld
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Institutionspercentheld
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Institutionsfloatpercentheld
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Institutionscount
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Earningshistory
        {
            public History[]? history { get; set; }
            public int maxAge { get; set; }
        }

        public class History
        {
            public int maxAge { get; set; }
            public Epsactual? epsActual { get; set; }
            public Epsestimate? epsEstimate { get; set; }
            public Epsdifference? epsDifference { get; set; }
            public Surprisepercent? surprisePercent { get; set; }
            public Quarter? quarter { get; set; }
            public string? period { get; set; }
        }

        public class Epsactual
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Epsestimate
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Epsdifference
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Surprisepercent
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Quarter
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Indextrend
        {
            public int maxAge { get; set; }
            public string? symbol { get; set; }
            public Peratio? peRatio { get; set; }
            public Pegratio? pegRatio { get; set; }
            public Estimate[]? estimates { get; set; }
        }

        public class Peratio
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Pegratio
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Estimate
        {
            public string? period { get; set; }
            public Growth? growth { get; set; }
        }

        public class Growth
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Defaultkeystatistics
        {
            public int maxAge { get; set; }
            public Pricehint? priceHint { get; set; }
            public Enterprisevalue? enterpriseValue { get; set; }
            public Forwardpe? forwardPE { get; set; }
            public Profitmargins? profitMargins { get; set; }
            public Floatshares? floatShares { get; set; }
            public Sharesoutstanding? sharesOutstanding { get; set; }
            public Sharesshort? sharesShort { get; set; }
            public Sharesshortpriormonth? sharesShortPriorMonth { get; set; }
            public Sharesshortpreviousmonthdate? sharesShortPreviousMonthDate { get; set; }
            public Dateshortinterest? dateShortInterest { get; set; }
            public Sharespercentsharesout? sharesPercentSharesOut { get; set; }
            public Heldpercentinsiders? heldPercentInsiders { get; set; }
            public Heldpercentinstitutions? heldPercentInstitutions { get; set; }
            public Shortratio? shortRatio { get; set; }
            public Shortpercentoffloat? shortPercentOfFloat { get; set; }
            public Beta? beta { get; set; }
            public Impliedsharesoutstanding? impliedSharesOutstanding { get; set; }
            public Morningstaroverallrating? morningStarOverallRating { get; set; }
            public Morningstarriskrating? morningStarRiskRating { get; set; }
            public object? category { get; set; }
            public Bookvalue? bookValue { get; set; }
            public Pricetobook? priceToBook { get; set; }
            public Annualreportexpenseratio? annualReportExpenseRatio { get; set; }
            public Ytdreturn? ytdReturn { get; set; }
            public Beta3year? beta3Year { get; set; }
            public Totalassets? totalAssets { get; set; }
            public Yield? yield { get; set; }
            public object? fundFamily { get; set; }
            public Fundinceptiondate? fundInceptionDate { get; set; }
            public object? legalType { get; set; }
            public Threeyearaveragereturn? threeYearAverageReturn { get; set; }
            public Fiveyearaveragereturn? fiveYearAverageReturn { get; set; }
            public Pricetosalestrailing12months? priceToSalesTrailing12Months { get; set; }
            public Lastfiscalyearend? lastFiscalYearEnd { get; set; }
            public Nextfiscalyearend? nextFiscalYearEnd { get; set; }
            public Mostrecentquarter? mostRecentQuarter { get; set; }
            public Earningsquarterlygrowth? earningsQuarterlyGrowth { get; set; }
            public Revenuequarterlygrowth? revenueQuarterlyGrowth { get; set; }
            public Netincometocommon? netIncomeToCommon { get; set; }
            public Trailingeps? trailingEps { get; set; }
            public Forwardeps? forwardEps { get; set; }
            public Pegratio1? pegRatio { get; set; }
            public string? lastSplitFactor { get; set; }
            public Lastsplitdate? lastSplitDate { get; set; }
            public Enterprisetorevenue? enterpriseToRevenue { get; set; }
            public Enterprisetoebitda? enterpriseToEbitda { get; set; }
            public _52Weekchange? _52WeekChange { get; set; }
            public Sandp52weekchange? SandP52WeekChange { get; set; }
            public Lastdividendvalue? lastDividendValue { get; set; }
            public Lastdividenddate? lastDividendDate { get; set; }
            public Lastcapgain? lastCapGain { get; set; }
            public Annualholdingsturnover? annualHoldingsTurnover { get; set; }
        }

        public class Pricehint
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Enterprisevalue
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Forwardpe
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Profitmargins
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Floatshares
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Sharesoutstanding
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Sharesshort
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Sharesshortpriormonth
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Sharesshortpreviousmonthdate
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Dateshortinterest
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Sharespercentsharesout
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Heldpercentinsiders
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Heldpercentinstitutions
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Shortratio
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Shortpercentoffloat
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Beta
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Impliedsharesoutstanding
        {
            public int raw { get; set; }
            public object? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Morningstaroverallrating
        {
        }

        public class Morningstarriskrating
        {
        }

        public class Bookvalue
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Pricetobook
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Annualreportexpenseratio
        {
        }

        public class Ytdreturn
        {
        }

        public class Beta3year
        {
        }

        public class Totalassets
        {
        }

        public class Yield
        {
        }

        public class Fundinceptiondate
        {
        }

        public class Threeyearaveragereturn
        {
        }

        public class Fiveyearaveragereturn
        {
        }

        public class Pricetosalestrailing12months
        {
        }

        public class Lastfiscalyearend
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Nextfiscalyearend
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Mostrecentquarter
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Earningsquarterlygrowth
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Revenuequarterlygrowth
        {
        }

        public class Netincometocommon
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Trailingeps
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Forwardeps
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Pegratio1
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Lastsplitdate
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Enterprisetorevenue
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Enterprisetoebitda
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class _52Weekchange
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Sandp52weekchange
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Lastdividendvalue
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Lastdividenddate
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Lastcapgain
        {
        }

        public class Annualholdingsturnover
        {
        }

        public class Industrytrend
        {
            public int maxAge { get; set; }
            public object? symbol { get; set; }
            public Peratio1? peRatio { get; set; }
            public Pegratio2? pegRatio { get; set; }
            public object?[]? estimates { get; set; }
        }

        public class Peratio1
        {
        }

        public class Pegratio2
        {
        }

        public class Netsharepurchaseactivity
        {
            public int maxAge { get; set; }
            public string? period { get; set; }
            public Buyinfocount? buyInfoCount { get; set; }
            public Buyinfoshares? buyInfoShares { get; set; }
            public Buypercentinsidershares? buyPercentInsiderShares { get; set; }
            public Sellinfocount? sellInfoCount { get; set; }
            public Sellinfoshares? sellInfoShares { get; set; }
            public Sellpercentinsidershares? sellPercentInsiderShares { get; set; }
            public Netinfocount? netInfoCount { get; set; }
            public Netinfoshares? netInfoShares { get; set; }
            public Netpercentinsidershares? netPercentInsiderShares { get; set; }
            public Totalinsidershares? totalInsiderShares { get; set; }
        }

        public class Buyinfocount
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Buyinfoshares
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Buypercentinsidershares
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Sellinfocount
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Sellinfoshares
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Sellpercentinsidershares
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Netinfocount
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Netinfoshares
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Netpercentinsidershares
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Totalinsidershares
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Sectortrend
        {
            public int maxAge { get; set; }
            public object? symbol { get; set; }
            public Peratio2? peRatio { get; set; }
            public Pegratio3? pegRatio { get; set; }
            public object?[]? estimates { get; set; }
        }

        public class Peratio2
        {
        }

        public class Pegratio3
        {
        }

        public class Insiderholders
        {
            public Holder[]? holders { get; set; }
            public int maxAge { get; set; }
        }

        public class Holder
        {
            public int maxAge { get; set; }
            public string? name { get; set; }
            public string? relation { get; set; }
            public string? url { get; set; }
            public string? transactionDescription { get; set; }
            public Latesttransdate? latestTransDate { get; set; }
            public Positiondirect? positionDirect { get; set; }
            public Positiondirectdate? positionDirectDate { get; set; }
        }

        public class Latesttransdate
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Positiondirect
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Positiondirectdate
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Earnings
        {
            public int maxAge { get; set; }
            public Earningschart? earningsChart { get; set; }
            public Financialschart? financialsChart { get; set; }
            public string? financialCurrency { get; set; }
        }

        public class Earningschart
        {
            public Quarterly[]? quarterly { get; set; }
            public Currentquarterestimate? currentQuarterEstimate { get; set; }
            public string? currentQuarterEstimateDate { get; set; }
            public int currentQuarterEstimateYear { get; set; }
            public Earningsdate[]? earningsDate { get; set; }
        }

        public class Currentquarterestimate
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Quarterly
        {
            public string? date { get; set; }
            public Actual? actual { get; set; }
            public Estimate1? estimate { get; set; }
        }

        public class Actual
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Estimate1
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Earningsdate
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Financialschart
        {
            public Yearly[]? yearly { get; set; }
            public Quarterly1[]? quarterly { get; set; }
        }

        public class Yearly
        {
            public int date { get; set; }
            public Revenue? revenue { get; set; }
            public Earnings1? earnings { get; set; }
        }

        public class Revenue
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Earnings1
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Quarterly1
        {
            public string? date { get; set; }
            public Revenue1? revenue { get; set; }
            public Earnings2? earnings { get; set; }
        }

        public class Revenue1
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Earnings2
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Upgradedowngradehistory
        {
            public History1[]? history { get; set; }
            public int maxAge { get; set; }
        }

        public class History1
        {
            public int epochGradeDate { get; set; }
            public string? firm { get; set; }
            public string? toGrade { get; set; }
            public string? fromGrade { get; set; }
            public string? action { get; set; }
        }

        public class Earningstrend
        {
            public Trend1[]? trend { get; set; }
            public int maxAge { get; set; }
        }

        public class Trend1
        {
            public int maxAge { get; set; }
            public string? period { get; set; }
            public string? endDate { get; set; }
            public Growth1? growth { get; set; }
            public Earningsestimate? earningsEstimate { get; set; }
            public Revenueestimate? revenueEstimate { get; set; }
            public Epstrend? epsTrend { get; set; }
            public Epsrevisions? epsRevisions { get; set; }
        }

        public class Growth1
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Earningsestimate
        {
            public Avg? avg { get; set; }
            public Low? low { get; set; }
            public High? high { get; set; }
            public Yearagoeps? yearAgoEps { get; set; }
            public Numberofanalysts? numberOfAnalysts { get; set; }
            public Growth2? growth { get; set; }
        }

        public class Avg
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Low
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class High
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Yearagoeps
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Numberofanalysts
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Growth2
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Revenueestimate
        {
            public Avg1? avg { get; set; }
            public Low1? low { get; set; }
            public High1? high { get; set; }
            public Numberofanalysts1? numberOfAnalysts { get; set; }
            public Yearagorevenue? yearAgoRevenue { get; set; }
            public Growth3? growth { get; set; }
        }

        public class Avg1
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Low1
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class High1
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Numberofanalysts1
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Yearagorevenue
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Growth3
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Epstrend
        {
            public Current? current { get; set; }
            public _7Daysago? _7daysAgo { get; set; }
            public _30Daysago? _30daysAgo { get; set; }
            public _60Daysago? _60daysAgo { get; set; }
            public _90Daysago? _90daysAgo { get; set; }
        }

        public class Current
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class _7Daysago
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class _30Daysago
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class _60Daysago
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class _90Daysago
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Epsrevisions
        {
            public Uplast7days? upLast7days { get; set; }
            public Uplast30days? upLast30days { get; set; }
            public Downlast30days? downLast30days { get; set; }
            public Downlast90days? downLast90days { get; set; }
        }

        public class Uplast7days
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Uplast30days
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Downlast30days
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Downlast90days
        {
        }

        public class Financialdata
        {
            public int maxAge { get; set; }
            public Currentprice? currentPrice { get; set; }
            public Targethighprice? targetHighPrice { get; set; }
            public Targetlowprice? targetLowPrice { get; set; }
            public Targetmeanprice? targetMeanPrice { get; set; }
            public Targetmedianprice? targetMedianPrice { get; set; }
            public Recommendationmean? recommendationMean { get; set; }
            public string? recommendationKey { get; set; }
            public Numberofanalystopinions? numberOfAnalystOpinions { get; set; }
            public Totalcash? totalCash { get; set; }
            public Totalcashpershare? totalCashPerShare { get; set; }
            public Ebitda? ebitda { get; set; }
            public Totaldebt? totalDebt { get; set; }
            public Quickratio? quickRatio { get; set; }
            public Currentratio? currentRatio { get; set; }
            public Totalrevenue? totalRevenue { get; set; }
            public Debttoequity? debtToEquity { get; set; }
            public Revenuepershare? revenuePerShare { get; set; }
            public Returnonassets? returnOnAssets { get; set; }
            public Returnonequity? returnOnEquity { get; set; }
            public Grossprofits? grossProfits { get; set; }
            public Freecashflow? freeCashflow { get; set; }
            public Operatingcashflow? operatingCashflow { get; set; }
            public Earningsgrowth? earningsGrowth { get; set; }
            public Revenuegrowth? revenueGrowth { get; set; }
            public Grossmargins? grossMargins { get; set; }
            public Ebitdamargins? ebitdaMargins { get; set; }
            public Operatingmargins? operatingMargins { get; set; }
            public Profitmargins1? profitMargins { get; set; }
            public string? financialCurrency { get; set; }
        }

        public class Currentprice
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Targethighprice
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Targetlowprice
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Targetmeanprice
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Targetmedianprice
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Recommendationmean
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Numberofanalystopinions
        {
            public int raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Totalcash
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Totalcashpershare
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Ebitda
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Totaldebt
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Quickratio
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Currentratio
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Totalrevenue
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Debttoequity
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Revenuepershare
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Returnonassets
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Returnonequity
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Grossprofits
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Freecashflow
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Operatingcashflow
        {
            public long raw { get; set; }
            public string? fmt { get; set; }
            public string? longFmt { get; set; }
        }

        public class Earningsgrowth
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Revenuegrowth
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Grossmargins
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Ebitdamargins
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Operatingmargins
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }

        public class Profitmargins1
        {
            public float raw { get; set; }
            public string? fmt { get; set; }
        }
    }
}

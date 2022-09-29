using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;

namespace DxMLEngine.Features.CrunchBase
{
    internal class Webpage
    {
        public string Domain { get { return ""; } }
        public string SearchUrl { get { return ConfigureSearchUrl(); } }
        public string AcquisitionUrl { get { return ConfigureAcquisitionUrl(); } }
        public string CompanyUrl { get { return ConfigureCompanyUrl(); } }
        public string ContactUrl { get { return ConfigureContactUrl(); } }
        public string EventUrl { get { return ConfigureEventUrl(); } }
        public string FundingUrl { get { return ConfigureFundingUrl(); } }
        public string HubUrl { get { return ConfigureHubUrl(); } }
        public string InvestorUrl { get { return ConfigureInvestorUrl(); } }
        public string PeopleUrl { get { return ConfigurePeopleUrl(); } }
        public string SchoolUrl { get { return ConfigureSchoolUrl(); } }

        public string? PageText { set; get; }
        public string? PageSource { set; get; }

        private const string URL_SEARCH_PAGE = "";
        private const string URL_ACQUISITION_PAGE = "";
        private const string URL_COMPANY_PAGE = "";
        private const string URL_CONTACT_PAGE = "";
        private const string URL_EVENT_PAGE = "";
        private const string URL_FUNDING_PAGE = "";
        private const string URL_HUB_PAGE = "";
        private const string URL_INVESTOR_PAGE = "";
        private const string URL_PEOPLE_PAGE = "";
        private const string URL_SCHOOL_PAGE = "";

        internal string? Param1 { set; get; }
        internal string? Param2 { set; get; }
        internal string? Param3 { set; get; }

        public Webpage()
        {
        }

        private string ConfigureSearchUrl()
        {
            throw new NotImplementedException();
        }

        private string ConfigureAcquisitionUrl()
        {
            throw new NotImplementedException();
        }

        private string ConfigureCompanyUrl()
        {
            throw new NotImplementedException();
        }

        private string ConfigureContactUrl()
        {
            throw new NotImplementedException();
        }

        private string ConfigureEventUrl()
        {
            throw new NotImplementedException();
        }

        private string ConfigureFundingUrl()
        {
            throw new NotImplementedException();
        }

        private string ConfigureHubUrl()
        {
            throw new NotImplementedException();
        }

        private string ConfigureInvestorUrl()
        {
            throw new NotImplementedException();
        }

        private string ConfigurePeopleUrl()
        {
            throw new NotImplementedException();
        }

        private string ConfigureSchoolUrl()
        {
            throw new NotImplementedException();
        }
    }
}

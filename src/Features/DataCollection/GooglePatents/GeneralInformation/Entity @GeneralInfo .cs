using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DxMLEngine.Features.GooglePatents
{
    internal class GeneralInfo
    {
        public string PublicationNumber { set; get; }
        public string CountryCode { set; get; }
        public string CountryName { set; get; }

        public string? DownloadHref { set; get; }
        public string[] PriorArtKeywords { set; get; }

        public string[] Inventors { set; get; }
        public string? CurrentAssignee { set; get; }
        public string[] OriginalAssignees { set; get; }

        public string? LegalStatus { set; get; }
        public string ApplicationNumber { set; get; }
        public Dictionary<string, string>[] ApplicationEvents { set; get; }
        public Dictionary<string, string> ExternalLinks { set; get; }

        public GeneralInfo(string publicationNumber, string countryCode, string countryName, 
            string? downloadHref, string[] priorArtKeywords, string[] inventors, string? currentAssignee, 
            string[] originalAssignees, string? legalStatus, string applicationNumber,
            Dictionary<string, string>[] applicationEvents, Dictionary<string, string> externalLinks)
        {
            this.PublicationNumber = publicationNumber;
            this.CountryCode = countryCode;
            this.CountryName = countryName;
            this.DownloadHref = downloadHref;
            this.PriorArtKeywords = priorArtKeywords;
            this.Inventors = inventors;
            this.CurrentAssignee = currentAssignee;
            this.OriginalAssignees = originalAssignees;
            this.LegalStatus = legalStatus;
            this.ApplicationNumber = applicationNumber;
            this.ApplicationEvents = applicationEvents;
            this.ExternalLinks = externalLinks;
        }
    }
}

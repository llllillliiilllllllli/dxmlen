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
    internal class Patent
    {
        public string? Title { set; get; }
        public string? Url { set; get; }

        public Abstract? Abstract { set; get; }
        public Images? Images { set; get; }
        public Classifications? Classifications { set; get; }
        public GeneralInfo? GeneralInfo { set; get; }

        public Description? Description { set; get; }
        public Claims? Claims { set; get; }
        public Concepts? Concepts { set; get; }

        public PatentCitations? PatentCitations { set; get; }
        public NonPatentCitations? NonPatentCitations { set; get; }
        public CitedBy? CitedBy { set; get; }
        public SimilarDocuments? SimilarDocuments { set; get; }

        public PriorityApplications? PriorityApplications { set; get; }
        public LegalEvents? LegalEvents { set; get; }
    }
}           

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
    internal class PriorityApplications
    {
        internal class PriorityApplication
        {
            public string Application { set; get; }
            public DateTime PriorityDate { set; get; }
            public DateTime FilingDate { set; get; }
            public string Title { set; get; }

            public PriorityApplication(string application, DateTime priorityDate, DateTime filingDate, string title)
            {
                this.Application = application;
                this.PriorityDate = priorityDate;
                this.FilingDate = filingDate;
                this.Title = title;
            }
        }

        public PriorityApplication[] Applications;

        public PriorityApplications(PriorityApplication[] applications)
            => this.Applications = applications;
    }
}

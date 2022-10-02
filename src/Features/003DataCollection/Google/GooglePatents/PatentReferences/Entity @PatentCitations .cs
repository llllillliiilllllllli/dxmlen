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
    internal class PatentCitations
    {
        internal class Citation
        {
            public string PublicationNumber { set; get; }
            public string PriorityDate { set; get; }
            public DateTime PublicationDate { set; get; }
            public string Assignee { set; get; }
            public string Title { set; get; }

            public Citation(string publicationNumber, string priorityDate,
                DateTime publicationDate, string assignee, string title)
            {
                PublicationNumber = publicationNumber;
                PriorityDate = priorityDate;
                PublicationDate = publicationDate;
                Assignee = assignee;
                Title = title;
            }
        }

        public Citation[] Citations;

        public PatentCitations(Citation[] citations)
            => this.Citations = citations;
    }
}

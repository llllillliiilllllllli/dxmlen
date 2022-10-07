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
    internal class SimilarDocuments
    {
        internal class SimilarDocument
        {
            public string Publication { set; get; }
            public DateTime PublicationDate { set; get; }
            public string Title { set; get; }

            public SimilarDocument(string publication, DateTime publicationDate, string title)
            {
                this.Publication = publication;
                this.PublicationDate = publicationDate;
                this.Title = title;
            }
        }

        public SimilarDocument[] Documents { set; get; }

        public SimilarDocuments(SimilarDocument[] documents)
            => this.Documents = documents;
    }
}

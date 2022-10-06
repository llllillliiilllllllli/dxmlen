using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.ScienceDirect
{
    public class Article
    {
        public string? Title { set; get; }
        public string? Description { set; get; }
        public string[]? Keywords { set; get; }
        public string[]? Creators { set; get; }
        public string? Publisher { set; get; }
        public string? Volume { set; get; }
        public string? Type { set; get; }
        public DateTime? CoverDate { set; get; }
        public string? PageRange { set; get; }
        public bool? OpenAccess { set; get; }

        public string? Doi { set; get; }
        public string? Eid { set; get; }
        public string? Pii { set; get; }
        public string? PubmedId { set; get; }
        public string? ScopusId { set; get; }

        public string? Url { set; get; }
    }
}

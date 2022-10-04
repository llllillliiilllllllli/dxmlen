using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.ScienceDirect
{
    internal class SearchResult
    {
        public string? Identifier { get; set; }
        public string? Title { get; set; }
        public string? Creator { get; set; }

        public string? Url { get; set; }
        public string? PublicationName { get; set; }
        public string? Volume { get; set; }
        public string? CoverDate { get; set; }
        public string? StartingPage { get; set; }
        public string? EndingPage { get; set; }
        public string? Doi { get; set; }

        public bool OpenAccess { get; set; }
        public string? Pii { get; set; }
        public string[]? Authors { get; set; }
    }
}

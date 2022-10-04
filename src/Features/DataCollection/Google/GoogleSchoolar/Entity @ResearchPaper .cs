using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DxMLEngine.Attributes;

namespace DxMLEngine.Features.GoogleScholar
{
    public class ResearchPaper
    {
        public string? Id { set; get; }
        public string? Title { set; get; }
        public string[]? Authors { set; get; }
        public string? Publisher { set; get; }
        public string? Desription { set; get; }
        public int? CitedBy { set; get; }
        public string? RelatedArticles { set; get; }
        public int? NumberOfVersions { set; get; }
        public string? Url { set; get; }
    }
}

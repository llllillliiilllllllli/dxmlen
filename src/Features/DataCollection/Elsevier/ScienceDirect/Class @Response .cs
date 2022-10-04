using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DxMLEngine.Features.ScienceDirect
{
    public class Response
    {
        public string? Id { set; get; }

        public string? Content { set; get; }

        public ScienceDirectSearch? ScienceDirectSearch { get { return Deserialize<ScienceDirectSearch>(); } }
        public ArticleRetrieval? ArticleRetrieval { get { return Deserialize<ArticleRetrieval>(); } }

        private T? Deserialize<T>() where T : class
        {
            if (Content == null) return null;
            var options = new JsonSerializerOptions() { WriteIndented = true };
            return JsonSerializer.Deserialize<T>(Content, options);
        }
    }
}

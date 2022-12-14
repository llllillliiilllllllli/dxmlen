using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.Amazon
{
    internal class Product
    {
        internal string? Asin { set; get; }
        internal string? Name { set; get; }
        internal string? Seller { set; get; }
        internal string? Price { set; get; }
        internal string[]? Categories { set; get; }
        internal string? Description { set; get; }
        internal string? Url { set; get; }
    }
}

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DxMLEngine.Objects
{
    internal class Dataset
    {
        internal class Meta
        {
            // ...
            // ...
            // ...
            // ...
            // ...
        }

        public object[] Rows { set; get; } = new object[0];
        public object[] Columns { set; get; } = new object[0];

        public Dataset() { }
    }
}

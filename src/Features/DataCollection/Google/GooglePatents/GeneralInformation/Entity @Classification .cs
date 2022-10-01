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
    internal class Classifications
    {
        internal class Class
        {
            public string ClassCode { set; get; }
            public string Description { set; get; }

            public Class(string classCode, string description)
            {
                this.ClassCode = classCode;
                this.Description = description;
            }
        }

        public Class[] Classes { set; get; }

        public Classifications(Class[] classes)
            => this.Classes = classes;
    }
}

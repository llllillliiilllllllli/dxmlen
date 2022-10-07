using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Attributes
{
    internal class Feature : Attribute
    {
        internal string? Instruction { set; get; }
        public Feature() { }
        public Feature(string? instruction)
        {
            this.Instruction = instruction;
        }
    }
}

/// ====================================================================================
/// -    
/// 1  Something: Lorem ipsum dolor sit amet, consectetur adipiscing elit.
/// 2  Anything: Nulla sem nisi, suscipit sed est nec, posuere aliquam tellus.
/// 3  Everything: Ut molestie ex sed felis pulvinar vestibulum. 
/// 4  
/// 5  Something: Lorem ipsum dolor sit amet, consectetur adipiscing elit.
/// 6  Anything: Nulla sem nisi, suscipit sed est nec, posuere aliquam tellus.
/// 7  Everything: Ut molestie ex sed felis pulvinar vestibulum. 
/// 8  
/// 9  Something: Lorem ipsum dolor sit amet, consectetur adipiscing elit.
/// 10 Anything: Nulla sem nisi, suscipit sed est nec, posuere aliquam tellus.
/// 11 Everything: Ut molestie ex sed felis pulvinar vestibulum.  
/// 12
/// ====================================================================================

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DxMLEngine.Objects
{
    internal class Datadoc
    {
        internal class Meta
        {
            // ...
            // ...
            // ...
            // ...
            // ...
        }

        public string?[] Lines { set; get; } = new string?[0];

        public Datadoc() { }
    }
}

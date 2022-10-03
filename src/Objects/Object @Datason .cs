/// ====================================================================================
/// {
///     "Something" : 
///         { 
///             "Something Else" : { ... } 
///             "Something Else" : { ... } 
///             "Something Else" : { ... } 
///         }
///     "Anything" : 
///         { 
///             "Anything Else" : { ... } 
///             "Anything Else" : { ... } 
///             "Anything Else" : { ... } 
///         }
///     "Everything" : 
///         { 
///             "Anything Else" : { ... } 
///             "Anything Else" : { ... } 
///             "Anything Else" : { ... } 
///         }              
/// }
/// ====================================================================================

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DxMLEngine.Objects
{
    internal class Datason
    {
        internal class Meta
        {
            // ...
            // ...
            // ...
            // ...
            // ...
        }

        public Dictionary<object, object?> Dictionary { set; get; } = new Dictionary<object, object?>();

        public Datason() { }
    }
}

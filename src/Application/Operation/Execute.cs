using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MLEngine.Operation
{
    internal class Execute
    {
        public static void ExecuteCommand(MethodInfo method)
        {
            try { method.Invoke(null, null); }
            catch { throw; }    
        }
    }
}

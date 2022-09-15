using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using DxMLEngine.Utilities;

namespace DxMLEngine.Operation
{
    internal class Execute
    {
        public static void ExecuteCommand(MethodInfo method)
        {
            var command = $"{method.DeclaringType?.Name}.{method.Name}";

            Log.Info($"Execute command: {command}");
            var beg = DateTime.Now;

            try { method.Invoke(null, null); }
            catch { throw; }

            var end = DateTime.Now;
            var duration = end - beg;
            Log.Info($"{command} finished in {duration.TotalSeconds:F2} sec");
        }
    }
}

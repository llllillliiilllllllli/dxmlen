using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using DxMLEngine.Utilities;
using DxMLEngine.Attributes;

namespace DxMLEngine.Operation
{
    internal class Execute
    {
        public static void ExecuteCommand(MethodInfo method)
        {
            var command = $"{method.DeclaringType?.Name}.{method.Name}";

            Log.Info($"Execute command: {command}");
            var beg = DateTime.Now;

            var feature = method.GetCustomAttribute<Feature>();
            if (feature != null) Console.WriteLine(feature.Instruction);

            var parameters = new List<object?>();
            var paramterInfos = method.GetParameters();
            foreach (var paramterInfo in paramterInfos) 
            {
                Console.Write($"\n{paramterInfo.ParameterType} {paramterInfo.Name}: ");
                var parameter = Convert.ChangeType(Console.ReadLine(), paramterInfo.ParameterType);
                parameter = HandlePathParameter(parameter);
                parameters.Add(parameter);
            }

            try { method.Invoke(null, parameters.ToArray()); }
            catch { throw; }

            var end = DateTime.Now;
            var duration = end - beg;
            Log.Info($"{command} finished in {duration.TotalSeconds:F2} sec");
        }

        private static object? HandlePathParameter(object? obj)
        {            
            if (obj == null) return null;
            if (obj.GetType() == typeof(string))
            {
                var path = Convert.ToString(obj)?.Replace("\"", "");
                if (path == null) return null;
                if (Path.IsPathFullyQualified(path))
                {
                    return path;
                }
            }
            return obj;
        }
    }
}

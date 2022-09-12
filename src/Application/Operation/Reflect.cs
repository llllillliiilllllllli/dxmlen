using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MLEngine.Operation
{
    internal class Reflect
    {
        public static MethodInfo[] CollectFeatures()
        {          
            var assembly = Assembly.GetExecutingAssembly();
            var features = 
                from type in assembly.GetTypes()
                where type.Namespace == "MLEngine.Features"
                from method in type.GetMethods()
                where method.Name != "GetType"
                where method.Name != "Equals"
                where method.Name != "ToString"
                where method.Name != "GetHashCode"
                select method;

            return features.ToArray();
        }
    }
}

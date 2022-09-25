using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using DxMLEngine.Attributes;

namespace DxMLEngine.Operation
{
    internal class Reflect
    {
        public static MethodInfo[] CollectFeatures()
        {          
            var assembly = Assembly.GetExecutingAssembly();
            var features =
                from type in assembly.GetTypes()
                where type.GetCustomAttribute(typeof(Feature)) != null
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

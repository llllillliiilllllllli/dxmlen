using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MLEngine.Styles;

namespace MLEngine.Operation
{
    internal class Select
    {
        public static MethodInfo? InquireSelection(MethodInfo[] features)
        {
            Console.WriteLine($"\n{Color.Green}\u276f{Color.Reset} Select from options:");
            for (int i = 0; i < features.Length; i++)
            {
                var feature = features[i];
                Console.WriteLine($"{i+1, 2} {feature.DeclaringType?.Name}.{feature.Name}");
            }

            Console.Write($"\n{Color.Green}\u276f{Color.Reset} Select: ");          
            var input = Console.ReadLine(); 
            if (input != null || input != "\n" || input != string.Empty)
            {
                try { return features[int.Parse(input!) - 1]; }                        
                catch { throw new Exception($"ERROR: Incorrect selection {input}"); }
            }

            return null;
        }
    }
}

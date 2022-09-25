using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using DxMLEngine.Styles;
using Color = DxMLEngine.Styles.Color;
namespace DxMLEngine.Operation
{
    internal class Select
    {
        public static MethodInfo? InquireSelection(MethodInfo[] features)
        {
            Console.WriteLine($"\n{Color.Green}\u276f{Color.Reset} Select from options:");
            for (int i = 0; i < features.Length; i++)
            {
                var feature = features[i];
                Console.WriteLine($"{i+1, 3} {feature.DeclaringType?.Name}.{feature.Name}");
            }

            Console.Write($"\n{Color.Green}\u276f{Color.Reset} Select: ");          
            var input = Console.ReadLine();
            
            if (!string.IsNullOrEmpty(input))
            {
                try { return features[int.Parse(input!) - 1]; }                        
                catch { throw new Exception($"Incorrect selection {input}"); }
            }

            return null;
        }
    }
}

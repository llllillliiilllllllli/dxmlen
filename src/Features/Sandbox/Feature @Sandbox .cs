using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

using DxMLEngine.Functions;
using DxMLEngine.Attributes;
using DxMLEngine.Features.GooglePatents;

namespace DxMLEngine.Features
{
    [Feature]
    internal class Sandbox
    {
        public static void FindRegex()
        {
            var text = "G06N3/04 Architectures, e.g. interconnection topology";

            var foundedClassCodes =
                from field in typeof(ClassCodePatterns).GetFields()
                let classCodePatern = (string?)field.GetValue(null)
                let codeRegex = new Regex(classCodePatern)
                let codeMatch = codeRegex.Match(text)
                where codeMatch.Success
                select codeMatch.Value;

            foreach (var foundedClassCode in foundedClassCodes.ToArray())
            {
                Console.WriteLine(foundedClassCode);
            }

            Console.WriteLine("==");

            foreach (var field in typeof(ClassCodePatterns).GetFields())
            {
                var classCodePatern = (string?)field.GetValue(null);
                Console.WriteLine(classCodePatern);
                var codeRegex = new Regex(classCodePatern!);
                var codeMatch = codeRegex.Match(text);
                if (codeMatch.Success) Console.WriteLine(codeMatch.Value);
            }

        }
    }
}

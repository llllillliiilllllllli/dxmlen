using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DxMLEngine.Objects;

namespace DxMLEngine.Functions
{
    internal class DxData
    {
        public static Datadoc ReadDatadoc(string path)
        {
            var lines = new List<string?>();
            using (var sr = new StreamReader(path))
            {
                while (sr.EndOfStream == false) 
                {
                    lines.Add(sr.ReadLine());
                }
            }

            return new Datadoc() { Lines = lines.ToArray() };          
        }        
        
        public static void WriteDatadoc(string path, Datadoc datadoc)
        {

        }

        public static void ReadDataset()
        {

        }

        public static void WriteDataset()
        {

        }        
        
        public static void ReadDatason()
        {

        }

        public static void WriteDatason()
        {

        }
    }
}

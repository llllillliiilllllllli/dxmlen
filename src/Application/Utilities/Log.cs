using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxMLEngine.Styles;
using Color = DxMLEngine.Styles.Color;
namespace DxMLEngine.Utilities
{
    internal class Log
    {
        public static void Debug(string msg)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:MM:SS");
            Console.WriteLine($"\n{timestamp} | {Color.Yellow}DEBUG{Color.Reset} | {msg}");
        }

        public static void Info(string msg)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
            Console.WriteLine($"\n{timestamp} | {Color.Blue}INFO{Color.Reset} | {msg}");
        }

        public static void Warning(string msg)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:MM:SS");
            Console.WriteLine($"\n{timestamp} | {Color.Magenta}WARNING{Color.Reset} | {msg}");
        }

        public static void Error(string msg)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:MM:SS");
            Console.WriteLine($"\n{timestamp} | {Color.Red}ERROR{Color.Reset} | {msg}");
        }

        public static void Crit(string msg)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:MM:SS");
            Console.WriteLine($"\n{timestamp} | {Color.Black}CRITICAL{Color.Reset} | {msg}");
        }
    }
}

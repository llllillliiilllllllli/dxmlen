using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Styles
{
    internal class UI
    {
        public static void PrintConsole()
        {
            Console.Clear();

            var windownWidth = Console.WindowWidth;
            var userName = Environment.UserName;
            var currentPath = Environment.CurrentDirectory;            
            currentPath = "~\\" + string.Join("\\", currentPath.Split("\\")[^3..]);
            var currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var branchName = ThisAssembly.Git.Branch;

            userName = $"{Color.Yellow}{userName}{Color.Reset}";
            currentPath = $"{Color.Cyan}{currentPath}{Color.Reset}";
            currentDate = $"{Color.White}{currentDate}{Color.Reset}";
            branchName = $"{Color.Red}\ue0a0 {branchName}{Color.Reset}";

            for (int i = 0; i < windownWidth; i++) Console.Write("#");
            Console.WriteLine($"\n{userName} in {currentPath} | {currentDate} on {branchName}");
        }
    }
}

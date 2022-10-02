using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DxMLEngine.Functions;

namespace DxMLEngine.Functions
{
    internal class Browser
    {
        public static Process? LaunghEdge(string initialUrl = "about:blank",
            ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal,
            bool createNoWindow = false, int wait = 100)
        {
            var startInfo = new ProcessStartInfo("MicrosoftEdge.exe");
            startInfo.Arguments = initialUrl;
            startInfo.WindowStyle = windowStyle;
            startInfo.CreateNoWindow = createNoWindow;

            var browser = Process.Start(startInfo);
            Thread.Sleep(wait);

            return browser;
        }

        public static void CloseBrowser(Process browser)
        {
            DxKeyboard.SendKeys(browser, "ALT+F4", 100);
        }

        public static Process OpenNewTab(Process browser, string url, int wait = 5000)
        {
            var fileName = browser.StartInfo.FileName;
            var process = Process.Start(fileName, url);
            Thread.Sleep(wait);

            return process;
        }

        public static void CloseCurrentTab(Process process)
        {
            DxKeyboard.SendKeys(process, "CTRL+W", 100);
        }

        public static string CopyPageText(Process process, int wait = 0)
        {
            Thread.Sleep(wait);

            DxClipboard.SetText("");
            DxClipboard.GetText();
            while (DxClipboard.GetText() == "")
            { 
                DxKeyboard.SendKeys(process, "CTRL+A", 100);
                DxKeyboard.SendKeys(process, "CTRL+C", 100);
                Thread.Sleep(1000);
            }

            return DxClipboard.GetText();
        }

        public static string CopyPageSource(Process process, int wait = 5000)
        {
            DxKeyboard.SendKeys(process, "CTRL+U", 100);
            Thread.Sleep(wait);

            var newProcess = Process.GetCurrentProcess();

            DxClipboard.SetText("");
            DxClipboard.GetText();
            while (DxClipboard.GetText() == "")
            {
                DxKeyboard.SendKeys(newProcess, "CTRL+A", 100);
                DxKeyboard.SendKeys(newProcess, "CTRL+C", 100);                
                Thread.Sleep(1000);
            }

            DxKeyboard.SendKeys(newProcess, "CTRL+W", 100);

            return DxClipboard.GetText();
        }

        public static void DownloadWebpage(Process process, int wait = 5000)
        {
            DxKeyboard.SendKeys(process, "CTRL+S", 100);
            Thread.Sleep(wait);
        }
    }
}

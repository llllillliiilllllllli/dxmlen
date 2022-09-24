/// Keyboard and Mouse Input for Windows User (Win32)
/// https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Diagnostics; 
using System.Runtime.InteropServices;

namespace DxMLEngine.Functions
{
    internal class Keyboard
    {
        public const byte VK_A = 0x41;
        public const byte VK_Q = 0x51;
        public const byte VK_R = 0x52;
        public const byte VK_S = 0x53;

        public const byte VK_F1 = 0x70;
        public const byte VK_F2 = 0x71;
        public const byte VK_F3 = 0x72;
        public const byte VK_F4 = 0x73;
        public const byte VK_F5 = 0x74;
        public const byte VK_F6 = 0x75;
        public const byte VK_F7 = 0x76;
        public const byte VK_F8 = 0x77;
        public const byte VK_F9 = 0x78;
        public const byte VK_F10 = 0x79;
        public const byte VK_F11 = 0x7A;
        public const byte VK_F12 = 0x7B;

        public const byte VK_SPACE = 0x20;
        public const byte VK_RETURN = 0x0D;

        public const byte VK_SHIFT = 0x10;
        public const byte VK_CONTROL = 0x11;
        public const byte VK_MENU = 0x12;
        public const byte VK_PAUSE = 0x13;
        public const byte VK_CAPITAL = 0x14;

        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;
        public const uint KEYEVENTF_KEYUP = 0x0002;
        public const uint KEYEVENTF_KEYDOWN = 0x0000;

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        [DllImport("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr point);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static void SendKeys(Process process, string pattern, int delay)
        {
            var intPtr = process.MainWindowHandle;
            SetForegroundWindow(intPtr);

            string temp = "";
            if (pattern.StartsWith("SHIFT+") == true) 
            {
                keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                pattern = pattern.Replace("SHIFT+", "");
                temp = "SHIFT+";
            }

            if (pattern.StartsWith("CTRL+") == true) 
            {
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                pattern = pattern.Replace("CTRL+", "");
                temp = "CTRL+";
            }

            if (pattern.StartsWith("ALT+") == true)
            {
                keybd_event(VK_MENU, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                pattern = pattern.Replace("ALT+", "");
                temp = "ALT+";
            }            
            
            if (pattern.StartsWith("F1") == true)
            {
                keybd_event(VK_F1, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                pattern = pattern.Replace("F1", "");
                keybd_event(VK_F1, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            }

            if (pattern.StartsWith("F2") == true)
            {
                keybd_event(VK_F2, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                pattern = pattern.Replace("F2", "");
                keybd_event(VK_F2, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            }            
            
            if (pattern.StartsWith("F3") == true)
            {
                keybd_event(VK_F3, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                pattern = pattern.Replace("F3", "");
                keybd_event(VK_F3, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            }

            if (pattern.StartsWith("F4") == true)
            {
                keybd_event(VK_F4, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                pattern = pattern.Replace("F4", "");
                keybd_event(VK_F4, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            }

            if (pattern.StartsWith("F5") == true)
            {
                keybd_event(VK_F5, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                pattern = pattern.Replace("F5", "");
                keybd_event(VK_F5, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            }

            if (pattern.StartsWith("F6") == true)
            {
                keybd_event(VK_F6, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                pattern = pattern.Replace("F6", "");
                keybd_event(VK_F6, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            }

            if (pattern.StartsWith("F7") == true)
            {
                keybd_event(VK_F7, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                pattern = pattern.Replace("F7", "");
                keybd_event(VK_F7, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            }

            if (pattern.StartsWith("F8") == true)
            {
                keybd_event(VK_F8, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                pattern = pattern.Replace("F8", "");
                keybd_event(VK_F8, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            }

            if (pattern.StartsWith("F9") == true)
            {
                keybd_event(VK_F9, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                pattern = pattern.Replace("F9", "");
                keybd_event(VK_F9, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            }

            Thread.Sleep(delay);

            foreach (var character in pattern)
            {
                var key = Encoding.ASCII.GetBytes(character.ToString())[0];

                keybd_event(key, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                Thread.Sleep(delay);
                keybd_event(key, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            }

            if (temp.StartsWith("SHIFT+")) 
                keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, IntPtr.Zero);

            if (temp.StartsWith("CTRL+")) 
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, IntPtr.Zero);

            if (temp.StartsWith("ALT+")) 
                keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
        }
    }
}

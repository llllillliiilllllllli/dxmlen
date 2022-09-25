using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DxMLEngine.Functions
{
    internal class Mouse
    {
        [DllImport("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr point);

        static int SM_CXSCREEN = 0;
        static int SM_CYSCREEN = 1;

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(int nIndex);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const uint MOUSEEVENTF_MOVE = 0x0001;

        public static void LeftClick(int xpos, int ypos)
        {
            int sx = GetSystemMetrics(SM_CXSCREEN);
            int sy = GetSystemMetrics(SM_CYSCREEN);

            int x = xpos * 65536 / sx;
            int y = ypos * 65536 / sy;

            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, x, y, 0, 0);
            Thread.Sleep(100);
            mouse_event(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, x, y, 0, 0);
        }
    }
}

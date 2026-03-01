using System.Runtime.InteropServices;

namespace ReFMGame.GameHelper
{
    public static class WindowSubclass
    {
        private static IntPtr oldWndProc;
        private static WndProc newWndProc;

        public static void Subclass(IntPtr hwnd)
        {
            newWndProc = WndProcImpl;
            oldWndProc = SetWindowLongPtr(hwnd, GWL_WNDPROC, newWndProc);
        }

        private static IntPtr WndProcImpl(
            IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_ENTERSIZEMOVE:
                case WM_SYSCOMMAND:
                    // DO NOT block update loop
                    break;
            }

            return CallWindowProc(oldWndProc, hWnd, msg, wParam, lParam);
        }

        private const int GWL_WNDPROC = -4;
        private const int WM_ENTERSIZEMOVE = 0x0231;
        private const int WM_SYSCOMMAND = 0x0112;

        private delegate IntPtr WndProc(
            IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLongPtr(
            IntPtr hWnd, int nIndex, WndProc newProc);

        [DllImport("user32.dll")]
        private static extern IntPtr CallWindowProc(
            IntPtr oldProc, IntPtr hWnd, int msg,
            IntPtr wParam, IntPtr lParam);
    }
}

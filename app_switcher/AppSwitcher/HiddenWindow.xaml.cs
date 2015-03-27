using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace AppSwitcher {

    public partial class HiddenWindow : Window {

        public HiddenWindow() {
            InitializeComponent();
        }

        private void OnExitClicked(object sender, RoutedEventArgs args) {
            Close();
        }

        private void OnTestClicked(object sender, RoutedEventArgs e) {
            ApplicationRunningHelper.BringAppToFront("HydroThunder");
        }

        private void OnTestClicked2(object sender, RoutedEventArgs e) {
            ApplicationRunningHelper.BringAppToFront("iexplore");
        }
    }

    public static class ApplicationRunningHelper {

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(HandleRef hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        /// The GetForegroundWindow function returns a handle to the 
        /// foreground window.
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);



        //one source
        private static int SW_HIDE = 0;
        private static int SW_SHOWNORMAL = 1;
        private static int SW_SHOWMINIMIZED = 2;
        private static int SW_SHOWMAXIMIZED = 3;
        private static int SW_SHOWNOACTIVATE = 4;
        private static int SW_RESTORE = 9;
        private static int SW_SHOWDEFAULT = 10;

        //other source
        private static int SW_SHOW = 5;


        public static void BringAppToFront(string processName) {
            var processes = Process.GetProcessesByName(processName);
            var appProcessId = processes[0].Id;

            bool foundWindow = false;
            var previousWindow = IntPtr.Zero;

            while (!foundWindow) {
                var desktopWindow = GetDesktopWindow();
                var nextWindow = FindWindowEx(desktopWindow, previousWindow, null, null);
                uint processId = 0;
                GetWindowThreadProcessId(nextWindow, out processId);
                if (processId == appProcessId)
                {
                    ForceForegroundWindow(nextWindow);
                    break;
                }

                previousWindow = nextWindow;
            }
        }

        public static void ForceForegroundWindow(IntPtr hWnd)
        {
            uint threadId = 0;
            GetWindowThreadProcessId(GetForegroundWindow(), out threadId);
            uint appThread = GetCurrentThreadId();
            const uint SW_SHOW = 5;

            if (threadId != appThread) {
                AttachThreadInput(threadId, appThread, true);
                BringWindowToTop(hWnd);
                ShowWindow(hWnd, SW_SHOW);
                AttachThreadInput(threadId, appThread, false);
            } else {
                BringWindowToTop(hWnd);
                ShowWindow(hWnd, SW_SHOW);
            }
        }
    }
}

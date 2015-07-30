using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Radiator {
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
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

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
            if (processes.Length == 0)
                return;

            bool didIt = false;
            var appProcessId = processes[0].Id;
            var mainWindow = processes[0].MainWindowHandle;
            if (mainWindow != IntPtr.Zero) {
                if (BringWindowToTop(mainWindow)) {
                    if (ShowWindowAsync(mainWindow, SW_SHOW)) {
                        if (ShowWindowAsync(mainWindow, SW_SHOWMAXIMIZED)) {
                        }
                        if (SetForegroundWindow(mainWindow)) {
                            didIt = true;
                        }
                    }
                }

//                if (!didIt)
//                    Debugger.Break();
            }

            bool foundWindow = false;
            var previousWindow = IntPtr.Zero;

            while (!foundWindow) {
                var desktopWindow = GetDesktopWindow();
                var nextWindow = FindWindowEx(desktopWindow, previousWindow, null, null);
                uint processId = 0;
                GetWindowThreadProcessId(nextWindow, out processId);
                if (processId == appProcessId) {
                    ForceForegroundWindow(nextWindow);
                    break;
                }

                previousWindow = nextWindow;
            }
        }

        public static void ForceForegroundWindow(IntPtr hWnd) {
            uint threadId = 0;
            GetWindowThreadProcessId(GetForegroundWindow(), out threadId);
            uint appThread = GetCurrentThreadId();

            bool didIt = false;

            if (threadId != appThread) {
                    try {
                        AttachThreadInput(threadId, appThread, true);
                        if (BringWindowToTop(hWnd)) {
                            if (ShowWindowAsync(hWnd, SW_SHOWNORMAL)) {
                            }
                            if (ShowWindow(hWnd, SW_SHOW)) {
                                didIt = true;
                            }
                        }
                    } finally {
                        AttachThreadInput(threadId, appThread, false);
                    }
            } else {
                if (BringWindowToTop(hWnd)) {
                    if (ShowWindowAsync(hWnd, SW_SHOWNORMAL)) {
                    }
                    if (ShowWindow(hWnd, SW_SHOW)) {
                        didIt = true;
                    }
                }
            }

//            if (!didIt)
//                Debugger.Break();
        }
    }
}

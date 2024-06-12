using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Seec.Marketing
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process instance = RunningInstance();
            if (instance == null)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
            else
            {
                HandleRunningInstance(instance);
            }
        }

        public static Process RunningInstance()
        {
            //取得目前的程序
            Process current = Process.GetCurrentProcess();
            //取得其他同名稱的程序
            Process[] processes = Process.GetProcessesByName(current.ProcessName);

            foreach (Process process in processes)
            {
                //判斷是不是目前的執行緒
                if (process.Id != current.Id)
                {
                    //確定一下是不是從同一個執行
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        //找到~ 回傳 Process
                        return process;
                    }
                }
            }

            //如果都沒有，則回傳 null
            return null;
        }

        public static void HandleRunningInstance(Process instance)
        {
            //Make sure the window is not minimized or maximized 
            ShowWindowAsync(instance.MainWindowHandle, WS_SHOWNORMAL);
            //Set the real intance to foreground window
            SetForegroundWindow(instance.MainWindowHandle);
        }

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        //1:normal
        //2:minimized
        //3:maximized
        private const int WS_SHOWNORMAL = 1;
    }
}

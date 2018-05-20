using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ComputerUsage
{
    class ComputerDatas
    {
        #region 进程
        public static Process[] GetProcessList()
        {
            return Process.GetProcesses();
        }
        #endregion

        #region 窗口
        private delegate bool WindowEnumDelegate(IntPtr hWnd, int lParam);

        //用来遍历所有窗口 
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WindowEnumDelegate lpEnumFunc, int lParam);

        //获取窗口Text 
        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        //获取窗口类名 
        [DllImport("user32.dll")]
        private static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();


        public static WindowInfo[] GetAllWindowsInfo()
        {
            //用来保存窗口对象 列表
            List<WindowInfo> windowList = new List<WindowInfo>();

            //enum all desktop windows 
            EnumWindows((handle, p2) =>
           {
               

               windowList.Add(GetWindowInfo(handle));
               return true;
           }, 0);

            return windowList.ToArray();
        }

        public static WindowInfo GetForegroundWindowInfo()
        {
            return GetWindowInfo(GetForegroundWindow());
        }

        public static WindowInfo GetWindowInfo(IntPtr handle)
        {
            WindowInfo win = new WindowInfo();
            StringBuilder str = new StringBuilder(256);

            win.handle = handle;

            GetWindowTextW(handle, str, str.Capacity);
            win.name = str.ToString();

            GetClassNameW(handle, str, str.Capacity);
            win.className = str.ToString();
            return win;
        }


        #endregion


        public static PowerStatus GetBatteryStatus()
        {
            return SystemInformation.PowerStatus;
        }
    }


}

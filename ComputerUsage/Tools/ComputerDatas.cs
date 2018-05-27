using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ComputerUsage
{
    public static class ComputerDatas
    {
        public static void RegistSystemEvents()
        {
            SystemEvents.SessionEnding += SystemEventsSessionEndingEventHandler;
            SystemEvents.PowerModeChanged += SystemEventsPowerModeChangedEventHandler;
            SystemEvents.SessionSwitch += SystemEventsSessionSwitchEventHandler;
        }


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



        private static void SystemEventsPowerModeChangedEventHandler(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    GlobalDatas.xml.WriteEvent("系统恢复");
                    break;

                case PowerModes.StatusChange:

                    var status = GetBatteryStatus();
                    if (status.PowerLineStatus == PowerLineStatus.Online)
                    {
                        GlobalDatas.xml.WriteEvent("接上电源");
                    }
                    else if (status.PowerLineStatus == PowerLineStatus.Offline)
                    {
                        GlobalDatas.xml.WriteEvent("拔下电源");
                    }
                    break;
                case PowerModes.Suspend:
                    GlobalDatas.xml.WriteEvent("系统休眠");
                    break;
            }
        }

        private static void SystemEventsSessionEndingEventHandler(object sender, SessionEndingEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionEndReasons.Logoff:
                    GlobalDatas.xml.WriteEvent("用户注销");
                    break;

                case SessionEndReasons.SystemShutdown:
                    GlobalDatas.xml.WriteEvent("系统关闭");
                    break;
            }
        }

        private static void SystemEventsSessionSwitchEventHandler(object sender, SessionSwitchEventArgs e)
        {
            string reason = "";
            switch (e.Reason)
            {
                case SessionSwitchReason.ConsoleConnect:
                    reason = "开始控制台连接";
                    break;

                case SessionSwitchReason.ConsoleDisconnect:
                    reason = "结束控制台连接";
                    break;
                case SessionSwitchReason.RemoteConnect:
                    reason = "开始远程连接";
                    break;
                case SessionSwitchReason.RemoteDisconnect:
                    reason = "结束远程连接";
                    break;

                case SessionSwitchReason.SessionLock:
                    reason = "锁定";
                    break;
                case SessionSwitchReason.SessionLogoff:
                    reason = "注销";
                    break;
                case SessionSwitchReason.SessionLogon:
                    reason = "登录";
                    break;
                case SessionSwitchReason.SessionUnlock:
                    reason = "解锁";
                    break;
                case SessionSwitchReason.SessionRemoteControl:
                    reason = "远程控制";
                    break;
            }
            GlobalDatas.xml.WriteEvent("用户：" + reason + "（当前用户为：" + Environment.UserName + "）");
        }
    }


}

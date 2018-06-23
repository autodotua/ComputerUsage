using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using static ComputerUsage.GlobalDatas;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Collections.Concurrent;
using System.Threading;

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


        #region ����
        public static Process[] GetProcessList()
        {
            return Process.GetProcesses();
        }
        #endregion

        #region ����
        private delegate bool WindowEnumDelegate(IntPtr hWnd, int lParam);

        //�����������д��� 
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WindowEnumDelegate lpEnumFunc, int lParam);

        //��ȡ����Text 
        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        //��ȡ�������� 
        [DllImport("user32.dll")]
        private static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();


        public static WindowInfo[] GetAllWindowsInfo()
        {
            //�������洰�ڶ��� �б�
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
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);   //��ȡ�߳�ID  
        public static WindowInfo GetWindowInfo(IntPtr handle)
        {

            StringBuilder winHeader = new StringBuilder(256);
            StringBuilder winClassName = new StringBuilder(256);

            GetWindowTextW(handle, winHeader, winHeader.Capacity);
            GetClassNameW(handle, winClassName, winClassName.Capacity);

            //����ID  
            int calcTD = 0;    //�߳�ID  
            calcTD = GetWindowThreadProcessId(handle, out int calcID);
            return new WindowInfo(handle, winHeader.ToString(), winClassName.ToString(), new ProcessInfo(Process.GetProcessById(calcID)));
        }


        #endregion


        public static PowerStatus GetBatteryStatus()
        {
            return SystemInformation.PowerStatus;
        }

        public static List<PingInfo> GetNetworkStatus()
        {
            // var s = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            ConcurrentBag<PingInfo> pings = new ConcurrentBag<PingInfo>();
            string[] addresses = Set.PingAddress.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries);
            //  int successfulCount = 0;
            Parallel.ForEach(addresses, p =>
            {
                PingNetAddress(p, out int time, out IPStatus result);
                pings.Add(new PingInfo(p, time, result));
            });
            return pings.ToList();
            //if(successfulCount==addresses.Length)
            //{
            //    return NetworkStatus.All;
            //}
            //if(successfulCount==0)
            //{
            //    return NetworkStatus.None;
            //}
            //return NetworkStatus.Some;
        }

        //public enum NetworkStatus
        //{
        //    All,
        //    Some,
        //    None,
        //    Unknow
        //}

        private static void PingNetAddress(string strNetAdd, out int time, out IPStatus result)
        {
            Ping ping = new Ping();
            result = IPStatus.Unknown;
            try
            {
                PingReply pr = ping.Send(strNetAdd, Set.PingTimeOut);
                //if (pr.Status == IPStatus.TimedOut)
                //{
                //    Flage = false;
                //}
                //if (pr.Status == IPStatus.Success)
                //{
                //    Flage = true;
                //}
                //else
                //{
                //    Flage = false;
                //}
                result = pr.Status;
                if (result != IPStatus.Success)
                {
                    time = -1;
                }
                else
                {
                    time = (int)pr.RoundtripTime;
                }
            }
            catch
            {
                time = -1;
            }
            //return Flage;
        }


        private static void SystemEventsPowerModeChangedEventHandler(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    xml.Write("ϵͳ�ָ�");
                    break;

                case PowerModes.StatusChange:

                    var status = GetBatteryStatus();
                    if (status.PowerLineStatus == PowerLineStatus.Online)
                    {
                        if (xml.LastEvent != "���ϵ�Դ")
                        {
                            xml.Write("���ϵ�Դ");
                        }
                    }
                    else if (status.PowerLineStatus == PowerLineStatus.Offline)
                    {
                        xml.Write("���µ�Դ");
                    }
                    break;
                case PowerModes.Suspend:
                    xml.Write("ϵͳ����");
                    break;
            }
        }

        private static void SystemEventsSessionEndingEventHandler(object sender, SessionEndingEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionEndReasons.Logoff:
                    xml.Write("�û�ע��");
                    break;

                case SessionEndReasons.SystemShutdown:
                    xml.Write("ϵͳ�ر�");
                    break;
            }
        }

        private static void SystemEventsSessionSwitchEventHandler(object sender, SessionSwitchEventArgs e)
        {
            string reason = "";
            switch (e.Reason)
            {
                case SessionSwitchReason.ConsoleConnect:
                    reason = "��ʼ����̨����";
                    break;

                case SessionSwitchReason.ConsoleDisconnect:
                    reason = "��������̨����";
                    break;
                case SessionSwitchReason.RemoteConnect:
                    reason = "��ʼԶ������";
                    break;
                case SessionSwitchReason.RemoteDisconnect:
                    reason = "����Զ������";
                    break;

                case SessionSwitchReason.SessionLock:
                    reason = "����";
                    break;
                case SessionSwitchReason.SessionLogoff:
                    reason = "ע��";
                    break;
                case SessionSwitchReason.SessionLogon:
                    reason = "��¼";
                    break;
                case SessionSwitchReason.SessionUnlock:
                    reason = "����";
                    break;
                case SessionSwitchReason.SessionRemoteControl:
                    reason = "Զ�̿���";
                    break;
            }
            xml.Write("�û���" + reason + "����ǰ�û�Ϊ��" + Environment.UserName + "��");
        }



        static Point lastPosition = new Point(0, 0);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out Point pt);

        public static Point GetCursorPosition()
        {
            GetCursorPos(out Point point);
            return point;
        }

        public static bool MouseMoved()
        {
            bool flag = false;
            Point currentPosition = GetCursorPosition();
            if (currentPosition != lastPosition)
            {
                flag = true;
            }
            lastPosition = currentPosition;
            return flag;
        }
    }


}

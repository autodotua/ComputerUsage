using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using static ComputerUsage.ComputerDatas;
using static ComputerUsage.GlobalDatas;

namespace ComputerUsage
{
    public class DataInfo
    {

        public DataInfo()
        {
            time = DateTime.Now;
            if (Set.IncludeProcesses)
            {
                processes = ProcessInfo.GetProcessInfos(GetProcessList()).ToArray();
            }
            if (Set.IncludeWindows)
            {
                windows = GetAllWindowsInfo();
            }
            if (Set.IncludeBattery)
            {
                battery = new BatteryInfo(GetBatteryStatus());
            }
            if(Set.IncludeNetwork)
            {
                pingInfos = GetNetworkStatus();
            }
            foregroundWindow = GetForegroundWindowInfo();
            mouseMoved = MouseMoved();

        }

        public DataInfo(DateTime time,
           IEnumerable< ProcessInfo> processes,
           IEnumerable<WindowInfo> windows,
            BatteryInfo battery,
            WindowInfo foregroundWindow,
            bool mouseMoved,
          List<  PingInfo> network)
        {
            this.time = time;
            if (processes != null)
            {
                this.processes = processes.ToArray();
            }
            if (windows != null)
            {
                this.windows = windows.ToArray();
            }
            this.battery = battery;
            this.foregroundWindow = foregroundWindow;
            this.mouseMoved = mouseMoved;
            this.pingInfos = network;
        }

        public DateTime time;
        public string DisplayTime => time.ToString();
        public DateTime Time => time;

        public ProcessInfo[] processes;
        public string DisplayProcessCount => processes==null? "无":processes.Length.ToString();

        public WindowInfo[] windows;
        public string DisplayWindowCount => windows==null?"无": windows.Length.ToString();
        public WindowInfo foregroundWindow;
        public string DisplayForegroundWindow
        {
            get
            {
                if (foregroundWindow.name=="")
                {
                    return "（" + foregroundWindow.className + "）";
                }
                return foregroundWindow.name;
            }
        }

        public BatteryInfo battery;
        public string DisplayBatteryLifePercent => battery==null?"无":battery.Percent + "%";
        public SolidColorBrush DisplayBatteryStatus
        {
            get
            {
                if(battery==null)
                {
                    return new SolidColorBrush(Colors.Black);
                }
                if (battery.PowerOnline)
                {
                    return new SolidColorBrush(Color.FromArgb(0xFF, 0x28, 0xB6, 0x6D));
                }
                if (battery.Percent < 20)
                {
                    return new SolidColorBrush(Color.FromArgb(0xFF, 0xff, 0x00, 0x00));
                }
                 return new SolidColorBrush(Colors.Black);
            
            }
        }


        public bool mouseMoved;
        public string DisplayMouseMoved => mouseMoved ? "●" : "";

        // public NetworkStatus network = NetworkStatus.Unknow;

        public List<PingInfo> pingInfos;

        public string DisplayNetworkStatus
        {
            //get
            //{
            //    switch(network)
            //    {
            //        case NetworkStatus.All:
            //            return "完全连接";
            //        case NetworkStatus.None:
            //            return "无连接";
            //        case NetworkStatus.Some:
            //            return "部分连接";
            //        default:
            //            return "未知";
            //    }

            get
            {
                if(pingInfos.Count==0)
                {
                    return "未知";
                }
                if(pingInfos.Any(p=>p.time==-1))
                {
                    if(pingInfos.Any(p=>p.time!=-1))
                    {
                        return "部分连接";
                    }
                    else
                    {
                        return "无连接";
                    }
                }
                return "完全连接";
            }
            
        }
    }
}

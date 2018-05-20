using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using static ComputerUsage.ComputerDatas;

namespace ComputerUsage
{
    public class DataInfo
    {

        public DataInfo()
        {
            time = DateTime.Now;
            if (RecordNeeded.Processes)
            {
                processes = ProcessInfo.GetProcessInfos(GetProcessList()).ToArray();
            }
            if (RecordNeeded.Windows)
            {
                windows = GetAllWindowsInfo();
            }
            if (RecordNeeded.Battery)
            {
                battery = new BatteryInfo(GetBatteryStatus());
            }
            foregroundWindow = GetForegroundWindowInfo();
        }

        public DataInfo(DateTime time,
           IEnumerable< ProcessInfo> processes,
           IEnumerable<WindowInfo> windows,
            BatteryInfo battery,
            WindowInfo foregroundWindow)
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
        }

        public DateTime time;
        public string DisplayTime => time.ToString();
        public DateTime Time => time;

        public ProcessInfo[] processes;
        public string DisplayProcessCount => processes==null? "无":processes.Length.ToString();

        public WindowInfo[] windows;
        public string DisplayWindowCount => windows==null?"无": windows.Length.ToString();
        public WindowInfo foregroundWindow;
        public string DisplayForegroundWindow => foregroundWindow.name + " - " + foregroundWindow.className;

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
        
        
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using static WpfCodes.Basic.Number;

namespace ComputerUsage
{
    public class ProcessInfo : INotifyPropertyChanged
    {
        public static List<ProcessInfo> GetProcessInfos(Process[] processes)
        {
            List<ProcessInfo> infos = new List<ProcessInfo>();
            foreach (var process in processes)
            {
                infos.Add(new ProcessInfo(process));
            }
            return infos;
        }
        public ProcessInfo()
        {
        }
            public ProcessInfo(Process process, bool recordAll = true)
        {
            id = process.Id;
            name = process.ProcessName;
            if (recordAll)
            {
                physicalMemory = process.WorkingSet64;
                window = process.MainWindowTitle;
                virtualMemory = process.PagedMemorySize64;
                responding = process.Responding;
                try
                {
                    mainModuleFileName = process.MainModule.FileName;
                }
                catch
                {

                }
            }
        }

        public ProcessInfo(long physicalMemory, string window, int id, long virtualMemory, string name, bool responding, string mainModuleFileName)
        {
            this.physicalMemory = physicalMemory;
            this.window = window;
            this.id = id;
            this.virtualMemory = virtualMemory;
            this.name = name;
            this.responding = responding;
            this.mainModuleFileName = mainModuleFileName;
        }

        public DateTime Time { get; set; }
        public string DisplayTime => Time.ToString("MM-dd HH:mm");

        public long physicalMemory;
        public string DisplayPhysicalMemory => ByteToFitString(physicalMemory);

        public string window;
        public string DisplayWindow => window;

        public int id;
        public string DisplayId => id.ToString();

        public string DisplayVirtualMemory => ByteToFitString(virtualMemory);
        public long virtualMemory;

        public string name;
        public string DisplayName => name;

        public string DisplayResponding => responding ? "是" : "否";

        public string DisplayMainModuleFileName => mainModuleFileName;

        public bool responding;

        public string mainModuleFileName = "";

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanAddToMonitor
        {
            get => !ProcessMonitorHelper.MonitoringProcesses.Contains(this);
            set => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanAddToMonitor"));
        }


    }
}

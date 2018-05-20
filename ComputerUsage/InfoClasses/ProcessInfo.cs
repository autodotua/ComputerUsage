using System.Collections.Generic;
using System.Diagnostics;

using static WPfCodes.Basic.Number;

namespace ComputerUsage
{
    public class ProcessInfo
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

        public ProcessInfo(Process process)
        {
            id = process.Id;
            physicalMemory = process.WorkingSet64;
            window = process.MainWindowTitle;
            virtualMemory = process.PagedMemorySize64;
            name = process.ProcessName;
            responding = process.Responding;
        }

        public ProcessInfo(long physicalMemory, string window, int id, long virtualMemory, string name, bool responding)
        {
            this.physicalMemory = physicalMemory;
            this.window = window;
            this.id = id;
            this.virtualMemory = virtualMemory;
            this.name = name;
            this.responding = responding;
        }

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
        public bool responding;
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WpfCodes.Basic.Number;

namespace ComputerUsage
{
    public class ProcessMonitorInfo
    {
        public ProcessMonitorInfo()
        {
        }
        public ProcessMonitorInfo(ProcessInfo info)
        {
            Process process = Process.GetProcessById(info.id);
            if (info.name != process.ProcessName)
            {
                throw new Exception("进程已改变");
            }
            Time = DateTime.Now;

            try
            {
                PagedMemorySize = process.PagedMemorySize64;
            }
            catch { }
            try
            {
                PagedSystemMemorySize = process.PagedSystemMemorySize64;
            }
            catch { }
            try
            {
                NonpagedSystemMemorySize = process.NonpagedSystemMemorySize64;
            }
            catch { }
            try
            {
                WorkingSet = process.WorkingSet64;
            }
            catch { }
            try
            {

                VirtualMemorySize = process.VirtualMemorySize64;
            }
            catch { }
            try
            {

                PrivateMemorySize = process.PrivateMemorySize64;
            }
            catch { }
            try
            {

                PrivilegedProcessorTime = process.PrivilegedProcessorTime;
            }
            catch { }
            try
            {

                UserProcessorTime = process.UserProcessorTime;
            }
            catch { }
            try
            {

                TotalProcessorTime = process.TotalProcessorTime;
            }
            catch { }
            try
            {

                Responding = process.Responding;
            }
            catch { }
            try
            {

                HandleCount = process.HandleCount;
            }
            catch { }
            try
            {

                ThreadCount = process.Threads.Count;
            }
            catch { }
        }

        public DateTime Time { get; set; }

        public long PagedMemorySize { get; set; }

        public long PagedSystemMemorySize { get; set; }

        public long NonpagedSystemMemorySize { get; set; }

        public long PrivateMemorySize { get; set; }

        public long WorkingSet { get; set; }

        public long VirtualMemorySize { get; set; }

        public TimeSpan PrivilegedProcessorTime { get; set; }

        public TimeSpan UserProcessorTime { get; set; }

        public TimeSpan TotalProcessorTime { get; set; }

        public bool Responding { get; set; }

        public int HandleCount { get; set; }

        public int ThreadCount { get; set; }


        [JsonIgnore]
        public string DisplayPagedSystemMemorySize => ByteToFitString(PagedSystemMemorySize);

        [JsonIgnore]
        public string DisplayPrivateMemorySize => ByteToFitString(PrivateMemorySize);

        [JsonIgnore]
        public string DisplayPagedMemorySize => ByteToFitString(PagedMemorySize);

        [JsonIgnore]
        public string DisplayWorkingSet => ByteToFitString(WorkingSet);

        [JsonIgnore]
        public string DisplayVirtualMemorySize => ByteToFitString(VirtualMemorySize);

        [JsonIgnore]
        public string DisplayTotalProcessorTime => TimeToString(TotalProcessorTime);

        private string TimeToString(TimeSpan time)
        {
            return time.ToString("h\\:mm\\:ss\\.f");
        }
    }
}

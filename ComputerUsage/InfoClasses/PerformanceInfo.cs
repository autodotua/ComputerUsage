using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WpfCodes.Basic.Number;
using static WpfCodes.WindowsApi.PerformanceInformation;

namespace ComputerUsage
{
    public class PerformanceInfo
    {
        public PerformanceInfo()
        {
            MemoryInfo memory = GetMemoryInfo();
            CpuUsagePercent = ComputerDatas.CpuUsage;
            FreePhysicalMemory = memory.AvailablePhysical;
            TotalPhysicalMemory = memory.TotalPhysical;
            FreePageFile = memory.AvailablePageFile;
            TotalPageFile = memory.TotalPageFile;
            DriveInfo drive = DriveInfo.GetDrives().FirstOrDefault(p => p.Name == "C:\\");
            if (drive != null)
            {
                try
                {

                MainDiskPartitionFreeSize = drive.AvailableFreeSpace;
                MainDiskPartitionTotalSize = drive.TotalSize;
                }
                catch
                {
                    MainDiskPartitionFreeSize = -1;
                    MainDiskPartitionTotalSize = -1;
                }
            }
        }

        public PerformanceInfo(int cpuUsagePercent, long freeMemory, long totalMemory, long freePage, long totalPage, long mainDiskPartitionTotalSize, long mainDiskPartitionFreeSize)
        {
            CpuUsagePercent = cpuUsagePercent;
            FreePhysicalMemory = freeMemory;
            TotalPhysicalMemory = totalMemory;
            FreePageFile = freePage;
            TotalPageFile = totalPage;
            MainDiskPartitionTotalSize = mainDiskPartitionTotalSize;
            MainDiskPartitionFreeSize = mainDiskPartitionFreeSize;
        }

        public int CpuUsagePercent { get;private set; }
        public long FreePhysicalMemory { get; private set; }
        public long TotalPhysicalMemory { get; private set; }
        public long FreePageFile{ get; private set; }
        public long TotalPageFile { get; private set; }
        public long MainDiskPartitionTotalSize { get; private set; }
        public long MainDiskPartitionFreeSize { get; private set; }
    }
}

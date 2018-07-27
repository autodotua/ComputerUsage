using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static WpfCodes.Basic.Number;

namespace ComputerUsage
{
    /// <summary>
    /// WinProcessesDetail.xaml 的交互逻辑
    /// </summary>
    public partial class PgPerformanceDetail : Page
    {
        ObservableCollection<KeyValueRawValueInfo> infos = new ObservableCollection<KeyValueRawValueInfo>();
        public PgPerformanceDetail(PerformanceInfo performances)
        {
            InitializeComponent();
            lvw.CloseTriggers();


            infos.Add(new KeyValueRawValueInfo("CPU使用率", performances.CpuUsagePercent + "%", performances.CpuUsagePercent));
            infos.Add(new KeyValueRawValueInfo());
            infos.Add(new KeyValueRawValueInfo("物理内存总量", ByteToFitString(performances.TotalPhysicalMemory), performances.TotalPhysicalMemory));
            infos.Add(new KeyValueRawValueInfo("物理内存已用", ByteToFitString(performances.TotalPhysicalMemory - performances.FreePhysicalMemory), performances.TotalPhysicalMemory - performances.FreePhysicalMemory));
            infos.Add(new KeyValueRawValueInfo("物理内存可用", ByteToFitString(performances.FreePhysicalMemory), performances.FreePhysicalMemory));
            infos.Add(new KeyValueRawValueInfo());
            infos.Add(new KeyValueRawValueInfo("页面文件总量", ByteToFitString(performances.TotalPageFile), performances.TotalPageFile));
            infos.Add(new KeyValueRawValueInfo("页面文件已用", ByteToFitString(performances.TotalPageFile - performances.FreePageFile), performances.TotalPageFile - performances.FreePageFile));
            infos.Add(new KeyValueRawValueInfo("页面文件可用", ByteToFitString(performances.FreePageFile), performances.FreePageFile));
            infos.Add(new KeyValueRawValueInfo());
            infos.Add(new KeyValueRawValueInfo("主分区总量", performances.MainDiskPartitionTotalSize == -1 ? "未知" : ByteToFitString(performances.MainDiskPartitionTotalSize), performances.MainDiskPartitionTotalSize));
            infos.Add(new KeyValueRawValueInfo("主分区已用", performances.MainDiskPartitionFreeSize == -1 || performances.MainDiskPartitionTotalSize == -1 ? "未知" : ByteToFitString(performances.MainDiskPartitionTotalSize - performances.MainDiskPartitionFreeSize), performances.MainDiskPartitionTotalSize - performances.MainDiskPartitionFreeSize));
            infos.Add(new KeyValueRawValueInfo("主分区可用", performances.MainDiskPartitionFreeSize == -1 ? "未知" : ByteToFitString(performances.MainDiskPartitionFreeSize), performances.MainDiskPartitionFreeSize));



            lvw.ItemsSource = infos;
        }

    }

    class KeyValueRawValueInfo
    {
        public KeyValueRawValueInfo(string type, string value, object rawValue)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            RawValue = rawValue ?? throw new ArgumentNullException(nameof(rawValue));
        }
        public KeyValueRawValueInfo()
        {
            Type = "";
            Value = "";
            RawValue = "";
        }
        public string Type { get; private set; }
        public string Value { get; private set; }
        public object RawValue { get; private set; }
    }

}

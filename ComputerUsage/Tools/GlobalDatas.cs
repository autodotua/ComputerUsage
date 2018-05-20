using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerUsage
{
    public static class GlobalDatas
    {
        public static XmlHelper xml = new XmlHelper();

        public static int ItemsCountOfEachPage { get; set; }

        public static TimeSpan TimerInterval { get; set; }

        public static Properties.Settings Set => Properties.Settings.Default;

        public static void LoadSettings()
        {
            ItemsCountOfEachPage = Set.ItemCountOfEachPage;
            TimerInterval = TimeSpan.FromSeconds(Set.Interval);
            RecordNeeded.Battery = Set.IncludeBattery;
            RecordNeeded.Windows = Set.IncludeWindows;
            RecordNeeded.Processes = Set.IncludeProcesses;
        }

        public static void SaveSettings()
        {
            Set.ItemCountOfEachPage = ItemsCountOfEachPage;
            Set.Interval =(int) TimerInterval.TotalSeconds;
            Set.IncludeBattery = RecordNeeded.Battery;
            Set.IncludeWindows = RecordNeeded.Windows;
            Set.IncludeProcesses = RecordNeeded.Processes;
            Set.Save();
        }
    }
}

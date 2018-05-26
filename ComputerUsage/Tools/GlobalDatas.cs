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

        //public static int Set.ItemsCountOfEachPage { get; set; }

        //public static int TimerInterval { get; set; }

        // public static Properties.Settings Set => Properties.Settings.Default;

       public static  string ConfigDirectory => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ComputerUsage";

        public static Settings Set = new Settings();
        public static void LoadSettings()
        {
            //Set.ItemsCountOfEachPage = Set.ItemCountOfEachPage;
            //TimerInterval = Set.Interval;
            //Set.IncludeBattery = Set.IncludeBattery;
            //Set.IncludeWindows = Set.IncludeWindows;
            //Set.IncludeProcesses = Set.IncludeProcesses;
        }

        public static void SaveSettings()
        {
            //Set.ItemCountOfEachPage = Set.ItemsCountOfEachPage;
            //Set.Interval = TimerInterval;
            //Set.IncludeBattery = Set.IncludeBattery;
            //Set.IncludeWindows = Set.IncludeWindows;
            //Set.IncludeProcesses = Set.IncludeProcesses;
            Set.Save();
        }



        public static string MinuteToTimeString(int minute)
        {
            if (minute < 60)
            {
                return  "00:" + minute.ToString("00");
            }
            else
            {
                return (minute / 60).ToString("00") + ":" + (minute % 60).ToString("00");
            }
        }
    }
}

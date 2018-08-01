using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerUsage
{
    public static class GlobalDatas
    {
        public static XmlHelper xml;

        //public static int Set.ItemsCountOfEachPage { get; set; }

        //public static int TimerInterval { get; set; }

        // public static Properties.Settings Set => Properties.Settings.Default;

        public static BackgroundWork background;

        public static void Load()
        {
            xml = new XmlHelper();
            Set = new Settings();
        }

        public static string ConfigDirectory
        {
            get
            {
                if (Directory.Exists(configDirectory))
                {
                    return configDirectory;
                }
                else
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ComputerUsage";
                }
            }
            set
            {
                configDirectory = value;
            }
        }

        private static string configDirectory = "";

        public static Settings Set;

        public static bool ReadOnlyMode { get; set; } = false;


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
                return "00:" + minute.ToString("00");
            }
            else
            {
                return (minute / 60).ToString("00") + ":" + (minute % 60).ToString("00");
            }
        }
    }
}

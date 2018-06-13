﻿using System;

namespace ComputerUsage
{
    public class Settings : WpfCodes.Program.SettingsBase
    {

        protected override string Path { get =>GlobalDatas. ConfigDirectory + "\\Settings.xml"; set { }}
     

        public int ItemsCountOfEachPage { get; set; } = 50;
        public int TimerInterval { get; set; } = 59;
        public bool IncludeBattery { get; set; } = true;
        public bool IncludeNetwork { get; set; } = true;

        public bool IncludeWindows { get; set; } = false;
        public bool IncludeProcesses { get; set; } = false;
        public bool NoWindowWhenStartup { get; set; } = false;

        public int BackupInterval { get; set; } = 60;
        public int BackupCount { get; set; } = 10;

        public string PingAddress { get; set; } = "www.baidu.com"+Environment.NewLine+"www.google.com"+Environment.NewLine+"www.qq.com";
        public int PingTimeOut { get; set; } = 1000;
    }
}

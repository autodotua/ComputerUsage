using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static WpfControls.Dialog.DialogHelper;
using static ComputerUsage.GlobalDatas;
using System.Windows.Controls;
using System.IO;
using static WpfCodes.Basic.String;
using System.Globalization;

namespace ComputerUsage
{
    public static class ProcessMonitorHelper
    {
        private static Timer processMonitorTimer;


        public static void Load()
        {
            processMonitorTimer = new Timer(new TimerCallback(p => Update()), null, 0, Set.ProcessMonitorTimerInterval * 1000);
        }
        public static Dictionary<ProcessInfo, DateTime> MonitoringProcessesStartTime { get; set; } = new Dictionary<ProcessInfo, DateTime>();

        public static Dictionary<ProcessInfo, ObservableCollection<ProcessMonitorInfo>> MonitoringProcessesDetails { get; set; } = new Dictionary<ProcessInfo, ObservableCollection<ProcessMonitorInfo>>();

        public static ObservableCollection<ProcessInfo> MonitoringProcesses { get; set; } = new ObservableCollection<ProcessInfo>();

        public static void CreatNewMonitor(ProcessInfo info)
        {
            if (MonitoringProcesses.Contains(info))
            {
                ShowError("重复了");
                return;
            }
            MonitoringProcesses.Add(info);
            MonitoringProcessesDetails.Add(info, new ObservableCollection<ProcessMonitorInfo>());
            MonitoringProcessesStartTime.Add(info, DateTime.Now);
            info.CanAddToMonitor = false;
        }

        private static void Update()
        {
            List<ProcessInfo> removeNeeded = new List<ProcessInfo>();

            foreach (var process in MonitoringProcesses)
            {
                var collection = MonitoringProcessesDetails[process];
                try
                {
                    var info = new ProcessMonitorInfo(process);
                    App.Current.Dispatcher.Invoke(() =>
                   {
                       collection.Add(info);
                   });

                }
                catch (Exception ex)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ShowError(ex.Message);
                    });
                    removeNeeded.Add(process);
                }
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (dataGrid != null && dataGrid.ItemsSource != null
                    && !dataGrid.IsFocused && dataGrid.ItemsSource == collection)
                    {
                        dataGrid.ScrollIntoView(collection.Last());
                    }
                });
            }

            if ((DateTime.Now - lastSaveTime).TotalSeconds > Set.ProcessMonitorSaveInterval)
            {
                foreach (var process in MonitoringProcesses)
                {
                    Save(process);
                }
                lastSaveTime = DateTime.Now;
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                if (removeNeeded.Count > 0)
                {
                    foreach (var process in removeNeeded)
                    {
                        Remove(process);
                    }

                }
            });
        }

        private static void Save(ProcessInfo info)
        {
            try
            {
                if (!Directory.Exists(ConfigDirectory + "\\ProcessMonitor"))
                {
                    Directory.CreateDirectory(ConfigDirectory + "\\ProcessMonitor");
                }
                foreach (var item in MonitoringProcesses)
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(MonitoringProcessesDetails[item]);
                    File.WriteAllText(ConfigDirectory + "\\ProcessMonitor\\"
                      + MonitoringProcessesStartTime[item].ToString("yyyyMMdd-HHmmss")
                       + "_" + item.DisplayId + "_" + item.name + ".json", json);
                }
            }
            catch (Exception ex)
            {
                ShowException("保存失败", ex);
            }
        }

        public static Dictionary<ProcessInfo, FileInfo> GetHistory()
        {
            Dictionary<ProcessInfo, FileInfo> histories = new Dictionary<ProcessInfo, FileInfo>();
            if (Directory.Exists(ConfigDirectory + "\\ProcessMonitor"))
            {
                foreach (var file in Directory.EnumerateFiles(ConfigDirectory + "\\ProcessMonitor").Select(p => new FileInfo(p)))
                {
                    try
                    {

                        string name = file.Name.RemoveEnd(file.Extension);
                        string[] str = name.Split('_');
                        if (str.Length < 3)
                        {
                            continue;
                        }
                        DateTime time = DateTime.ParseExact(str[0], "yyyyMMdd-HHmmss", CultureInfo.CurrentCulture);
                        int id = int.Parse(str[1]);
                        string processName = name.RemoveStart(str[0] + "_" + str[1] + "_");
                        ProcessInfo info = new ProcessInfo() { Time = time, id = id, name = processName };
                        histories.Add(info, file);
                    }
                    catch
                    {

                    }
                }
            }
            return histories;
        }


        public static void Remove(ProcessInfo process)
        {
            Save(process);

            MonitoringProcesses.Remove(process);
            MonitoringProcessesDetails.Remove(process);
            MonitoringProcessesStartTime.Remove(process);
        }

        private static DateTime lastSaveTime = DateTime.Now;
        public static DataGrid dataGrid;


    }
}

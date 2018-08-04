using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ComputerUsage
{
    /// <summary>
    /// UcProcessMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class UcProcessMonitor : UserControl, INotifyPropertyChanged
    {
        public UcProcessMonitor()
        {
            InitializeComponent();
            DataContext = this;
            lvwMonitoringProcesses.SelectionChanged += Lvw_SelectionChanged;
            ProcessMonitorHelper.dataGrid = table;
            lvwHistory.CloseTriggers();
            if (GlobalDatas.ReadOnlyMode)
            {
                for (int i = 0; i < 6; i++)
                {
                    g.RowDefinitions[i].Height = new GridLength(0);
                }
                expHistory.ExpandDirection = ExpandDirection.Down;
                expHistory.IsExpanded = true;
            }

        }

        private void Lvw_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvwMonitoringProcesses.SelectedItem is ProcessInfo selectedItem)
            {
                SetTableItemsSource(ProcessMonitorHelper.MonitoringProcessesDetails[selectedItem]);
            }
        }

        private void SetTableItemsSource(ObservableCollection<ProcessMonitorInfo> infos)
        {
            CurrentProcessesMonitor = infos;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentProcessesMonitor)));

        }

        public ObservableCollection<ProcessMonitorInfo> CurrentProcessesMonitor { get; set; } = new ObservableCollection<ProcessMonitorInfo>();

        public ObservableCollection<ProcessInfo> ProcessesMonitorHistory { get; set; } = new ObservableCollection<ProcessInfo>();

        public ObservableCollection<ProcessInfo> RunningProcessesList { get; set; } = new ObservableCollection<ProcessInfo>();

        public async Task LoadProcesses()
        {
            RunningProcessesList.Clear();
            ProcessInfo[] infos = null;
            await Task.Run(() =>
             {
                 infos = Process.GetProcesses().Select(p => new ProcessInfo(p, false)).ToArray();
             });
            foreach (var info in infos)
            {
                RunningProcessesList.Add(info);
            }
        }

        private async void ModernExpander_Expanded(object sender, RoutedEventArgs e)
        {
            if ((sender as Expander).Name == nameof(expNew))
            {
                if (expHistory.IsExpanded)
                {
                    expHistory.IsExpanded = false;
                }
                //(App.Current.MainWindow as MainWindow).IsBusy = true;
                await LoadProcesses();
                //(App.Current.MainWindow as MainWindow).IsBusy = false;

            }
            else
            {
                if (expNew.IsExpanded)
                {
                    expNew.IsExpanded = false;
                }
                ProcessesMonitorHistory.Clear();
                histories = ProcessMonitorHelper.GetHistory();
                foreach (var item in histories.Keys)
                {
                    ProcessesMonitorHistory.Add(item);
                }
            }
        }
        Dictionary<ProcessInfo, FileInfo> histories;

        public event PropertyChangedEventHandler PropertyChanged;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProcessMonitorHelper.CreatNewMonitor(RunningProcessesList.First(p => p.DisplayId == (sender as Button).Tag as string));
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = txtSearch.Text.ToLower();
            if (text.Trim() == "")
            {
                return;
            }
            lvwProcesses.SelectedItems.Clear();
            foreach (var item in RunningProcessesList.Where(p => p.name.ToLower().Contains(text) || p.DisplayId.Contains(text)))
            {
                lvwProcesses.SelectedItems.Add(item);
            }
            if (lvwProcesses.SelectedItems.Count > 0)
            {
                lvwProcesses.ScrollIntoView(lvwProcesses.SelectedItems[0]);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ProcessMonitorHelper.Remove(ProcessMonitorHelper.MonitoringProcesses.First(p => p.DisplayId == (sender as Button).Tag as string));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {

                FileInfo file = histories[ProcessesMonitorHistory.First(p => p.DisplayId == (sender as Button).Tag as string)];
                string jsonText = File.ReadAllText(file.FullName);
                var items = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<ProcessMonitorInfo>>(jsonText);
                SetTableItemsSource(items);
            }
            catch (Exception ex)
            {
                WpfControls.Dialog.DialogHelper.ShowException("加载历史信息失败", ex);
            }
        }
    }
}

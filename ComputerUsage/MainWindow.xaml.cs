using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using WpfControls;
using static ComputerUsage.ComputerDatas;
using static ComputerUsage.GlobalDatas;

namespace ComputerUsage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WpfControls.Win10Style.ModernWindow
    {

        public MainWindow()
        {
            WpfControls.Dialog.DialogHelper.DefautDialogOwner = this;

            InitializeComponent();
            UpdateColor(new SolidColorBrush(Colors.White));
            if (ReadOnlyMode)
            {
                btnReadOnly.Visibility = Visibility.Hidden;
                Title = "计算机使用情况统计 - 只读模式";
            }
            Icon = 
            Imaging.CreateBitmapSourceFromHBitmap(
            Properties.Resources.ICON.ToBitmap().GetHbitmap(),
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
        }
        private void UpdateColor(SolidColorBrush value)
        {
            Resources["back"] = value;
            DarkerBrushConverter.GetDarkerColor(value, out SolidColorBrush darker1, out SolidColorBrush darker2, out SolidColorBrush darker3, out SolidColorBrush darker4);
            Resources["darker1"] = darker1;
            Resources["darker2"] = darker2;
            Resources["darker3"] = darker3;
            Resources["darker4"] = darker4;

        }

        //public void AddToList(DataInfo info)
        //{
        //    if (tab.SelectedIndex == 0)
        //    {
        //        ucHistoryList.AddToList(info);
        //    }
        //}

        private void WindowLoadedEventHandler(object sender, RoutedEventArgs e)
        {


        }

        private async void TabItem_Selected(object sender, RoutedEventArgs e)
        {
            if (sender == tabSettings)
            {
                ucSettings.Initialize();
            }
            else if (sender == tabClipboard)
            {
                ucClipboard.Load();
            }
            //else if (sender == tabProcessMonitor)
            //{
            //    IsBusy = true;
            //    await ucProcessMonitor.LoadProcesses();
            //    IsBusy = false;x
            //}
        }

        private void ChartBtnClickEventHandler(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Tag)
            {
                case "1":
                    var ucBlockChart = new UcBlockChart();
                    ucBlockChart.LoadDateComboBox();
                    frmChart.Content = ucBlockChart;
                    break;
                case "2":
                    var ucForegroundWindowList = new UcForegroundWindowList();
                    frmChart.Content = ucForegroundWindowList;
                    break;
                case "3":
                    var ucBatteryChart = new UcBatteryChart();
                    frmChart.Content = ucBatteryChart;
                    break;
                case "4":
                    var ucPerformanceChart = new UcPerformanceChart();
                    frmChart.Content = ucPerformanceChart;
                    break;
            }
        }

        public bool IsBusy
        {
            set
            {
                if (value)
                {
                    loading.Show();
                }
                else
                {
                    loading.Hide();
                }
            }
        }

        private void btnReadOnly_Click(object sender, RoutedEventArgs e)
        {
            string path = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(path, "readonly");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (ReadOnlyMode)
            {
                App.Current.Shutdown();
            }
        }

        private void tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void btnShutdown_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }


}

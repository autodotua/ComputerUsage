using System;
using System.Collections.Generic;
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
using static ComputerUsage.GlobalDatas;

namespace ComputerUsage
{
    /// <summary>
    /// UcSettings.xaml 的交互逻辑
    /// </summary>
    public partial class UcSettings : UserControl
    {
        public UcSettings()
        {
            InitializeComponent();
            sldInterval.TextConvert = p => p.ToString() + "秒";
            sldItemsCountOfEachPage.TextConvert = p => p.ToString() + "条";
        }

        public void Initialize()
        {
            sldInterval.Value = TimerInterval.TotalSeconds;
            sldItemsCountOfEachPage.Value = ItemsCountOfEachPage;
            chkBattery.IsChecked = RecordNeeded.Battery;
            chkProcesses.IsChecked = RecordNeeded.Processes;
            chkWindows.IsChecked = RecordNeeded.Windows;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == btnReInitialize)
            {
                Initialize();
            }
            else if (btn == btnOk)
            {
                TimerInterval = TimeSpan.FromSeconds((int)sldInterval.Value);
                ItemsCountOfEachPage = (int)sldItemsCountOfEachPage.Value;
                RecordNeeded.Battery = chkBattery.IsChecked.Value;
                RecordNeeded.Processes = chkProcesses.IsChecked.Value;
                RecordNeeded.Windows = chkWindows.IsChecked.Value;
                SaveSettings();

                btn.IsEnabled = false;
                await Task.Delay(1000);
                btn.IsEnabled = true;
            }
        }
    }
}

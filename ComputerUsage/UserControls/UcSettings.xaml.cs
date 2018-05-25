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
            if(WpfCodes.Program.Startup.WillRunWhenStartup("ComputerUsage"))
            {
                if(Set.NoWindowWhenStartup)
                {
                    cbbStartUp.SelectedIndex = 2;
                }
                else
                {
  cbbStartUp.SelectedIndex = 1;
                }
            }
            else
            {
                cbbStartUp.SelectedIndex = 0;
            }
        }

        public void Initialize()
        {
            sldInterval.Value = Set.TimerInterval;
            sldItemsCountOfEachPage.Value = Set.ItemsCountOfEachPage;
            chkBattery.IsChecked = Set.IncludeBattery;
            chkProcesses.IsChecked = Set.IncludeProcesses;
            chkWindows.IsChecked = Set.IncludeWindows;
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
                Set.TimerInterval = (int)sldInterval.Value;
                Set.ItemsCountOfEachPage = (int)sldItemsCountOfEachPage.Value;
                Set.IncludeBattery = chkBattery.IsChecked.Value;
                Set.IncludeProcesses = chkProcesses.IsChecked.Value;
                Set.IncludeWindows = chkWindows.IsChecked.Value;
                if(cbbStartUp.SelectedIndex==0)
                {
                    WpfCodes.Program.Startup.CancelRunWhenStartup("ComputerUsage");
                }
                else
                {
                    if(cbbStartUp.SelectedIndex==1)
                    {
                        WpfCodes.Program.Startup.SetRunWhenStartup("ComputerUsage");
                        Set.NoWindowWhenStartup = false;
                    }
                    else
                    {
                        WpfCodes.Program.Startup.SetRunWhenStartup("ComputerUsage","noWindow");
                        Set.NoWindowWhenStartup = true;

                    }
                }
                SaveSettings();

                btn.IsEnabled = false;
                await Task.Delay(1000);
                btn.IsEnabled = true;
            }
        }
    }
}

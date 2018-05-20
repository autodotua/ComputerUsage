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
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            WpfControls.Dialog.DialogHelper.DefautDialogOwner = this;

            InitializeComponent();
            UpdateColor(new SolidColorBrush(Colors.White));

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


        private void WindowLoadedEventHandler(object sender, RoutedEventArgs e)
        {

 

        }

        private void TabItem_Selected(object sender, RoutedEventArgs e)
        {
            var tab = sender as TabItem;
            if (tab == tabSettings)
            {
                ucSettings.Initialize();
            }
        }

        private void ChartBtnClickEventHandler(object sender, RoutedEventArgs e)
        {
            switch((sender as Button).Tag)
            {
                case "1":
                   var win = new UcBlockDiagram();
                    win.LoadDateComboBox();
                    frmChart.Content = win;
                    break;
            }
        }
    }


}

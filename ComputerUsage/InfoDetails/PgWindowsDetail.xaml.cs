using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Shapes;
using static WpfCodes.Basic.Number;

namespace ComputerUsage
{
    /// <summary>
    /// WinProcessesDetail.xaml 的交互逻辑
    /// </summary>
    public partial class PgWindowsDetail : Page
    {
        ObservableCollection<WindowInfo> windowInfos;
        public PgWindowsDetail(WindowInfo[] wins)
        {
            InitializeComponent();
            lvw.CloseTriggers();
            windowInfos = new ObservableCollection<WindowInfo>(wins);
            lvw.ItemsSource = windowInfos;
        }

        private void ButtonsClickEventHandler(object sender, RoutedEventArgs e)
        {
            ProcessInfo info = windowInfos.FirstOrDefault(p => p.DisplayHandle.Equals((sender as Button).Tag)).process;
            (App.Current.MainWindow as MainWindow).ucHistoryList.frm.Content = new PgProcessesDetail(new ProcessInfo[] { info });
        }
    }

}

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
    public partial class PgProcessesDetail : Page
    {
        ObservableCollection<ProcessInfo> processInfos = new ObservableCollection<ProcessInfo>();
        public PgProcessesDetail(ProcessInfo[] processes)
        {
            InitializeComponent();
            lvw.CloseTriggers();
            lvw.ItemsSource = processInfos;
            foreach (var process in processes)
            {
                processInfos.Add(process);
            }


        }
    }

}

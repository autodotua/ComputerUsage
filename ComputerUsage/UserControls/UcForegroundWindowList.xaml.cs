using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml;
using static ComputerUsage.GlobalDatas;
using static WpfControls.Dialog.DialogHelper;

namespace ComputerUsage
{
    /// <summary>
    /// UcForegroundWindowList.xaml 的交互逻辑
    /// </summary>
    public partial class UcForegroundWindowList : UserControl
    {
        ObservableCollection<WindowClass> windowClasses = new ObservableCollection<WindowClass>();
        ObservableCollection<ForegroundWindowUsage> foregroundWindowUsages;

        public UcForegroundWindowList()
        {
            InitializeComponent();
            lvwWindowClass.CloseTriggers();
            xml.ReloadFields();
            try
            {
                windowClasses = new ObservableCollection<WindowClass>(WpfCodes.Basic.Enumerable.ImportFromCsv<WindowClass>(ConfigDirectory + "\\WindowClasses.csv"));
            }
            catch (Exception ex)
            {
                ShowException("读取配置失败", ex);
            }
            lvwWindowClass.ItemsSource = windowClasses;
            range.DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            range.DateTo = DateTime.Now.Date;

        }

        private void BtnsIntWindowClassClickEventHandler(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case "btnNew":
                    windowClasses.Add(new WindowClass());
                    break;
                case "btnClear":
                    windowClasses.Clear();
                    break;
                case "btnSave":
                    WpfCodes.Basic.Enumerable.ExportToCsvByClassProperties(windowClasses, ConfigDirectory + "\\WindowClasses.csv");
                    RangeDateSelectionChangedEventHandler(null, null);
                    break;
                case "btnReset":
                    try
                    {
                        windowClasses = new ObservableCollection<WindowClass>(WpfCodes.Basic.Enumerable.ImportFromCsv<WindowClass>(ConfigDirectory + "\\WindowClasses.csv"));
                    }
                    catch(Exception ex)
                    {
                        ShowException("读取文件失败！",ex);
                    }
                    lvwWindowClass.ItemsSource = windowClasses;
                    break;
            }
        }

        private void BtnRemoveClickEventHandler(object sender, RoutedEventArgs e)
        {
            windowClasses.Remove(windowClasses.FirstOrDefault(p => p.ButtonTag == (sender as Button).Tag as string));
        }



        private void TryMatch(WindowInfo info)
        {
            foreach (var windowClass in windowClasses)
            {
                if (windowClass.Name == "" || windowClass.Value == "")
                {
                    continue;
                }
                string bigKey = windowClass.PropertyTypeIndex == 0 ? info.name : info.className;

                string smallKey = windowClass.Value;

                bool flag = false;

                switch (windowClass.PropertyMatchModeIndex)
                {
                    case 0:
                        flag = bigKey.Contains(smallKey);
                        break;
                    case 1:
                        flag = bigKey == smallKey;
                        break;
                    case 2:
                        flag = Regex.IsMatch(bigKey, smallKey);
                        break;
                }

                if (flag)
                {
                    Dispatcher.Invoke(() =>
                    {

                        if (foregroundWindowUsages.Any(p => p.Name == windowClass.Name))
                        {
                            var item = foregroundWindowUsages.FirstOrDefault(p => p.Name == windowClass.Name);
                            item.count++;
                        }
                        else
                        {
                            foregroundWindowUsages.Add(new ForegroundWindowUsage(windowClass.Name));
                        }
                    });
                    return;
                }
            }
        }

        private async void RangeDateSelectionChangedEventHandler(object sender, SelectionChangedEventArgs e)
        {
            loading.Show();
            DateTime dateFrom = range.DateFrom.Value;
            DateTime dateTo = range.DateTo.Value;
            await Task.Run(() =>
            {
                foregroundWindowUsages = new ObservableCollection<ForegroundWindowUsage>();
                var datas = xml.DataElements.Where(p =>
                {
                    DateTime date = DateTime.Parse(p.GetAttribute("Time"));
                    return date >= dateFrom && date <=dateTo;
                });
                foreach (var data in datas)
                {
                    var windowElement = data.ChildNodes.Cast<XmlElement>().FirstOrDefault(p => p.Name == "ForegroundWindow");
                    if (windowElement == null)
                    {
                        continue;
                    }
                    TryMatch(XmlHelper.GetWindowInfo(windowElement));

                }

            });
            loading.Hide();
            lvw.ItemsSource = foregroundWindowUsages.OrderByDescending(p => p.count);
        }
    }

    public class ForegroundWindowUsage
    {
        public ForegroundWindowUsage(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public int count = 0;
        public string Time => MinuteToTimeString((int)(1.0*count*60/Set.TimerInterval));
    }

    public class WindowClass
    {
        public WindowClass()
        {
            ButtonTag = Guid.NewGuid().ToString();

        }


        public string Name { get; set; }
        public int PropertyTypeIndex { get; set; }
        public int PropertyMatchModeIndex { get; set; }
        public string Value { get; set; }
        public string ButtonTag { get; set; }
    }


}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using System.Windows.Threading;
using System.Xml;
using WpfControls;
using static ComputerUsage.ComputerDatas;
using static ComputerUsage.GlobalDatas;


namespace ComputerUsage
{
    /// <summary>
    /// UcHistoryList.xaml 的交互逻辑
    /// </summary>
    public partial class UcHistoryList : UserControl
    {
        int currentPageCount;
        bool needReload = true;

        ObservableCollection<DataInfo> dataHistoryInfos = new ObservableCollection<DataInfo>();
        ObservableCollection<EventInfo> eventHistoryInfos = new ObservableCollection<EventInfo>();


        public UcHistoryList()
        {
            InitializeComponent();
            lvwDataHistory.ItemsSource = dataHistoryInfos;
            lvwEventHistory.ItemsSource = eventHistoryInfos;
        }

        public void AddToList(DataInfo info)
        {
            if (currentPageCount > 0 && currentPageCount < Set.ItemsCountOfEachPage)
            {
                dataHistoryInfos.Add(info);
                ComboBoxItem comboBoxItem = cbbPageSelection.Items.Cast<ComboBoxItem>().Last();
                int first = (cbbPageSelection.Items.Count - 1) * Set.ItemsCountOfEachPage + 1;
                int last = (cbbPageSelection.Items.Count - 1) * Set.ItemsCountOfEachPage + (++currentPageCount) + 1;
                comboBoxItem.Content = GetComboBoxItemContent(first, last, comboBoxItem.Tag as string, info.DisplayTime);
                if (cbbPageSelection.SelectedIndex == cbbPageSelection.Items.Count - 1)
                {
                    lvwDataHistory.ScrollIntoView(info);
                }
            }
            else
            {
                currentPageCount = 0;
                dataHistoryInfos.Clear();
                int first = cbbPageSelection.Items.Count * Set.ItemsCountOfEachPage + 1;
                int last = cbbPageSelection.Items.Count * Set.ItemsCountOfEachPage + 1;
                cbbPageSelection.Items.Add(new ComboBoxItem()
                {
                    Content = GetComboBoxItemContent(first, last, info.DisplayTime, info.DisplayTime),
                    Tag = info.DisplayTime,
                });
                if (cbbPageSelection.SelectedIndex == cbbPageSelection.Items.Count - 2)
                {
                    needReload = false;
                    cbbPageSelection.SelectedIndex = cbbPageSelection.Items.Count - 1;

                }
                dataHistoryInfos.Add(info);

            }
        }
        private void ButtonsClickEventHandler(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Name == "btnLastPage")
            {
                cbbPageSelection.SelectedIndex--;
            }
            else if (btn.Name == "btnNextPage")
            {
                cbbPageSelection.SelectedIndex++;

            }
            else
            {
                object[] tag = btn.Tag as object[];
                DateTime time = (DateTime)tag[0];
                string type = tag[1] as string;
                Page page = null;

                DataInfo info = dataHistoryInfos.First(p => p.Time == time);

                switch (type)
                {
                    case "process":
                        if (info.processes != null)
                        {
                            page = new PgProcessesDetail(info.processes);
                        }
                        break;
                    case "window":
                        if (info.windows != null)
                        {
                            page = new PgWindowsDetail(info.windows);
                        }
                        break;
                }

                if (page != null)
                {
                    frm.Content = page;
                }
            }
        }




        private void LoadList(List<XmlElement> elements)
        {
            cbbPageSelection.Items.Clear();
            int count = elements.Count;
            int pageCount = (int)Math.Ceiling(1.0 * count / Set.ItemsCountOfEachPage);

            for (int page = 0; page < pageCount; page++)
            {
                int first = page * Set.ItemsCountOfEachPage;
                string begin = elements[first].GetAttribute("Time");
                int last;
                string end;
                if (page < pageCount - 1)
                {
                    last = page * Set.ItemsCountOfEachPage + Set.ItemsCountOfEachPage - 1;

                    end = elements[last].GetAttribute("Time");
                }
                else
                {
                    last = elements.Count - 1;
                    end = elements.Last().GetAttribute("Time");
                    if (elements == xml.DataElements)
                    {
                        foreach (var item in xml.ReadDataHistory(first, last))
                        {
                            dataHistoryInfos.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var item in xml.ReadEventHistory(first, last))
                        {
                            eventHistoryInfos.Add(item);
                        }
                    }

                }

                cbbPageSelection.Items.Add(new ComboBoxItem()
                {
                    Content = GetComboBoxItemContent(page * Set.ItemsCountOfEachPage + 1, last + 1, begin, end),
                    Tag = begin,

                });
            }
            cbbPageSelection.SelectedIndex = cbbPageSelection.Items.Count - 1;
        }

        private string GetComboBoxItemContent(int first, int last, string beginTime, string endTime)
        {
            return first.ToString() + " - " + last.ToString() + "     " + beginTime + " ~ " + endTime;
        }



        private void PageSelectionChangedEventHandler(object sender, SelectionChangedEventArgs e)
        {

            if (cbbPageSelection == null || cbbPageSelection.Items.Count == 0)
            {
                return;
            }
            int page = cbbPageSelection.SelectedIndex;
            int first = page * Set.ItemsCountOfEachPage;

            if (cbbType.SelectedIndex == 0)
            {
                dataHistoryInfos.Clear();
                int last = (page == cbbPageSelection.Items.Count - 1) ? xml.DataElements.Count - 1 : page * Set.ItemsCountOfEachPage + Set.ItemsCountOfEachPage - 1;
                SelectDataPage(page, first, last);
            }
            else
            {
                eventHistoryInfos.Clear();
                int last = (page == cbbPageSelection.Items.Count - 1) ? xml.EventElements.Count - 1 : page * Set.ItemsCountOfEachPage + Set.ItemsCountOfEachPage - 1;
                SelectEventPage(page, first, last);
            }

        }

        private void SelectDataPage(int page, int first, int last)
        {
            foreach (var item in xml.ReadDataHistory(first, last))
            {
                dataHistoryInfos.Add(item);
            }
            btnLastPage.IsEnabled = btnNextPage.IsEnabled = true;
            if (page == cbbPageSelection.Items.Count - 1)
            {
                btnNextPage.IsEnabled = false;
            }
            else if (page == 0)
            {
                btnLastPage.IsEnabled = false;
            }
        }

        private void SelectEventPage(int page, int first, int last)
        {
            foreach (var item in xml.ReadEventHistory(first, last))
            {
                eventHistoryInfos.Add(item);
            }
            btnLastPage.IsEnabled = btnNextPage.IsEnabled = true;
            if (page == cbbPageSelection.Items.Count - 1)
            {
                btnNextPage.IsEnabled = false;
            }
            else if (page == 0)
            {
                btnLastPage.IsEnabled = false;
            }
        }

        private void TypeChangedEventHandler(object sender, SelectionChangedEventArgs e)
        {
            if (needReload == false)
            {
                needReload = true;
                return;
            }
            ComboBox cbb = sender as ComboBox;
            if (lvwDataHistory == null)
            {
                return;
            }
            if (cbb.SelectedIndex == 0)
            {
                lvwDataHistory.Visibility = Visibility.Visible;
                lvwEventHistory.Visibility = Visibility.Collapsed;
                LoadList(xml.DataElements);

            }
            else
            {
                lvwDataHistory.Visibility = Visibility.Collapsed;
                lvwEventHistory.Visibility = Visibility.Visible;
                LoadList(xml.EventElements);

            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadList(xml.DataElements);
            currentPageCount = dataHistoryInfos.Count;
        }

    }
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new object[] { value, parameter };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

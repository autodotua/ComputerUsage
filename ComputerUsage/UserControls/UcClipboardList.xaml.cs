using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace ComputerUsage
{
    /// <summary>
    /// UcHistoryList.xaml 的交互逻辑
    /// </summary>
    public partial class UcClipboardList : UserControl
    {
        int currentPageCount;
        bool needReload = true;

        ObservableCollection<ClipboardInfo> clipboardHistoryInfos = new ObservableCollection<ClipboardInfo>();
        ObservableCollection<ClipboardDetailInfo> clipboardDetailInfos = new ObservableCollection<ClipboardDetailInfo>();


        public UcClipboardList()
        {
            InitializeComponent();


            lvwClipboardHistory.ItemsSource = clipboardHistoryInfos;
            lvwClipboardDetail.ItemsSource = clipboardDetailInfos;

        }

        public void Load()
        {
            clipboardHistoryInfos.Clear();
            clipboardDetailInfos.Clear();
            if(!Directory.Exists(ClipboardHelper.ClipboardHistoryPath))
            {
                Directory.CreateDirectory(ClipboardHelper.ClipboardHistoryPath);
                return;
            }
            foreach (var directory in Directory.EnumerateDirectories(ClipboardHelper.ClipboardHistoryPath))
            {
                ClipboardInfo info = null;
                try
                {
                    info = new ClipboardInfo(directory);
                }
                catch
                {
                    continue;
                }
                clipboardHistoryInfos.Add(info);
            }
        }

        private void ButtonsClickEventHandler(object sender, RoutedEventArgs e)
        {
            ClipboardInfo info = clipboardHistoryInfos.First(p => p.DisplayTime == (sender as Button).Tag as string);

            Process.Start(info.Path);
        }


        private void LoadedEventHandler(object sender, RoutedEventArgs e)
        {
        }

        private void lvwClipboardHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clipboardDetailInfos.Clear();
            if(lvwClipboardHistory.SelectedItem==null)
            {
                return;
            }
            FileInfo[] files = Directory.EnumerateFiles((lvwClipboardHistory.SelectedItem as ClipboardInfo).Path).Select(p => new FileInfo(p)).ToArray();

            foreach (var file in files)
            {
                clipboardDetailInfos.Add(new ClipboardDetailInfo(file));
            }
        }

        private void lvwClipboardDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClipboardDetailInfo info = lvwClipboardDetail.SelectedItem as ClipboardDetailInfo;
            if (info == null)
            {
                return;
            }
            grdDetail.Children.Clear();
            switch (info.Type)
            {
                case "文本":
                    TextBox txt = new TextBox()
                    {
                        IsReadOnly = true,
                        AcceptsReturn = true,
                        BorderThickness = new Thickness(),
                        Text = File.ReadAllText(info.file.FullName),
                    };
                    grdDetail.Children.Add(txt);
                    break;
                case "富文本":
                    RichTextBox rtf = new RichTextBox()
                    {
                        IsReadOnly = true,
                        AcceptsReturn = true,
                        BorderThickness = new Thickness(),

                    };
                    rtf.Selection.Load(info.file.OpenRead(), DataFormats.Rtf);
                    grdDetail.Children.Add(rtf);
                    break;
                case "图片":
                    Image image = new Image()
                    {
                        Source = new BitmapImage(new Uri(info.file.FullName)),
                    };
                    grdDetail.Children.Add(image);
                    break;
                case "网页":
                    WebBrowser web = new WebBrowser();
                    web.Navigate(new Uri(info.file.FullName));
                    grdDetail.Children.Add(web);
                    break;
                case "表格":
                    TextBox csv = new TextBox()
                    {
                        IsReadOnly = true,
                        AcceptsReturn = true,
                        BorderThickness = new Thickness(),
                        Text = File.ReadAllText(info.file.FullName),
                    };
                    grdDetail.Children.Add(csv);
                    break;
                case "文件":
                    WpfControls.FlatStyle.ListBox file = new WpfControls.FlatStyle.ListBox()
                    {
                        BorderThickness=new Thickness(),
                    };
                    foreach (var line in File.ReadAllLines(info.file.FullName))
                    {
                        file.Items.Add(line);
                    }
                    grdDetail.Children.Add(file);
                    break;
                case "所有类型":
                    WpfControls.FlatStyle.ListBox all = new WpfControls.FlatStyle.ListBox()
                    {
                        BorderThickness = new Thickness(),
                    };
                    foreach (var line in File.ReadAllLines(info.file.FullName))
                    {
                        all.Items.Add(line);
                    }
                    grdDetail.Children.Add(all);
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(((sender as Button).Tag as FileInfo).FullName);
        }
    }

}

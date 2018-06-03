using System;
using System.Collections.Generic;
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
using System.Xml;
using static ComputerUsage.GlobalDatas;
using static WpfControls.Dialog.DialogHelper;

namespace ComputerUsage
{
    /// <summary>
    /// BlockDiagram.xaml 的交互逻辑
    /// </summary>
    public partial class UcBatteryChart : UserControl
    {
        public UcBatteryChart()
        {
            InitializeComponent();
            dateRange.DateTo = DateTime.Today;
            dateRange.DateFrom = DateTime.Today;
        }
        TimeSpan span;
        private bool DrawBorder()
        {
            TimeSpan longestTimeSpan = TimeSpan.FromDays(5);
            DateTime to = dateRange.DateTo.Value;
            DateTime from = dateRange.DateFrom.Value;
            span = to - from + TimeSpan.FromDays(1);
            int totalHours = (int)span.TotalHours;
            if (span > longestTimeSpan)
            {
                ShowError("最大只允许" + longestTimeSpan.Days + "天的内容");
                return false;
            }

            for (int x = 0; x < totalHours; x++)
            {
                if (x % 2 == 1)
                {
                    DrawVerticalLine(1.0 * x / totalHours + 0.5 / totalHours, 1.0 / totalHours);
                }
            }


            for (int i = 0; i <= 10; i++)
            {
                DrawHorizentalLine(i * 0.1 * cvsBackground.Height);
            }
            return true;
        }

        private void DrawPoint(double x, double y, Brush color)
        {
            double r = 0.002;
            Ellipse e = new Ellipse()
            {
                Width = 2 * r,
                Height = 2 * r,
                Stroke = color,
                StrokeThickness = r,
            };

            Canvas.SetLeft(e, x - r);
            Canvas.SetTop(e, y - r);
            cvs.Children.Add(e);
        }

        private void DrawVerticalLine(double x, double thickness)
        {
            Line line = new Line()
            {
                X1 = x,
                X2 = x,
                Y1 = 0,
                Y2 = cvsBackground.Height,
                Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xEE, 0xEE, 0xEE)),
                StrokeThickness = thickness,
            };
            cvsBackground.Children.Add(line);
        }


        private void DrawHorizentalLine(double y)
        {
            Line line = new Line()
            {
                X1 = 0,
                X2 = 1,
                Y1 = y,
                Y2 = y,
                Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xAA, 0xAA, 0xAA)),
                StrokeThickness = 0.002,
            };
            cvsBackground.Children.Add(line);
        }


        private void DrawText(double x, double y, string text)
        {

            TextBlock tbk = new TextBlock()
            {
                Text = text,
                FontSize = 0.01,
                Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xCC, 0xCC, 0xCC)),
            };
            Canvas.SetLeft(tbk, x);
            Canvas.SetTop(tbk, y);
            cvsBackground.Children.Add(tbk);

        }

        private void DrawPoints()
        {
            Dictionary<DateTime, BatteryInfo> batteryInfos = new Dictionary<DateTime, BatteryInfo>();
            foreach (var element in xml.dataElements)
            {
                DateTime dateTime = DateTime.Parse(element.GetAttribute("Time"));

                DateTime date = dateTime.Date;
                if (date > dateRange.DateTo || date < dateRange.DateFrom)
                {
                    continue;
                }

                BatteryInfo battery = XmlHelper.GetBatteryInfo(element.ChildNodes.Cast<XmlElement>().FirstOrDefault(p => p.Name == "Battery"));

                batteryInfos.Add(dateTime, battery);


            }



            foreach (var item in batteryInfos)
            {
                DateTime time = item.Key;
                BatteryInfo battery = item.Value;

                TimeSpan currentSpan = time - dateRange.DateFrom.Value;

                double width = currentSpan.TotalMilliseconds / span.TotalMilliseconds;
                double height = cvs.Height * (1 - battery.Percent / 100.0);
                DrawPoint(width, height, battery.PowerOnline ? Brushes.Green : Brushes.Red);
            }


        }

        private void DrawTexts()
        {
            string format = "HH";
            int hourSpan = 1;
            if (span.Days > 1)
            {
                format = "dd.HH";
                if (span.Days <= 3)
                {
                    hourSpan = 4;
                }
                else
                {
                    hourSpan = 8;
                }

            }

            for (DateTime current = dateRange.DateFrom.Value; current <= dateRange.DateTo.Value.AddDays(1); current = current.AddHours(hourSpan))
            {
                double percent = (current - dateRange.DateFrom.Value).TotalMilliseconds / span.TotalMilliseconds;
                DrawText(percent - 0.005, 0.565, current.ToString(format));
            }

            for (int i = 0; i <= 10; i++)
            {
                DrawText(-0.035, i * 0.1 * cvsBackground.Height - 0.005, i * 10 + "%");
            }

        }

        DateTime lastDateBegin = DateTime.Now;
        DateTime lastDateEnd = DateTime.Now;
        private void DateRangeAvailableAndChangedEventHandler(object sender, SelectionChangedEventArgs e)
        {
            cvsBackground.Children.Clear();
            cvs.Children.Clear();
            if (DrawBorder())
            {
                DrawTexts();
                DrawPoints();
            }
        }

    }

}


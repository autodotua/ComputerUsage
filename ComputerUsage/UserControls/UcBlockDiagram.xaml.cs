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
using static ComputerUsage.GlobalDatas;

namespace ComputerUsage
{
    /// <summary>
    /// BlockDiagram.xaml 的交互逻辑
    /// </summary>
    public partial class UcBlockDiagram : UserControl
    {
        public UcBlockDiagram()
        {
            InitializeComponent();
            DrawBorder();
        }

        private void DrawBorder()
        {
            for (int x = 0; x <= 60; x++)
            {
                DrawLine(x * 5, x * 5, 0, 120);
                if(x%5==0 && x!=60)
                {
                    DrawText(x * 5+xOffset, 122, x.ToString());
                }
            }

            for (int y = 0; y <= 24; y++)
            {
                DrawLine(0, 300, y * 5, y * 5);
                if (y % 2 == 0 && y!=24)
                {
                    DrawText(0, y*5, y.ToString());
                }
            }
        }

        public void LoadDateComboBox()
        {
            cbbSelectDate.Items.Clear();
            cvs.Children.Clear();
            xml.ReloadFields();
            List<DateTime> dates = xml.DataElements.Select(p => DateTime.Parse(p.GetAttribute("Time"))).ToList();
            List<DateTime> days = new List<DateTime>();
            foreach (var date in dates)
            {
                if (!days.Any(p => p.Date.Equals(date.Date)))
                {
                    days.Add(date.Date);
                }
            }
            foreach (var day in days)
            {
                cbbSelectDate.Items.Add(new ComboBoxItem()
                {
                    Content = day.ToShortDateString(),
                    Tag = day,
                });
            }
            cbbSelectDate.SelectedIndex = cbbSelectDate.Items.Count - 1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           

        }

        double xOffset = 8;

        private void DrawLine(double x1, double x2, double y1, double y2)
        {
            Line line = new Line()
            {
                X1 = x1 + xOffset,
                X2 = x2 + xOffset,
                Y1 = y1,
                Y2 = y2,
                Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xDD, 0xDD, 0xDD)),
                StrokeThickness = 1,
            };
            cvsBackground.Children.Add(line);
        }

        private void DrawPoint(int x, int y)
        {
            Line line = new Line()
            {
                X1 = x * 5 + 0.5 + xOffset,
                X2 = x * 5 + 4 + 0.5 + xOffset,
                Y1 = y * 5 + 2.5,
                Y2 = y * 5 + 2.5,
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 4,
            };
            cvs.Children.Add(line);
        }

        private void DrawText(double x, double y, string text)
        {

            TextBlock tbk = new TextBlock()
            {
                Text = text,
                FontSize = 5,
                Foreground= new SolidColorBrush(Color.FromArgb(0xFF, 0xCC, 0xCC, 0xCC)),
            };
            Canvas.SetLeft(tbk, x);
            Canvas.SetTop(tbk, y);
            cvsBackground.Children.Add(tbk);

            //FormattedText ft = new FormattedText(text,
            // CultureInfo.CurrentCulture,
            //  FlowDirection.LeftToRight,
            //  new Typeface(new FontFamily("微软雅黑"), FontStyles.Normal,FontWeights.Normal, new FontStretch()),
            //  12D,
            //  Brushes.Black);
            //DrawingVisual dv = new DrawingVisual();
            //DrawingContext dc = dv.RenderOpen();
            //dc.DrawText(ft, new Point(x, y));

            // The following does't work because "dc" is not a UIElement

            // canvasObj.Children.Add(dc);

        }

        private void cbbSelectDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cvs.Children.Clear();

            DateTime day = (DateTime)(cbbSelectDate.SelectedItem as ComboBoxItem).Tag;
            List<DateTime> dates = xml.DataElements
                .Select(p => DateTime.Parse(p.GetAttribute("Time")))
                .Where(p => p.Date.Equals(day))
                .ToList();
            foreach (var time in dates)
            {
                DrawPoint(time.Minute, time.Hour);
            }
            if(dates.Count<60)
            {
                tbkTotalTime.Text = "共 00:" + dates.Count.ToString("00");
            }
            else
            {
                tbkTotalTime.Text = "共 "+(dates.Count/60).ToString("00")+":" + (dates.Count%60).ToString("00");
            }
        }
    }

}


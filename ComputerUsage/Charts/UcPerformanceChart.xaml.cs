using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using static ComputerUsage.GlobalDatas;
using static WpfControls.Dialog.DialogHelper;

namespace ComputerUsage
{
    /// <summary>
    /// BlockDiagram.xaml 的交互逻辑
    /// </summary>
    public partial class UcPerformanceChart : UserControl
    {
        IChartValues Values { get; set; } = new ChartValues<DateTimePerformanceInfo>();
        bool isCpuUsage = false;
        public UcPerformanceChart()
        {
            InitializeComponent();
            dateRange.DateTo = DateTime.Today;
            dateRange.DateFrom = DateTime.Today;
            DataContext = this;
            c.Series.Configuration = Mappers.Xy<DateTimePerformanceInfo>()
                   .X(p => p.DateTime.Ticks)
                   .Y(p => p.Value);

            axisX.LabelFormatter = value => new DateTime((long)value).ToString("dd日HH时");
            axisY.LabelFormatter = p =>isCpuUsage?(int)p+"%": WpfCodes.Basic.Number.ByteToFitString((long)p);
            s.Values = Values;
            

        }


        private void DrawPoints()
        {
            Values.Clear();
            Dictionary<DateTime, PerformanceInfo> performanceInfos = new Dictionary<DateTime, PerformanceInfo>();
            foreach (var element in xml.dataElements)
            {
                DateTime dateTime = DateTime.Parse(element.GetAttribute("Time"));
                DateTime date = dateTime.Date;
                if (date > dateRange.DateTo || date < dateRange.DateFrom)
                {
                    continue;
                }

                XmlElement child = element.ChildNodes.Cast<XmlElement>().FirstOrDefault(p => p.Name == "Performance");
                if (child != null)
                {
                    try
                    {
                        performanceInfos.Add(dateTime, XmlHelper.GetPerformanceInfo(child));
                    }
                    catch
                    {

                    }
                }

            }



            foreach (var item in performanceInfos)
            {
                DateTime time = item.Key;
                PerformanceInfo per = item.Value;
                switch(cbbType.SelectedIndex)
                {
                    case 0:
                        Values.Add(new DateTimePerformanceInfo(time,per.TotalPhysicalMemory- per.FreePhysicalMemory));
                        break;
                    case 1:
                        Values.Add(new DateTimePerformanceInfo(time, per.FreePhysicalMemory));
                        break;
                    case 2:
                        Values.Add(new DateTimePerformanceInfo(time, per.TotalPageFile - per.FreePageFile));
                        break;
                    case 3:
                        Values.Add(new DateTimePerformanceInfo(time, per.FreePageFile));
                        break;
                    case 4:
                        Values.Add(new DateTimePerformanceInfo(time, per.MainDiskPartitionTotalSize - per.MainDiskPartitionFreeSize));
                        break;
                    case 5:
                        Values.Add(new DateTimePerformanceInfo(time, per.MainDiskPartitionFreeSize));
                        break;
                    case 6:
                        Values.Add(new DateTimePerformanceInfo(time, per.CpuUsagePercent));
                        break;
                }
            }

            isCpuUsage = false;
            switch (cbbType.SelectedIndex)
            {
                case 0:
                    axisY.MaxValue = performanceInfos.Max(p => p.Value.TotalPhysicalMemory);
                    break;
                case 1:
                    axisY.MaxValue = performanceInfos.Max(p => p.Value.TotalPhysicalMemory);
                    break;
                case 2:
                    axisY.MaxValue = performanceInfos.Max(p => p.Value.TotalPageFile);
                    break;
                case 3:
                    axisY.MaxValue = performanceInfos.Max(p => p.Value.TotalPageFile);
                    break;
                case 4:
                    axisY.MaxValue = performanceInfos.Max(p => p.Value.MainDiskPartitionTotalSize);
                    break;
                case 5:
                    axisY.MaxValue = performanceInfos.Max(p => p.Value.MainDiskPartitionTotalSize);
                    break;
                case 6:
                    isCpuUsage = true;
                    axisY.MaxValue = 100;
                    break;
            }

            axisY.MinValue = 0;
           axisX.MinValue = axisX.MaxValue = double.NaN;
        }


        DateTime lastDateBegin = DateTime.Now;
        DateTime lastDateEnd = DateTime.Now;
        private void DateRangeAvailableAndChangedEventHandler(object sender, SelectionChangedEventArgs e)
        {

            DrawPoints();
        }
        
    }

    public class DateTimePerformanceInfo
    {
        public DateTimePerformanceInfo(DateTime dateTime, double value)
        {
            DateTime = dateTime;
            Value = value;
        }

        public DateTime DateTime { get; set; }
        public double Value { get; set; }

    }
}


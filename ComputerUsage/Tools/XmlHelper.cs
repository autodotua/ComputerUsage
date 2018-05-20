using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static ComputerUsage.GlobalDatas;

namespace ComputerUsage
{
    public class XmlHelper
    {
        XmlDocument xml = new XmlDocument();
        XmlElement root;
        readonly string xmlDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ComputerUsage";

        string xmlPath;

        public XmlHelper()
        {
            xmlPath = xmlDirectory + "\\history.xml";
            if (!File.Exists(xmlPath))
            {
                Directory.CreateDirectory(xmlDirectory);
                XmlDeclaration xdec = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
                xml.AppendChild(xdec);
                root = xml.CreateElement("ComputerUsage");
                xml.AppendChild(root);
                xml.Save(xmlPath);
            }
            else
            {
                xml.Load(xmlPath);
                root = xml.LastChild as XmlElement;
            }
            ReloadFields();
            WriteStart();
        }

        public void ReloadFields()
        {
            dataElements = root.ChildNodes.Cast<XmlElement>().Where(p => p.Name == "Data").ToList();
            eventElements = root.ChildNodes.Cast<XmlElement>().Where(p => p.Name == "Event").ToList();
        }

        ~XmlHelper()
        {
            XmlElement element = xml.CreateElement("Event");
            element.SetAttribute("Time", DateTime.Now.ToString());
            element.SetAttribute("Type", "程序关闭");
            root.AppendChild(element);
            xml.Save(xmlPath);
        }

        public void WriteStart()
        {
            XmlElement element = xml.CreateElement("Event");
            element.SetAttribute("Time", DateTime.Now.ToString());
            element.SetAttribute("Type", "程序启动");
            root.AppendChild(element);
            xml.Save(xmlPath);
        }

        public int Count => root.ChildNodes.Count;
        public List<XmlElement> dataElements;
        public List<XmlElement> eventElements;

        public List<XmlElement> DataElements => dataElements;
        public List<XmlElement> EventElements => eventElements;

        public int DataCount => DataElements.Count;
        public int EventCount => EventElements.Count;

        public List<DataInfo> ReadDataHistory(int first, int last)
        {
            List<DataInfo> infos = new List<DataInfo>();
            int current = first;
            while (current<= last)
            {

                if (current >= DataCount)
                {
                    break;
                }

                XmlElement element = dataElements[current] as XmlElement;
                //if (element.Name == "Event")
                //{
                //    CurrentItemColor = 1 - CurrentItemColor;
                //    continue;
                //}
                XmlElement winElements = null;
                XmlElement batteryElement = null;
                XmlElement processElements = null;
                XmlElement foregroundWindowElement = null;
                foreach (XmlElement child in element.ChildNodes)
                {
                    switch (child.Name)
                    {

                        case "Battery":
                            batteryElement = child;
                            break;
                        case "Processes":
                            processElements = child;
                            break;
                        case "Windows":
                            winElements = child;
                            break;
                        case "ForegroundWindow":
                            foregroundWindowElement = child;
                            break;
                    }
                }

                //if (batteryElement == null || processElements == null || winElements == null || foregroundWindowElement == null)
                //{
                //    throw new Exception("XML文档被篡改");
                //}
                DateTime time = DateTime.Parse(element.GetAttribute("Time"));
                List<WindowInfo> wins=null;
                if (winElements!=null)
                {
                    wins = new List<WindowInfo>();
                    foreach (XmlElement winElement in winElements.ChildNodes)
                    {

                        wins.Add(GetWindowInfo(winElement));
                    }
                }
                List<ProcessInfo> pros=null;
                if (processElements!=null)
                {
                     pros = new List<ProcessInfo>();
                    foreach (XmlElement processElement in processElements.ChildNodes)
                    {

                        pros.Add(GetProcessInfo(processElement));
                    }
                }
                BatteryInfo battery = null;
                if (batteryElement!=null)
                {
                     battery = GetBatteryInfo(batteryElement);
                }
                WindowInfo foreground = GetWindowInfo(foregroundWindowElement);
                DataInfo history = new DataInfo(time, pros, wins, battery, foreground);
                infos.Add(history);

                current++;

            }
            return infos;
        }
        public List<EventInfo> ReadEventHistory(int first, int last)
        {
            List<EventInfo> infos = new List<EventInfo>();
            int current = first;
            while (current <= last)
            {

                if (current >= Count)
                {
                    break;
                }

                XmlElement element = eventElements[current];

                EventInfo info = new EventInfo(element.GetAttribute("Type"), DateTime.Parse(element.GetAttribute("Time")));
                infos.Add(info);

                current++;

            }
            return infos;
        }

        private BatteryInfo GetBatteryInfo(XmlElement element)
        {
            return new BatteryInfo(
              int.Parse(element.GetAttribute("Percent")),
              bool.Parse(element.GetAttribute("PowerOnline")));
        }

        private ProcessInfo GetProcessInfo(XmlElement element)
        {
            return new ProcessInfo(
                 long.Parse(element.GetAttribute("PhysicalMemory")),
                 element.GetAttribute("Window"),
                 int.Parse(element.GetAttribute("Id")),
                 long.Parse(element.GetAttribute("VirtualMemory")),
                 element.GetAttribute("Name"),
                 bool.Parse(element.GetAttribute("Responding")));
        }
        private WindowInfo GetWindowInfo(XmlElement element)
        {
            return new WindowInfo(
             (IntPtr)int.Parse(element.GetAttribute("Handle")),
             element.GetAttribute("Name"),
             element.GetAttribute("ClassName"));
        }


        public void Write(DataInfo info)
        {
            XmlElement element = xml.CreateElement("Data");
            element.SetAttribute("Time", info.time.ToString());
            if (info.battery != null)
            {
                element.AppendChild(GetBatteryXml(info.battery));
            }
            if (info.windows != null)
            {
                element.AppendChild(GetWindowXml(info.windows));
            }
            if (info.processes != null)
            {
                element.AppendChild(GetProcessXml(info.processes));
            }
            element.AppendChild(GetForegroundWindowXml(info.foregroundWindow));
            root.AppendChild(element);

            xml.Save(xmlPath);
        }

        private XmlElement GetBatteryXml(BatteryInfo battery)
        {
            XmlElement element = xml.CreateElement("Battery");
            element.SetAttribute("Percent", battery.Percent.ToString());
            element.SetAttribute("PowerOnline", battery.PowerOnline.ToString());
            return element;
        }


        private XmlElement GetWindowXml(WindowInfo[] wins)
        {
            XmlElement element = xml.CreateElement("Windows");
            foreach (var win in wins)
            {
                XmlElement child = xml.CreateElement("Window");
                child.SetAttribute("Handle", win.handle.ToString());
                child.SetAttribute("Name", win.name);
                child.SetAttribute("ClassName", win.className);
                element.AppendChild(child);
            }
            return element;
        }

        private XmlElement GetForegroundWindowXml(WindowInfo win)
        {
            XmlElement element = xml.CreateElement("ForegroundWindow");

            element.SetAttribute("Handle", win.handle.ToString());
            element.SetAttribute("Name", win.name);
            element.SetAttribute("ClassName", win.className);

            return element;
        }

        private XmlElement GetProcessXml(ProcessInfo[] processes)
        {
            XmlElement element = xml.CreateElement("Processes");
            foreach (var process in processes)
            {
                XmlElement child = xml.CreateElement("Process");
                child.SetAttribute("Id", process.id.ToString());
                child.SetAttribute("Name", process.name);
                child.SetAttribute("PhysicalMemory", process.physicalMemory.ToString());
                child.SetAttribute("VirtualMemory", process.virtualMemory.ToString());
                child.SetAttribute("Window", process.window);
                child.SetAttribute("Responding", process.responding.ToString());

                element.AppendChild(child);
            }
            return element;
        }
    }
}

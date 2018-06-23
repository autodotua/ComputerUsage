using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static ComputerUsage.GlobalDatas;

namespace ComputerUsage
{
    public class XmlHelper
    {
        XmlDocument xml;
        XmlElement root;

        string xmlPath;

        public XmlHelper()
        {
            xmlPath = ConfigDirectory + "\\history.xml";

            reLoad:
            xml = new XmlDocument();
            if (!File.Exists(xmlPath))
            {
                Directory.CreateDirectory(ConfigDirectory);
                XmlDeclaration xdec = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
                xml.AppendChild(xdec);
                root = xml.CreateElement("ComputerUsage");
                xml.AppendChild(root);
               Save();
            }
            else
            {
                try
                {
                    xml.Load(xmlPath);
                }
                catch (Exception ex)
                {
                    int result = WpfControls.Dialog.DialogHelper.ShowMessage("加载XML失败：" + ex.Message + Environment.NewLine + "请选择操作",
                        WpfControls.Dialog.DialogType.Error, new string[] { "打开目录手动恢复并退出", "尝试恢复上一次的备份", "清空历史文件", "退出" });
                    if (result == 0)
                    {
                        Process.Start(ConfigDirectory);
                        App.Current.Shutdown();
                    }
                    else if (result == 1)
                    {
                        string directory = ConfigDirectory + "\\HistoryBackup";
                        if (!Directory.Exists(directory))
                        {
                            WpfControls.Dialog.DialogHelper.ShowError("备份文件不存在！");
                            App.Current.Shutdown();

                        }
                        else
                        {
                            List<FileInfo> existingFile = Directory.EnumerateFiles(directory).Select(p => new FileInfo(p)).Where(p => p.Name.StartsWith("Backup") && p.Extension == ".xml").OrderByDescending(p => p.CreationTime).ToList();
                            if (existingFile.Count == 0)
                            {
                                WpfControls.Dialog.DialogHelper.ShowError("备份文件不存在！");
                                App.Current.Shutdown();
                            }


                            if (File.Exists(xmlPath))
                            {
                                File.Delete(xmlPath);
                            }
                            try
                            {
                                existingFile[0].CopyTo(xmlPath);
                            }
                            catch (Exception ex2)
                            {
                                WpfControls.Dialog.DialogHelper.ShowException("复制备份文件出错", ex2);
                                App.Current.Shutdown();
                            }
                            WpfControls.Dialog.DialogHelper.ShowPrompt("已尝试恢复历史文件，将重试");
                            goto reLoad;
                        }

                        //if (!File.Exists(ConfigDirectory+"\\history.bak"))
                        //{
                        //    WpfControls.Dialog.DialogHelper.ShowError("备份文件不存在！");
                        //    App.Current.Shutdown();
                        //}
                        //else
                        //{
                        //    if(File.Exists(xmlPath))
                        //    {
                        //        File.Delete(xmlPath);
                        //    }
                        //    File.Move(ConfigDirectory + "\\history.bak",xmlPath);
                        //    File.Delete(ConfigDirectory + "\\history.bak");
                        //    WpfControls.Dialog.DialogHelper.ShowPrompt("已尝试恢复历史文件，将重试");
                        //    goto reLoad;
                        //}
                    }
                    else if (result == 2)
                    {
                        File.Delete(xmlPath);
                        goto reLoad;
                    }
                    else
                    {
                        App.Current.Shutdown();
                    }

                }
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
           Save();
        }

        public void WriteStart()
        {
            XmlElement element = xml.CreateElement("Event");
            element.SetAttribute("Time", DateTime.Now.ToString());
            if ((App.Current as App).startup)
            {
                element.SetAttribute("Type", "程序启动（开机自启）");
            }
            else
            {
                element.SetAttribute("Type", "程序启动");
            }
            root.AppendChild(element);
           Save();
        }

        public void Write(string @event)
        {
            XmlElement element = xml.CreateElement("Event");
            element.SetAttribute("Time", DateTime.Now.ToString());
            element.SetAttribute("Type", @event);
            LastEvent = @event;
            root.AppendChild(element);
           Save();
        }
        public string LastEvent { get; set; }
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
            while (current <= last)
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
                XmlElement networkElement = null;
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
                        case "NetworkStatus":
                            networkElement = child;
                            break;
                    }
                }

                //if (batteryElement == null || processElements == null || winElements == null || foregroundWindowElement == null)
                //{
                //    throw new Exception("XML文档被篡改");
                //}
                DateTime time = DateTime.Parse(element.GetAttribute("Time"));
                List<WindowInfo> wins = null;
                if (winElements != null)
                {
                    wins = new List<WindowInfo>();
                    foreach (XmlElement winElement in winElements.ChildNodes)
                    {

                        wins.Add(GetWindowInfo(winElement));
                    }
                }
                List<ProcessInfo> pros = null;
                if (processElements != null)
                {
                    pros = new List<ProcessInfo>();
                    foreach (XmlElement processElement in processElements.ChildNodes)
                    {

                        pros.Add(GetProcessInfo(processElement));
                    }
                }
                BatteryInfo battery = null;
                if (batteryElement != null)
                {
                    battery = GetBatteryInfo(batteryElement);
                }
                WindowInfo foreground = GetWindowInfo(foregroundWindowElement);
                bool mouseMoved = false;
                if (element.HasAttribute("MouseMoved"))
                {
                    mouseMoved = bool.Parse(element.GetAttribute("MouseMoved"));
                }
                //ComputerDatas.NetworkStatus net = ComputerDatas.NetworkStatus.Unknow;
                List<PingInfo> pings = new List<PingInfo>();
                if (networkElement != null)
                {
                    foreach (XmlElement child in networkElement)
                    {
                        pings.Add(new PingInfo(child.GetAttribute("Address"), int.Parse(child.GetAttribute("Time")), child.HasAttribute("Result") ? ((IPStatus)Enum.Parse(typeof(IPStatus), child.GetAttribute("Result"))) : IPStatus.Unknown));
                    }
                    //    switch(networkElement.GetAttribute("NetworkStatus"))
                    //    {
                    //        case "完全连接":
                    //            net = ComputerDatas.NetworkStatus.All;
                    //            break;
                    //        case "部分连接":
                    //            net = ComputerDatas.NetworkStatus.Some;
                    //            break;
                    //        case "无连接":
                    //            net = ComputerDatas.NetworkStatus.None;
                    //            break;
                    //    }
                }
                DataInfo history = new DataInfo(time, pros, wins, battery, foreground, mouseMoved, pings);
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

        public static BatteryInfo GetBatteryInfo(XmlElement element)
        {
            return new BatteryInfo(
              int.Parse(element.GetAttribute("Percent")),
              bool.Parse(element.GetAttribute("PowerOnline")));
        }

        public static ProcessInfo GetProcessInfo(XmlElement element)
        {
            return new ProcessInfo(
                 long.Parse(element.GetAttribute("PhysicalMemory")),
                 element.GetAttribute("Window"),
                 int.Parse(element.GetAttribute("Id")),
                 long.Parse(element.GetAttribute("VirtualMemory")),
                 element.GetAttribute("Name"),
                 bool.Parse(element.GetAttribute("Responding")),
                element.GetAttribute("MainModuleFileName") ?? "");
        }
        public static WindowInfo GetWindowInfo(XmlElement element)
        {
            XmlElement processElement = element.ChildNodes.Cast<XmlElement>().FirstOrDefault(p => p.Name == "Process");
            ProcessInfo process = null;
            if (processElement != null)
            {
                process = GetProcessInfo(processElement);
            }
            return new WindowInfo(
             (IntPtr)int.Parse(element.GetAttribute("Handle")),
             element.GetAttribute("Name"),
             element.GetAttribute("ClassName"),
             process);
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
            if (info.pingInfos != null)
            {
                element.AppendChild(GetNetworkXml(info.pingInfos));
            }
            element.AppendChild(GetWindowXml(info.foregroundWindow));
            element.SetAttribute("MouseMoved", info.mouseMoved.ToString());
            // element.SetAttribute("NetworkStatus",info.DisplayNetwork);
            root.AppendChild(element);


            //if(File.Exists(xmlPath))
            //{
            //    if(File.Exists(ConfigDirectory + "\\history.bak"))
            //    {
            //        File.Delete(ConfigDirectory + "\\history.bak");
            //    }
            //    File.Copy(xmlPath, ConfigDirectory + "\\history.bak");
            //}
            Save();
        }

        int saveFailedTimes = 0;
        private void Save()
        {
            try
            {
                xml.Save(xmlPath);
                saveFailedTimes = 0;
            }
            catch(Exception ex)
            {
                saveFailedTimes++;
                File.AppendAllText("Exception.log", Environment.NewLine + Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine +"文件保存失败"+Environment.NewLine+ ex.ToString());
                if(saveFailedTimes>5)
                {
                    App.Current.Dispatcher.Invoke(() => WpfControls.Dialog.DialogHelper.ShowException("历史文件保存已连续失败五次，请检查错误日志！", ex));
                }
            }
        }
        private XmlElement GetBatteryXml(BatteryInfo battery)
        {
            XmlElement element = xml.CreateElement("Battery");
            element.SetAttribute("Percent", battery.Percent.ToString());
            element.SetAttribute("PowerOnline", battery.PowerOnline.ToString());
            return element;
        }

        private XmlElement GetWindowXml(IEnumerable<WindowInfo> wins)
        {
            XmlElement element = xml.CreateElement("Windows");
            foreach (var win in wins)
            {
                XmlElement child = xml.CreateElement("Window");
                child.SetAttribute("Handle", win.handle.ToString());
                child.SetAttribute("Name", win.name);
                child.SetAttribute("ClassName", win.className);
                XmlElement processElement = GetProcessXml(win.process);
                child.AppendChild(processElement);
                element.AppendChild(child);
            }
            return element;
        }

        private XmlElement GetWindowXml(WindowInfo win)
        {
            XmlElement element = xml.CreateElement("ForegroundWindow");

            element.SetAttribute("Handle", win.handle.ToString());
            element.SetAttribute("Name", win.name);
            element.SetAttribute("ClassName", win.className);
            XmlElement processElement = GetProcessXml(win.process);
            element.AppendChild(processElement);
            return element;
        }
        private XmlElement GetNetworkXml(IEnumerable<PingInfo> pings)
        {
            XmlElement element = xml.CreateElement("NetworkStatus");

            try
            {
                foreach (var ping in pings)
                {
                    if (ping == null)
                    {
                        File.AppendAllText("Exception.log", Environment.NewLine + Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine + "ping is null, pings.count=" + pings.Count());
                        continue;
                    }
                    XmlElement child = xml.CreateElement("Ping");
                    child.SetAttribute("Address", ping.Address);
                    child.SetAttribute("Time", ping.time.ToString());
                    child.SetAttribute("Result", ping.result.ToString());

                    element.AppendChild(child);
                }

            }
            catch(Exception ex)
            {
                File.AppendAllText("Exception.log", Environment.NewLine + Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine + "network failed" +Environment.NewLine+ ex.ToString());
            }

            return element;
        }
        private XmlElement GetProcessXml(ProcessInfo[] processes)
        {
            XmlElement element = xml.CreateElement("Processes");
            foreach (var process in processes)
            {
                //XmlElement child = xml.CreateElement("Process");
                //child.SetAttribute("Id", process.id.ToString());
                //child.SetAttribute("Name", process.name);
                //child.SetAttribute("PhysicalMemory", process.physicalMemory.ToString());
                //child.SetAttribute("VirtualMemory", process.virtualMemory.ToString());
                //child.SetAttribute("Window", process.window);
                //child.SetAttribute("Responding", process.responding.ToString());

                element.AppendChild(GetProcessXml(process));
            }
            return element;
        }

        private XmlElement GetProcessXml(ProcessInfo process)
        {

            XmlElement element = xml.CreateElement("Process");
            element.SetAttribute("Id", process.id.ToString());
            element.SetAttribute("Name", process.name);
            element.SetAttribute("PhysicalMemory", process.physicalMemory.ToString());
            element.SetAttribute("VirtualMemory", process.virtualMemory.ToString());
            element.SetAttribute("Window", process.window);
            element.SetAttribute("Responding", process.responding.ToString());
            element.SetAttribute("MainModuleFileName", process.mainModuleFileName);


            return element;
        }

    }
}

//#define DEBUG

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static ComputerUsage.GlobalDatas;

namespace ComputerUsage
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        WpfCodes.Program.TrayIcon trayIcon;
        public BackgroundWork background;
        public bool startup = false;
        public App()
        {

#if !DEBUG
            if (WpfCodes.Program.Startup.HaveAnotherInstance("ComputerUsage"))
            {
                WpfControls.Dialog.DialogHelper.ShowError("已存在另一实例，请不要重复运行！");
                Environment.Exit(0);
                return;
            }

            //TaskScheduler.UnobservedTaskException += (p1, p2) => { if (!p2.Observed) ShowException(p2.Exception,3); };//Task
            //AppDomain.CurrentDomain.UnhandledException += (p1, p2) => ShowException((Exception)p2.ExceptionObject,2);//UI
            //DispatcherUnhandledException += (p1, p2) => ShowException(p2.Exception,1);//Thread
            new WpfCodes.Program.Exception().UnhandledException += (p1, p2) =>
          {
              try
              {
                  Dispatcher.Invoke(() => WpfControls.Dialog.DialogHelper.ShowException("程序发生了未捕获的错误，类型" +p2.Source.ToString(), p2.Exception));

                  File.AppendAllText("Exception.log", Environment.NewLine + Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine + p2.Exception.ToString());
              }
              catch (Exception ex)
              {
                  Dispatcher.Invoke(() => WpfControls.Dialog.DialogHelper.ShowException("错误信息无法写入", ex));
              }
              finally
              {
                  App.Current.Shutdown();
              }
          };
#endif

        }




        //private void ShowException(Exception ex,int type)
        //{
        //    try
        //    {
        //        Dispatcher.Invoke(() => WpfControls.Dialog.DialogHelper.ShowException("程序发生了未捕获的错误，类型"+type.ToString(), ex));

        //        File.AppendAllText("Exception.log", Environment.NewLine + Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine + ex.ToString());
        //    }
        //    catch (Exception ex2)
        //    {
        //        Dispatcher.Invoke(() => WpfControls.Dialog.DialogHelper.ShowException("错误信息无法写入", ex2));
        //    }
        //    finally
        //    {
        //        App.Current.Shutdown();
        //    }
        //}

        private void ApplicationStartupEventHandler(object sender, StartupEventArgs e)
        {
            ConcernConfigPath();
            Load();
            if (e.Args.Length > 0 && e.Args[0] == "noWindow")
            {
                startup = true;
            }
            //ComputerDatas.GetNetworkStatus();
            background = new BackgroundWork();
            //await background.TimerTickEventHandler();
            void newWindow()
            {
                if (Current.MainWindow as MainWindow == null)
                {
                    Current.MainWindow = new MainWindow();
                    Current.MainWindow.Show();
                }
            };
            Dictionary<string, Action> rightMouseClick = new Dictionary<string, Action>
            {
                { "回收",() => GC.Collect() },

                {"退出" ,()=>Shutdown() },
            };
            trayIcon = new WpfCodes.Program.TrayIcon(ComputerUsage.Properties.Resources.ICON, "计算机使用情况", newWindow, rightMouseClick);
            trayIcon.Show();
            if (!startup)
            {
                newWindow();
            }
            ComputerDatas.RegistSystemEvents();
            ClipboardHelper clipBoardHelper = new ClipboardHelper();
        }

        private void ConcernConfigPath()
        {
            if (File.Exists("config.ini"))
            {
                string value = File.ReadAllText("config.ini");
                if (value == "here")
                {
                    value = AppDomain.CurrentDomain.BaseDirectory + "Config";
                }

                ConfigDirectory = value;

                if (!Directory.Exists(value))

                {
                    try
                    {
                        Directory.CreateDirectory(value);
                        ConfigDirectory = value;
                    }
                    catch
                    {
                        ConfigDirectory = "";
                    }
                }
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            trayIcon.Dispose();
        }


    }
}

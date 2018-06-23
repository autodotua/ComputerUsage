﻿//#define DEBUG

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
using WpfControls.ButtonBase;
using static ComputerUsage.GlobalDatas;

namespace ComputerUsage
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        TrayIcon trayIcon;
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

            TaskScheduler.UnobservedTaskException += (p1, p2) => { if (!p2.Observed) ShowException(p2.Exception); };//Task
            AppDomain.CurrentDomain.UnhandledException += (p1, p2) => ShowException((Exception)p2.ExceptionObject);//UI
            DispatcherUnhandledException += (p1, p2) => ShowException(p2.Exception);//Thread
#endif

        }




        private void ShowException(Exception ex)
        {
            try
            {
                Dispatcher.Invoke(() => WpfControls.Dialog.DialogHelper.ShowException("程序发生了未捕获的错误", ex));

                File.AppendAllText("Exception.log", Environment.NewLine + Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine + ex.ToString());
            }
            catch (Exception ex2)
            {
                Dispatcher.Invoke(() => WpfControls.Dialog.DialogHelper.ShowException("错误信息无法写入", ex2));
            }
            finally
            {
                App.Current.Shutdown();
            }
        }

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
                if (Current.MainWindow == null)
                {
                    Current.MainWindow = new MainWindow();
                    Current.MainWindow.Show();
                }
            };
            Dictionary<string, Action> rightMouseClick = new Dictionary<string, Action>
            {
                {"退出" ,()=>Shutdown() }
            };
            trayIcon = new TrayIcon(ComputerUsage.Properties.Resources.ICON, "计算机使用情况", newWindow, rightMouseClick);
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

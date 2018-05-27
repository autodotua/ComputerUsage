using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfControls.ButtonBase;

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
          

            if (WpfCodes.Program.Startup.HaveAnotherInstance("ComputerUsage"))
            {
                WpfControls.Dialog.DialogHelper.ShowError("已存在另一实例，请不要重复运行！");
                Environment.Exit(0);
                return;
            }

        }

        private  void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0 && e.Args[0] == "noWindow")
            {
                startup = true;
            }
         

            GlobalDatas.LoadSettings();
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
            if(!startup)
            {
                newWindow();
            }
            ComputerDatas.RegistSystemEvents();
        }

        

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            trayIcon.Dispose();
        }


    }
}

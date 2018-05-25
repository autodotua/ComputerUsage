using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
        private  void Application_Startup(object sender, StartupEventArgs e)
        {
            GlobalDatas.LoadSettings();
            background = new BackgroundWork();
            //await background.TimerTickEventHandler();
            void click()
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
            trayIcon = new TrayIcon(ComputerUsage.Properties.Resources.ICON, "计算机使用情况", click, rightMouseClick);
            trayIcon.Show();


            if(!(e.Args.Length>0 && e.Args[0] == "noWindow"))
            {
                click();
            }


        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            trayIcon.Dispose();
        }


    }
}

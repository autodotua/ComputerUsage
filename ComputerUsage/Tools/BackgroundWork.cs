using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ComputerUsage.GlobalDatas;

namespace ComputerUsage
{
   public class BackgroundWork
    {
        Timer mainTimer;

        public BackgroundWork()
        {
            mainTimer = new Timer(new TimerCallback(/*async*/ p =>/*await*/ TimerTickEventHandler()),this,0,Set.TimerInterval*1000);
        }

        public /*async*/ void TimerTickEventHandler()
        {
            DataInfo info = null;
            //await Task.Run(() =>
            //{
                info = new DataInfo();
                xml.Write(info);
            //});
            //App.Current.Dispatcher.Invoke(() =>
            //{
            //    if (App.Current.MainWindow != null)
            //    {
            //        (App.Current.MainWindow as MainWindow).ucHistoryList.AddToList(info);
            //        //App.Current.MainWindow.. AddToList(info);
            //    }
            //});
        }

    }
}

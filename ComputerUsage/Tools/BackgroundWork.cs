using System;
using System.Collections.Generic;
using System.IO;
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
        private DateTime lastBackupTime = DateTime.MinValue;
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

            if((DateTime.Now-lastBackupTime).TotalMinutes>Set.BackupInterval)
            {
                lastBackupTime = DateTime.Now;
                Backup();
            }
        }



        public void Backup()
        {
            string directory = ConfigDirectory + "\\HistoryBackup";
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            List<FileInfo> existingFile = Directory.EnumerateFiles(directory).Select(p => new FileInfo(p)).Where(p => p.Name.StartsWith("Backup") && p.Extension == ".xml").OrderByDescending(p=>p.CreationTime).ToList();
            if(existingFile.Count>=Set.BackupCount)
            {
               for(int i=Set.BackupCount-1;i<existingFile.Count;i++)
                {
                    existingFile[i].Delete();
                }
            }

            File.Copy(ConfigDirectory + "\\History.xml", directory + "\\Backup_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xml");
        }
    }
}

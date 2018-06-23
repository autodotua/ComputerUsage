using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using static ComputerUsage.GlobalDatas;

namespace ComputerUsage
{
    public class ClipboardHelper
    {
        public ClipboardHelper()
        {
            WpfCodes.WindowsApi.Clipboard.ClipboardChanged += ClipboardChangedEventHandler;
        }

        private static DateTime lastTime = DateTime.MinValue;

        public static string ClipboardHistoryPath => ConfigDirectory + "\\ClipboardHistory";
        private async void ClipboardChangedEventHandler(object sender, WpfCodes.WindowsApi.ClipboardEventArgs e)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            Task task = Task.Factory.StartNew(() =>
            {
                //Debug.WriteLine(a++);
                //Stopwatch stop = new Stopwatch();
                ///=stop.Start();
                DateTime now = DateTime.Now;

                if((now-lastTime).Milliseconds<Set.ClipboardMinInterval)
                {
                    return;
                }
                lastTime = now;
              string path= ClipboardHistoryPath+"\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "\\";
                Directory.CreateDirectory(path);

                string[] ContainedDataFormats= null;
                string text = null;
                string html = null;
                Bitmap image = null;
                string rtf = null;
                string csv = null;
                string[] files = null;
                App.Current.Dispatcher.Invoke(() =>
                {
                    ContainedDataFormats = e.ContainedDataFormats;
                    text = e.Text;
                    html = e.Html;
                    image = e.Image;
                    rtf = e.Rtf;
                    csv = e.Csv;
                    files = e.Files;
                });

                StringBuilder formats = new StringBuilder();
                foreach (var line in ContainedDataFormats)
                {
                    formats.AppendLine(line);
                }
                File.WriteAllText(path + "ContainedDataFormats.txt", formats.ToString());

                if (text != null)
                {
                    File.WriteAllText(path + "Text.txt", text);
                }


                if (html != null)
                {
                    File.WriteAllText(path + "Html.html", html);
                }


                if (image != null)
                {
                    image.Save(path + "Image." + Set.ImageFormat, GetImageFormat());
                }

                if (rtf != null)
                {
                    File.WriteAllText(path + "Rtf.rtf", rtf);
                }

                if (csv != null)
                {
                    File.WriteAllText(path + "Csv.csv", csv);
                }

                if (files != null)
                {
                    StringBuilder fileNames = new StringBuilder();
                    foreach (var line in files)
                    {
                        fileNames.AppendLine(line);
                    }
                File.WriteAllText(path + "Files.txt", fileNames.ToString());
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    if (App.Current.MainWindow != null)
                    {
                        if ((App.Current.MainWindow as MainWindow).tabClipboard.IsSelected)
                        {
                            (App.Current.MainWindow as MainWindow).ucClipboard.Load();
                        }
                    }
                });
                    //stop.Stop();
                    //Debug.WriteLine(stop.ElapsedMilliseconds.ToString());
                }, token);
            await Task.Delay(Set.ClipboradTimeOut);
            if (task.Status.HasFlag(TaskStatus.Running))
            {
                tokenSource.Cancel();
                Debug.WriteLine("超时！");
            }

        }

        private static ImageFormat GetImageFormat()
        {
            switch (Set.ImageFormat)
            {
                case "png":
                    return ImageFormat.Png;
                case "jpg":
                    return ImageFormat.Jpeg;
                case "bmp":
                    return ImageFormat.Bmp;
            }
            return null;
        }
    }
}

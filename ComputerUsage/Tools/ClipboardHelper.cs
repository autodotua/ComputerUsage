using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
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
            //var tokenSource = new CancellationTokenSource();
            //var token = tokenSource.Token;
            //await Task.Run(() =>
            //{
            //Debug.WriteLine(a++);
            //Stopwatch stop = new Stopwatch();
            ///=stop.Start();
            ///
            string currentStep ="开始";
            DateTime now = DateTime.Now;

            if ((now - lastTime).Milliseconds < Set.ClipboardMinInterval)
            {
                return;
            }
            lastTime = now;
            string path = ClipboardHistoryPath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "\\";
            Directory.CreateDirectory(path);
            currentStep = "变量声明";
            string[] ContainedDataFormats = null;
            string text = null;
            string html = null;
            Bitmap image = null;
            string rtf = null;
            string csv = null;
            string[] files = null;

            //Exception exception = null;

            //App.Current.Dispatcher.Invoke(() =>
            //{
            
            try
            {
                currentStep = "获取内容0";
                ContainedDataFormats = e.ContainedDataFormats;

                currentStep = "获取内容1";
                text = e.Text;

                currentStep = "获取内容2";
                html = e.Html;

                currentStep = "获取内容3";
                image = e.Image;

                currentStep = "获取内容4";
                rtf = e.Rtf;

                currentStep = "获取内容5";
                csv = e.Csv;

                currentStep = "获取内容6";
                files = e.Files;
            }
            catch (Exception ex)
            {
                File.WriteAllText(path + "Exception.txt",ex.ToString()+Environment.NewLine+currentStep);

                File.AppendAllText("Exception.log", Environment.NewLine + Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine + "剪贴板失败" + Environment.NewLine + ex.ToString());
                return;
            }
            //});

            //if (exception != null)
            //{

            //}
            try
            {
                currentStep = "写入内容0";
                if (ContainedDataFormats == null || ContainedDataFormats.Length == 0)
                {
                    return;
                }
                StringBuilder formats = new StringBuilder();
                foreach (var line in ContainedDataFormats)
                {
                    formats.AppendLine(line);
                }
                File.WriteAllText(path + "ContainedDataFormats.txt", formats.ToString());

                currentStep = "写入内容1";
                if (text != null)
                {
                    File.WriteAllText(path + "Text.txt", text);
                }

                currentStep = "写入内容2";
                if (html != null)
                {
                    File.WriteAllText(path + "Html.html", html);
                }

                currentStep = "写入内容3";
                if (image != null)
                {
                    image.Save(path + "Image." + Set.ImageFormat, GetImageFormat());
                }
                currentStep = "写入内容4";
                if (rtf != null)
                {
                    File.WriteAllText(path + "Rtf.rtf", rtf);
                }
                currentStep = "写入内容5";
                if (csv != null)
                {
                    File.WriteAllText(path + "Csv.csv", csv);
                }
                currentStep = "写入内容6";
                if (files != null)
                {
                    StringBuilder fileNames = new StringBuilder();
                    foreach (var line in files)
                    {
                        fileNames.AppendLine(line);
                    }
                    File.WriteAllText(path + "Files.txt", fileNames.ToString());
                }
                currentStep = "刷新";
                try
                {
                    if (App.Current!=null && App.Current.MainWindow != null)
                    {
                        if ((App.Current.MainWindow as MainWindow).tabClipboard.IsSelected)
                        {
                            (App.Current.MainWindow as MainWindow).ucClipboard.Load();
                        }
                    }
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(path + "Exception.txt", ex.ToString() + Environment.NewLine + currentStep);

                File.AppendAllText("Exception.log", Environment.NewLine + Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine + "剪贴板写入失败" + Environment.NewLine + ex.ToString());

            }
            ContainedDataFormats = null;
            text = null;
            html = null;
            image = null;
            rtf = null;
            csv = null;
            files = null;
            GC.Collect();



            //});

            //stop.Stop();
            //Debug.WriteLine(stop.ElapsedMilliseconds.ToString());
            //}, token);
            //await Task.Delay(Set.ClipboradTimeOut);
            //if (task.Status.HasFlag(TaskStatus.Running))
            //{
            //    tokenSource.Cancel();
            //    Debug.WriteLine("超时！");
            //}

            foreach (var directoryName in Directory.EnumerateDirectories(ClipboardHistoryPath).Select(p=>new DirectoryInfo(p).Name))
            {
                if (!DateTime.TryParseExact(directoryName, "yyyyMMddHHmmssfff", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime result))
                {
                    continue;
                }
                if (result.Date != DateTime.Today)
                {
                  await  ZipOldFiles();
                    xml.Write("打包了旧的剪贴板数据");
                    return;
                }
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

        public async static Task ZipOldFiles()
        {
            await Task.Run(() =>
            {
                Dictionary<DateTime, List<DirectoryInfo>> directoryClassesByDate = new Dictionary<DateTime, List<DirectoryInfo>>();
                var dateDirectories = new List<DirectoryInfo>();

                try
                {
                    foreach (var directory in Directory.EnumerateDirectories(ClipboardHistoryPath).Select(p => new DirectoryInfo(p)))
                    {
                        if (!DateTime.TryParseExact(directory.Name, "yyyyMMddHHmmssfff", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime result))
                        {
                            continue;
                        }
                        if (result.Date == DateTime.Today)
                        {
                            continue;
                        }
                        if (directoryClassesByDate.ContainsKey(result.Date))
                        {
                            directoryClassesByDate[result.Date].Add(directory);
                        }
                        else
                        {
                            directoryClassesByDate.Add(result.Date, new List<DirectoryInfo>() { directory });
                        }
                    }
                    foreach (var directorys in directoryClassesByDate)
                    {
                        DirectoryInfo dateDirectory = Directory.CreateDirectory(ClipboardHistoryPath + "\\" + directorys.Key.ToString("yyyy-MM-dd"));
                        dateDirectories.Add(dateDirectory);
                        foreach (var directory in directorys.Value)
                        {
                            directory.MoveTo(dateDirectory.FullName + "\\" + directory.Name);
                        }
                        ZipFile.CreateFromDirectory(dateDirectory.FullName, ClipboardHistoryPath + "\\" + directorys.Key.ToString("yyyy-MM-dd") + ".zip");

                    }
                }
                catch(Exception  ex)
                {
                    App.Current.Dispatcher.Invoke(() => WpfControls.Dialog.DialogHelper.ShowException("打包失败", ex));
                    return;
                }

                try
                {
                    //foreach (var directories in directoryClassesByDate)
                    //{
                    //    foreach (var directory in directories.Value)
                    //    {
                    //        directory.Delete();
                    //    }
                    //}

                    foreach (var directory in dateDirectories)
                    {
                        directory.Delete(true);
                    }
                }
                catch(Exception ex)
                {
                    App.Current.Dispatcher.Invoke(() => WpfControls.Dialog.DialogHelper.ShowException("打包成功，但删除原文件夹失败", ex));
 

                }
                //foreach (var directorys in directoryClassesByDate)
                //{
                //    using (FileStream fs = File.Create(ClipboardHistoryPath + "\\" + directorys.Key.ToString("yyyy-MM-dd") + ".zip"))
                //    {
                //        using (ZipOutputStream zs = new ZipOutputStream(fs))
                //        {

                //        }
                //    }
                //}
            });


        }
    }
}

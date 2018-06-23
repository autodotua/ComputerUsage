using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerUsage
{
    public class ClipboardInfo
    {

        DateTime time;
        public string Path { get; private set; }

        public ClipboardInfo(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException();
            }

            string timeString = new DirectoryInfo(path).Name;
            if (!DateTime.TryParseExact(timeString, "yyyyMMddHHmmssfff", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime result))
            {
                throw new Exception("指定的目录文件名格式不正确");
            }
            time = result;
            Path = path;

            FileInfo[] files = Directory.EnumerateFiles(path).Select(p => new FileInfo(p)).ToArray();
            if (files.Length == 0)
            {
                throw new Exception("空的文件夹");
            }

            if (files.Any(p => p.Name.Contains("Image.")))
            {
                overview="[图片]";
            }
            else if (files.Any(p => p.Name == "Rtf.rtf"))
            {
                overview = "[富文本]";
            }
            else if (files.Any(p => p.Name == "Html.html"))
            {
                overview= "[网页]";
            }
            else if (files.Any(p => p.Name == "Text.txt"))
            {
                FileInfo file = files.First(p => p.Name == "Text.txt");
                char[] buffer = new char[20];
                var reader = file.OpenText();
                int count = reader.Read(buffer, 0, 20);
                overview=new string(buffer, 0, count).Replace(Environment.NewLine, "") + "…";
            }
        }

        public string DisplayTime => time.ToString();

        private string overview = "";
        public string DisplayOverview => overview;
    }
}

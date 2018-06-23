using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerUsage
{
    public class ClipboardDetailInfo
    {
        public ClipboardDetailInfo(FileInfo file)
        {
            this.file = file;
        }
        public FileInfo file;
        public FileInfo FileInformation => file;
        public string Name => file.Name;
        public string Path { get; set; }
        public string Type
        {
            get
            {
                if(file.Name.StartsWith("Image."))
                {
                    return "图片";
                }
                switch(file.Name)
                {
                    case "Text.txt":
                        return "文本";
                    case "Html.html":
                        return "网页";
                    case "Rtf.rtf":
                        return "富文本";
                    case "Csv.csv":
                        return "表格";
                    case "Files.txt":
                        return "文件";
                    case "ContainedDataFormats.txt":
                        return "所有类型";
                    default:
                        return "未知";
                }
            }
        }
        public string Size
        {
            get
            {
                if(file.Name== "ContainedDataFormats.txt")
                {
                    return "";
                }
                if (!file.Exists)
                {
                    return "文件不存在";
                }

                return WpfCodes.Basic.Number.ByteToFitString(file.Length);
            }
        }
    }
}

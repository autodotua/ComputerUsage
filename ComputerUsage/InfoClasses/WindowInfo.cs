using System;
using System.Diagnostics;
using System.Windows;

namespace ComputerUsage
{
    [DebuggerDisplay("{handle.ToInt32()},{name}")]
    public class WindowInfo
    {
        public IntPtr handle;
        public string name;
        public string className;
        public ProcessInfo process;

        public string DisplayHandle => handle.ToString();
        public string DisplayName => name;
        public string DisplayClassName => className;
        public Visibility NeedProcessButtonVisibal => process == null ? Visibility.Collapsed : Visibility.Visible;
        public string DisplayProcess
        {
            get
            {
                if(process!=null)
                {
                    return process.DisplayName;
                }
                else
                {
                    return "";
                }

            }
        }

        //public WindowInfo()
        //{

        //}
        public WindowInfo(IntPtr handle, string name, string className, ProcessInfo process)
        {
            this.handle = handle;
            this.name = name;
            this.className = className;
            this.process = process;
        }
    }
}

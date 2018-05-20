using System;
using System.Diagnostics;

namespace ComputerUsage
{
    [DebuggerDisplay("{handle.ToInt32()},{name}")]
    public class WindowInfo
    {
        public IntPtr handle;
        public string name;
        public string className;

        public string DisplayHandle => handle.ToString();
        public string DisplayName => name;
        public string DisplayClassName => className;

        public WindowInfo()
        {

        }
        public WindowInfo(IntPtr handle, string name, string className)
        {
            this.handle = handle;
            this.name = name;
            this.className = className;
        }
    }
}

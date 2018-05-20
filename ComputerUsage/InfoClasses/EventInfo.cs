using System;

namespace ComputerUsage
{
    public class EventInfo
    {
        public DateTime time;

        public string DisplayTime => time.ToString();

        public string type;

        public EventInfo(string type)
        {
            this.type = type;
            time = DateTime.Now;
        }
        public EventInfo(string type, DateTime time)
        {
            this.type = type;
            this.time = time;
        }
        public string DisplayType => type;
    }
}

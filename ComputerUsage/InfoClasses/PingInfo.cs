using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerUsage
{
    public class PingInfo
    {
       public int time;

        private string address;



        public PingInfo(string address, int time)
        {
            this.address = address;
            this.time = time;
        }

        public string DisplayTime => time.ToString();

        public string Address { get => address; set => address = value; }
    }
}

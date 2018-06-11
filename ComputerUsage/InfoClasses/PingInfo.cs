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



        public PingInfo(string address, int time, System.Net.NetworkInformation.IPStatus result)
        {
            this.address = address;
            this.time = time;
            this.result = result;
        }

        public System.Net.NetworkInformation.IPStatus result;

        public string DisplayResult => result == System.Net.NetworkInformation.IPStatus.Success ? "成功" : "失败：" + result.ToString();

        public string DisplayTime => time.ToString();

        public string Address { get => address; set => address = value; }
    }
}

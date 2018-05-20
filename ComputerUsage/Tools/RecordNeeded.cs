using System.Text;
using System.Threading.Tasks;
using static ComputerUsage.GlobalDatas;

namespace ComputerUsage
{

    public static class RecordNeeded
    {
        private static bool windows = true;
        private static bool processes = true;
        private static bool battery = true;


        public static bool Windows { get => windows; set => windows = value; }
        public static bool Processes { get => processes; set => processes = value; }
        public static bool Battery { get => battery; set => battery = value; }
    }
}

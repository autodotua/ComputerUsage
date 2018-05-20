namespace ComputerUsage
{
    public class BatteryInfo
    {
        public BatteryInfo(System.Windows.Forms.PowerStatus power)
        {
            Percent = (int)(power.BatteryLifePercent * 100);
            PowerOnline = power.PowerLineStatus == System.Windows.Forms.PowerLineStatus.Online;
        }

        public BatteryInfo(int percent, bool powerOnline)
        {
            this.percent = percent;
            this.powerOnline = powerOnline;
        }

        private int percent;
        private bool powerOnline;

        public bool PowerOnline { get => powerOnline; set => powerOnline = value; }
        public int Percent { get => percent; set => percent = value; }
    }
}

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
            this.Percent = percent;
            this.PowerOnline = powerOnline;
        }

        public bool PowerOnline { get; set; }
        public int Percent { get; set; }
    }
}

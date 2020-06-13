namespace devmon_library.Models
{
    public sealed class CpuInfo
    {
        public string Name {get; set;}
        public int Cores { get; set; }
        public int Threads { get; set; }
        public int SpeedMhz { get; set; }
    }
}

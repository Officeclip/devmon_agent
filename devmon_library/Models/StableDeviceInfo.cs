namespace devmon_library.Models
{
    sealed class StableDeviceInfo
    {
        public CpuInfo Cpu { get; set; }
        public MemoryInfo Mem { get; set; }
        public NetworkInfo[] Net { get; set; }
        public DriveInfo[] Drives { get; set; }
        public OsInfo Os { get; set; }
        public SoftwareInfo[] Softwares { get; set; }
    }
}

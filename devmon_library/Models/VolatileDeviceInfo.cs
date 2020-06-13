namespace devmon_library.Models
{
    sealed class VolatileDeviceInfo
    {
        public CpuUtilization Cpu { get; set; }
        public MemoryUtilization Mem { get; set; }
        public NetworkUtilization[] Net { get; set; }
        public DriveUtilization[] Drives { get; set; }
        public OsUtilization Os { get; set; }
    }
}

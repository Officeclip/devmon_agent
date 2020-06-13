using Microsoft.VisualBasic.Devices;

namespace devmon_library.Models
{
    public sealed class MemoryInfo
    {
        public ulong TotalBytes { get; set; }

        public MemoryInfo(ComputerInfo computerInfo)
        {
            TotalBytes = computerInfo.TotalPhysicalMemory;
        }

        public MemoryInfo()
        {

        }
    }
}

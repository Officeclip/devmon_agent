using Microsoft.VisualBasic.Devices;

namespace Geheb.DevMon.Agent.Models
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

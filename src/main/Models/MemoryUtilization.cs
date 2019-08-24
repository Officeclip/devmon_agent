using Microsoft.VisualBasic.Devices;
namespace Geheb.DevMon.Agent.Models
{
    public sealed class MemoryUtilization
    {
        public ulong FreeBytes { get; set; }

        public MemoryUtilization(ComputerInfo computerInfo)
        {
            FreeBytes = computerInfo.AvailablePhysicalMemory;
        }

        public MemoryUtilization()
        {

        }
    }
}

using Microsoft.VisualBasic.Devices;
namespace devmon_library.Models
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

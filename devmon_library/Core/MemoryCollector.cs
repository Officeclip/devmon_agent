using devmon_library.Models;
using Microsoft.VisualBasic.Devices;
using System.Threading.Tasks;

namespace devmon_library.Core
{
    internal sealed class MemoryCollector : IMemoryCollector
    {
        private ICancellation _cancellation;

        public MemoryCollector(ICancellation cancellation)
        {
            _cancellation = cancellation;
        }

        public Task<MemoryInfo> ReadMemoryInfo()
        {
            var computerInfo = new ComputerInfo();
            return Task.FromResult(new MemoryInfo(computerInfo));
        }

        public Task<MemoryUtilization> ReadMemoryUtilization()
        {
            var computerInfo = new ComputerInfo();
            return Task.FromResult(new MemoryUtilization(computerInfo));
        }
    }
}

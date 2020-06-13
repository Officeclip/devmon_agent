using devmon_library.Models;
using System.Threading.Tasks;

namespace devmon_library.Core
{
    sealed class VolatileDeviceCollector
    {
        private readonly INetworkCollector _networkCollector;
        private readonly ICpuCollector _cpuCollector;
        private readonly IMemoryCollector _memoryCollector;
        private readonly IDriveCollector _driveCollector;
        private readonly IOsCollector _osCollector;

        public VolatileDeviceCollector(
            ICpuCollector cpuCollector,
            IMemoryCollector memoryCollector,
            INetworkCollector networkCollector,
            IDriveCollector driveCollector,
            IOsCollector osCollector)
        {
            _cpuCollector = cpuCollector;
            _memoryCollector = memoryCollector;
            _networkCollector = networkCollector;
            _driveCollector = driveCollector;
            _osCollector = osCollector;
        }
        public async Task<VolatileDeviceInfo> Read()
        {
            return new VolatileDeviceInfo
            {
                Cpu = await _cpuCollector.ReadCpuUtilization(),
                Mem = await _memoryCollector.ReadMemoryUtilization(),
                Net = await _networkCollector.ReadNetworkUtilization(),
                Drives = await _driveCollector.ReadDriveUtilization(),
                Os = await _osCollector.ReadOsUtilization()
            };
        }
    }
}

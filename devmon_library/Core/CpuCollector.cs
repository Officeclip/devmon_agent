using devmon_library.Models;
using System.Diagnostics;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace devmon_library.Core
{
    internal sealed class CpuCollector : ICpuCollector
    {
        private readonly ICancellation _cancellation;

        public CpuCollector(ICancellation cancellation)
        {
            _cancellation = cancellation;
        }
    
        public Task<CpuInfo> ReadCpuInfo()
        {
            var cpu = new CpuInfo();

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
            {
                foreach (ManagementObject o in searcher.Get())
                {
                    cpu.Name = o["Name"] as string;
                    cpu.Cores = System.Convert.ToInt32(o["NumberOfCores"]);
                    cpu.Threads = System.Convert.ToInt32(o["ThreadCount"]);
                    cpu.SpeedMhz = System.Convert.ToInt32(o["MaxClockSpeed"]);
                    break;
                }
            }

            return Task.FromResult(cpu);
        }

        public async Task<CpuUtilization> ReadCpuUtilization()
        {
            var cpu = new CpuUtilization();

            using (var cpuTime = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
            {
                int i = 0;
                while (i++ < 3) // needs multitple times to calcuate correct value
                {
                    cpu.LoadPercentage = cpuTime.NextValue();
                    if (_cancellation == null) { 
                        await Task.Delay(1000);
                    }
                    else
                    {
                        await Task.Delay(1000, _cancellation.Token);
                    }
                }
            }

            _cancellation?.Token.ThrowIfCancellationRequested();

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
            {
                foreach (ManagementObject o in searcher.Get())
                {
                    cpu.SpeedMhz = System.Convert.ToInt32(o["CurrentClockSpeed"]);
                    break;
                }
            }

            return cpu;
        }
    }
}

using devmon_library.Models;
using System;
using System.Diagnostics;
using System.Management;
using System.ServiceModel.Channels;
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

        public object TryGetProperty(
                            ManagementObject wmiObj, 
                            string propertyName)
        {
            object retval;
            try
            {
                retval = wmiObj.GetPropertyValue(propertyName);
            }
            catch (System.Management.ManagementException ex)
            {
                retval = null;
            }
            return retval;
        }
   
        public Task<CpuInfo> ReadCpuInfo()
        {
            var cpu = new CpuInfo();

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
            {
                foreach (ManagementObject managementObject in searcher.Get())
                {
                 
                    cpu.Name = managementObject["Name"] as string;
                    cpu.Cores = Convert.ToInt32(TryGetProperty(managementObject, "NumberOfCores"));
                    cpu.Threads = Convert.ToInt32(TryGetProperty(managementObject, "ThreadCount"));
                    cpu.SpeedMhz = Convert.ToInt32(TryGetProperty(managementObject, "MaxClockSpeed"));
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

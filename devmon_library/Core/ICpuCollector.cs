using devmon_library.Models;
using System.Threading.Tasks;

namespace devmon_library.Core
{
    public interface ICpuCollector
    {
        Task<CpuInfo> ReadCpuInfo();
        Task<CpuUtilization> ReadCpuUtilization();
    }
}

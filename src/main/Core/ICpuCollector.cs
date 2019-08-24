using Geheb.DevMon.Agent.Models;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Core
{
    public interface ICpuCollector
    {
        Task<CpuInfo> ReadCpuInfo();
        Task<CpuUtilization> ReadCpuUtilization();
    }
}

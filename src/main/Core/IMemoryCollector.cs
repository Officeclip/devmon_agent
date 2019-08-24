using Geheb.DevMon.Agent.Models;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Core
{
    public interface IMemoryCollector
    {
        Task<MemoryInfo> ReadMemoryInfo();
        Task<MemoryUtilization> ReadMemoryUtilization();
    }
}

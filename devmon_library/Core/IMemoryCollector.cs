using devmon_library.Models;
using System.Threading.Tasks;

namespace devmon_library.Core
{
    public interface IMemoryCollector
    {
        Task<MemoryInfo> ReadMemoryInfo();
        Task<MemoryUtilization> ReadMemoryUtilization();
    }
}

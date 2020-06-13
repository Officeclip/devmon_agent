using devmon_library.Models;
using System.Threading.Tasks;

namespace devmon_library.Core
{
    public interface IOsCollector
    {
        Task<OsInfo> ReadOsInfo();
        Task<OsUtilization> ReadOsUtilization();
    }
}

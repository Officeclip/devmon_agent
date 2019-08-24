using Geheb.DevMon.Agent.Models;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Core
{
    public interface IOsCollector
    {
        Task<OsInfo> ReadOsInfo();
        Task<OsUtilization> ReadOsUtilization();
    }
}

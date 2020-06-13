using devmon_library.Models;
using System.Threading.Tasks;

namespace devmon_library.Core
{
    public interface INetworkCollector
    {
        Task<NetworkInfo[]> ReadNetworkInfo();
        Task<NetworkUtilization[]> ReadNetworkUtilization();
    }
}

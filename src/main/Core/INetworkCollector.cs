using Geheb.DevMon.Agent.Models;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Core
{
    public interface INetworkCollector
    {
        Task<NetworkInfo[]> ReadNetworkInfo();
        Task<NetworkUtilization[]> ReadNetworkUtilization();
    }
}

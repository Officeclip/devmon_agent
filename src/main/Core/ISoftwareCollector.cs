using Geheb.DevMon.Agent.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Core
{
    public interface ISoftwareCollector
    {
        Task<SoftwareInfo[]> ReadSoftware();
        //Task<SoftwareInfo[]> Read64Bit();
    }
}

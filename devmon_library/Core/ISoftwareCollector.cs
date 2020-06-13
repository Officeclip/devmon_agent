using devmon_library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace devmon_library.Core
{
    public interface ISoftwareCollector
    {
        Task<SoftwareInfo[]> ReadSoftware();
        //Task<SoftwareInfo[]> Read64Bit();
    }
}

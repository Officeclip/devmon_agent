using devmon_library.Models;
using System;
using System.Threading.Tasks;

namespace devmon_library.Core
{
    public interface IDriveCollector
    {
        Task<DriveInfo[]> ReadDriveInfo();
        Task<DriveUtilization[]> ReadDriveUtilization();
    }
}

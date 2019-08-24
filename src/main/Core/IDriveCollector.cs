using Geheb.DevMon.Agent.Models;
using System;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Core
{
    public interface IDriveCollector
    {
        Task<DriveInfo[]> ReadDriveInfo();
        Task<DriveUtilization[]> ReadDriveUtilization();
    }
}

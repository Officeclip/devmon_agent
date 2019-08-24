using Geheb.DevMon.Agent.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Core
{
    sealed class DriveCollector : IDriveCollector
    {
        private ICancellation _cancellation;

        public DriveCollector(ICancellation cancellation)
        {
            _cancellation = cancellation;
        }

        public Task<DriveInfo[]> ReadDriveInfo()
        {
            var drives = System.IO.DriveInfo.GetDrives()
                .Select(di => new DriveInfo(di))
                .ToArray();

            return Task.FromResult(drives);
        }

        public Task<DriveUtilization[]> ReadDriveUtilization()
        {
            var drives = System.IO.DriveInfo.GetDrives()
                .Select(di => new DriveUtilization(di))
                .ToArray();

            return Task.FromResult(drives);
        }
    }
}

using System;

namespace devmon_library.Models
{
    public sealed class WindowsUpdateInfo
    {
        public int PendingUpdates { get; set; }
        public DateTime? LastUpdateInstalledAt { get; set; }
    }
}

﻿using System;

namespace Geheb.DevMon.Agent.Models
{
    public sealed class WindowsUpdateInfo
    {
        public int PendingUpdates { get; set; }
        public DateTime? LastUpdateInstalledAt { get; set; }
    }
}

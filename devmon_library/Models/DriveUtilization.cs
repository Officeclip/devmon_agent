using System;
namespace devmon_library.Models
{
    public sealed class DriveUtilization
    {
        public string Name { get; set; }
        public ulong? FreeBytes { get; set; }

        public DriveUtilization(System.IO.DriveInfo drive)
        {
            Name = drive.Name;
            FreeBytes = drive.IsReady ? (ulong)drive.TotalFreeSpace : default(ulong?);
        }

        public DriveUtilization()
        {

        }
    }
}

namespace Geheb.DevMon.Agent.Models
{
    public sealed class DriveInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string Format { get; set; }
        public ulong? TotalBytes { get; set; }

        public DriveInfo(System.IO.DriveInfo drive)
        {
            Name = drive.Name;
            Type = drive.DriveType.ToString();
            Label = drive.IsReady ? drive.VolumeLabel : null;
            Format = drive.IsReady ? drive.DriveFormat : null;
            TotalBytes = drive.IsReady ? (ulong)drive.TotalSize : default(ulong?);
        }

        public DriveInfo()
        {

        }
    }
}

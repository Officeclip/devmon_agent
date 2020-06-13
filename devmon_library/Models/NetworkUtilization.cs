namespace devmon_library.Models
{
    public sealed class NetworkUtilization
    {
        public string Name { get; set; }
        public ulong ReceivedBytesPerSecond { get; set; }
        public ulong SentBytesPerSecond { get; set; }
    }
}

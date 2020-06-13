using System.Collections;

namespace devmon_library.Models
{
    public sealed class OsInfo
    {
        public string MachineName { get; set; }
        public byte Bitness { get; set; }
        public string Edition { get; set; }
        public string Version { get; set; }
        public string InstalledUICulture { get; set; }
        public IDictionary EnvironmentVariables { get; set; }
    }
}

using System.Collections;

namespace Geheb.DevMon.Agent.Models
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

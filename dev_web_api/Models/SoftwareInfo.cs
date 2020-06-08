using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dev_web_api.Models
{
    public class SoftwareInfo
    {
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("installed_date")]
        public string InstalledDate { get; set; }
        [JsonProperty("publisher")]
        public string Publisher { get; set; }
        [JsonProperty("estimated_size")]
        public string EstimatedSize { get; set; }
        [JsonProperty("bit_size")]
        public int BitSize { get; set; }
    }
}

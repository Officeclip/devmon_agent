namespace dev_web_api.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class MonitorSettings
    {
        [JsonProperty("db_location")]
        public string DbLocation { get; set; }

        [JsonProperty("email")]
        public Email Email { get; set; }
    }

    public partial class Email
    {
        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("port")]
        public string Port { get; set; }

        [JsonProperty("is-ssl")]
        public bool IsSsl { get; set; }

        [JsonProperty("is_start_tls")]
        public bool IsStartTls { get; set; }

        [JsonProperty("from_email")]
        public string FromEmail { get; set; }
    }

    public partial class MonitorSettings
    {
        public static MonitorSettings FromJson(string json) => JsonConvert.DeserializeObject<MonitorSettings>(json);
    }

}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmon_library.Models
{
    public class ResultInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("is_success")]
        public bool IsSuccess { get; set; } = true;

        [JsonProperty("return_code")]
        public int ReturnCode { get; set; } = 0;

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; } = "";

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        public ResultInfo()
        {

        }

        public ResultInfo(string id, string value, string unit)
        {
            Id = id;
            Value = value;
            Unit = unit;
        }
    }
}

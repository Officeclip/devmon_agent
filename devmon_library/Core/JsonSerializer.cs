using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace devmon_library.Core
{
    internal sealed class JsonSerializer : IJsonSerializer
    {
        public string SerializeWithFormatting(object value)
        {
            return Serialize(value, true);
        }

        public string SerializeWithoutFormatting(object value)
        {
            return Serialize(value, false);
        }

        private string Serialize(object value, bool indentedFormatting = false)
        {
            var json = JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                Formatting = indentedFormatting ? Formatting.Indented : Formatting.None,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            });
            return json;
        }
    }
}

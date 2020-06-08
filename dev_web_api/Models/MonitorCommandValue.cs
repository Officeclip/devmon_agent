﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using dev_web_api.Models;
//
//    var MonitorValue = MonitorValue.FromJson(jsonString);

namespace dev_web_api.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class MonitorValue
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("is_success")]
        public bool IsSuccess { get; set; }

        [JsonProperty("return_code")]
        public int ReturnCode { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("value")]
        [JsonConverter(typeof(ParseStringConverter))]
        public double Value { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }
    }

    public partial class MonitorValue
    {
        public static List<MonitorValue> FromJson(string json) => JsonConvert.DeserializeObject<List<MonitorValue>>(json, dev_web_api.Models.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<MonitorValue> self) => JsonConvert.SerializeObject(self, dev_web_api.Models.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            double l;
            if (double.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
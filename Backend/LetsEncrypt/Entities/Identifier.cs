using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PemConverter.Backend.LetsEncrypt.Entities
{
    public class Identifier
    {
        [JsonProperty("type")]
        public IdentifierType Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IdentifierType
    {
        [EnumMember(Value = "dns")]
        Dns
    }
}
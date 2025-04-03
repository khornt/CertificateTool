﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PemConverter.Backend.LetsEncrypt.Entities
{
    public class Authorization : BaseEntity
    {
        [JsonProperty("identifier")]
        public Identifier Identifier { get; set; }

        [JsonProperty("status")]
        public AuthorizationStatus? Status { get; set; }

        [JsonProperty("expires")]
        public DateTime? Expires { get; set; }

        [JsonProperty("scope")]
        public Uri Scope { get; set; }

        [JsonProperty("challenges")]
        public IList<Challenge> Challenges { get; set; }

        [JsonProperty("wildcard")]
        public bool? Wildcard { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuthorizationStatus
    {
        [EnumMember(Value = "pending")]
        Pending,

        [EnumMember(Value = "processing")]
        Processing,

        [EnumMember(Value = "valid")]
        Valid,

        [EnumMember(Value = "invalid")]
        Invalid,

        [EnumMember(Value = "revoked")]
        Revoked,

        [EnumMember(Value = "deactivated")]
        Deactivated,

        [EnumMember(Value = "expired")]
        Expired,
    }
}
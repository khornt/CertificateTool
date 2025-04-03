using LetsEncrypt.Client.Json;
using Newtonsoft.Json;
using System;

namespace PemConverter.Backend.LetsEncrypt.WebClient
{
    public partial class AcmeClient : AcmeBaseWebClient
    {
        private readonly JsonSerializerSettings _jsonSettings = JsonSettings.CreateSettings();

        // Ctor

        public AcmeClient(Uri directoryUri)
            : base(directoryUri)
        {
        }
    }
}
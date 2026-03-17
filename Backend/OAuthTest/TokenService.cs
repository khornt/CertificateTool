using LetsEncrypt.Client.Json;
using Newtonsoft.Json;
using PemConverter.Models.Dto.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PemConverter.Backend.OAuthTest
{
    public class TokenService
    {
        private readonly static JsonSerializerSettings jsonSettings = JsonSettings.CreateSettings();
        private readonly static Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(() => new HttpClient());
        public TokenService() 
        { 
        }

        public async Task<TokenResponseDto> GetToken()
        {
            var tokenEndpoint = "https://auth.horntvedt.com/realms/master/protocol/openid-connect/token";
            var clientId = "kristian-api-write";
            var clientSecret = "Yzl9VK6U8eQawUm6LZwjvoQSagiOdPEt";
            //var scope = "https://graph.microsoft.com/.default";
            var grantType = "client_credentials";
            var requestBody = new Dictionary<string, string>
            {
                { "grant_type", grantType },
                { "client_id", clientId },
                { "client_secret", clientSecret }                                
            };
            var content = new FormUrlEncodedContent(requestBody);
            var response = _httpClient.Value.PostAsync(tokenEndpoint, content).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponseDto>(responseContent, jsonSettings);
                Console.WriteLine($"Access Token: {tokenResponse.AccessToken}"); 
                return tokenResponse;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                Console.WriteLine(responseContent);
            }

            return null;
        }

    }
}

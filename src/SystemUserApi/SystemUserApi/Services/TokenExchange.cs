using Microsoft.Extensions.Options;
using SmartCloud.Server.Config;
using SmartCloud.Server.Services.Interfaces;
using System.Net.Http.Headers;

namespace SmartCloud.Server.Services
{
    public class TokenExchange : ITokenExchange
    {
        private readonly HttpClient _client;

        private readonly MaskinportenConfig _maskinportenConfig;

        public TokenExchange(HttpClient httpClient, IOptions<MaskinportenConfig> maskinportenConfig)
        {
            _client = httpClient;
            _maskinportenConfig = maskinportenConfig.Value;
        }

        public async Task<string> ExhangeMaskinporten(string token)
        {
           _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine("Exchanging token" + _maskinportenConfig.AltinnExchangeEndpoint);
            HttpResponseMessage httpResponse = await _client.GetAsync(_maskinportenConfig.AltinnExchangeEndpoint);
            return await httpResponse.Content.ReadAsStringAsync();
        }
    }
}

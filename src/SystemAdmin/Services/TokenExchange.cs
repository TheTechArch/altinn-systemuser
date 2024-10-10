using SuperSystem.Server.Services.Interfaces;
using System.Net.Http.Headers;

namespace SuperSystem.Server.Services
{
    public class TokenExchange : ITokenExchange
    {
        private readonly HttpClient _client;
        public TokenExchange(HttpClient httpClient)
        {
            _client = httpClient;
        }

        public async Task<string> ExhangeMaskinporten(string token)
        {
           _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage httpResponse = await _client.GetAsync("https://platform.at22.altinn.cloud/authentication/api/v1/exchange/maskinporten");
            return await httpResponse.Content.ReadAsStringAsync();
        }
    }
}

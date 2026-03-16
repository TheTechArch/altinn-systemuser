using System.Net.Http.Headers;
using OnPremOnboarder.Config;

namespace OnPremOnboarder.Services;

public class TokenExchangeService
{
    private readonly HttpClient _client;
    private readonly OnboarderConfig _config;

    public TokenExchangeService(HttpClient httpClient, OnboarderConfig config)
    {
        _client = httpClient;
        _config = config;
    }

    /// <summary>
    /// Exchanges a Maskinporten token for an Altinn platform token
    /// </summary>
    public async Task<string> ExchangeMaskinporten(string maskinportenToken)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", maskinportenToken);
        string url = $"{_config.AltinnBaseAddress}/authentication/api/v1/exchange/maskinporten";
        HttpResponseMessage response = await _client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Token exchange failed ({response.StatusCode}): {error}");
        }

        return await response.Content.ReadAsStringAsync();
    }
}

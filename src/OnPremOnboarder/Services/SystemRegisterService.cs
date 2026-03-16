using System.Net.Http.Json;
using System.Text.Json;
using OnPremOnboarder.Config;
using OnPremOnboarder.Models;

namespace OnPremOnboarder.Services;

public class SystemRegisterService
{
    private readonly HttpClient _client;
    private readonly MaskinportenService _maskinportenService;
    private readonly TokenExchangeService _tokenExchangeService;
    private readonly OnboarderConfig _config;

    public SystemRegisterService(
        HttpClient httpClient,
        MaskinportenService maskinportenService,
        TokenExchangeService tokenExchangeService,
        OnboarderConfig config)
    {
        _client = httpClient;
        _maskinportenService = maskinportenService;
        _tokenExchangeService = tokenExchangeService;
        _config = config;
    }

    /// <summary>
    /// Creates a system in the Altinn System Register.
    /// System ID = {orgNumber}_{systemName}
    /// </summary>
    public async Task CreateSystem()
    {
        string systemId = $"{_config.SystemOwnerOrgNumber}_{_config.SystemId}";

        Console.WriteLine($"Creating system: {systemId}");

        // Build rights from single resources
        List<Right> rights = _config.SingleResources
            .Select(resource => new Right
            {
                Resource = [new AttributePair { Id = "urn:altinn:resource", Value = resource }]
            })
            .ToList();

        // Build access packages
        List<AccessPackage> accessPackages = _config.Packages
            .Select(pkg => new AccessPackage { Urn = pkg })
            .ToList();

        // Build redirect urls
        List<Uri> redirectUrls = [];
        if (!string.IsNullOrEmpty(_config.RedirectUrl))
        {
            redirectUrls.Add(new Uri(_config.RedirectUrl));
        }

        SystemRegisterRequest request = new SystemRegisterRequest
        {
            Id = systemId,
            Vendor = new Vendor { ID = $"0192:{_config.SystemOwnerOrgNumber}" },
            Name = new Dictionary<string, string>
            {
                { "nb", _config.SystemName },
                { "nn", _config.SystemName },
                { "en", _config.SystemName }
            },
            Description = new Dictionary<string, string>
            {
                { "nb", _config.SystemName },
                { "nn", _config.SystemName },
                { "en", _config.SystemName }
            },
            Rights = rights,
            AccessPackages = accessPackages,
            ClientId = [_config.ClientId],
            IsVisible = true,
            AllowedRedirectUrls = redirectUrls,
        };

        // Get Maskinporten token and exchange for Altinn platform token
        TokenResponse tokenResponse = await _maskinportenService.GetToken(_config.SystemRegisterScope);
        string altinnToken = await _tokenExchangeService.ExchangeMaskinporten(tokenResponse.AccessToken);

        // Call system register API
        _client.BaseAddress = new Uri(_config.AltinnBaseAddress);
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {altinnToken}");

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            "/authentication/api/v1/systemregister/vendor", request);

        string responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"System created successfully!");
            Console.WriteLine($"System ID: {systemId}");
            Console.WriteLine($"Response: {responseBody}");
        }
        else
        {
            Console.WriteLine($"Failed to create system ({response.StatusCode})");
            Console.WriteLine($"Response: {responseBody}");
        }
    }
}

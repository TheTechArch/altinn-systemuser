using System.Net.Http.Json;
using OnPremOnboarder.Config;
using OnPremOnboarder.Models;

namespace OnPremOnboarder.Services;

public class SystemUserRequestService
{
    private readonly HttpClient _client;
    private readonly MaskinportenService _maskinportenService;
    private readonly TokenExchangeService _tokenExchangeService;
    private readonly OnboarderConfig _config;

    public SystemUserRequestService(
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
    /// Creates a system user request for the configured organization.
    /// Returns the approval URL that the organization admin must open.
    /// </summary>
    public async Task CreateRequest()
    {
        string systemId = $"{_config.SystemOwnerOrgNumber}_{_config.SystemId}";

        Console.WriteLine($"Creating system user request for system: {systemId}");
        Console.WriteLine($"Target organization: {_config.EffectiveSystemUserOrgNumber}");

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

        SystemUserRequestCreate request = new SystemUserRequestCreate
        {
            SystemId = systemId,
            PartyOrgNo = _config.EffectiveSystemUserOrgNumber,
            Rights = rights,
            AccessPackages = accessPackages,
            RedirectUrl = _config.RedirectUrl,
        };

        // Get Maskinporten token and exchange for Altinn platform token
        TokenResponse tokenResponse = await _maskinportenService.GetToken(_config.SystemUserRequestScope);
        string altinnToken = await _tokenExchangeService.ExchangeMaskinporten(tokenResponse.AccessToken);

        // Call system user request API
        _client.BaseAddress = new Uri(_config.AltinnBaseAddress);
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {altinnToken}");

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            "/authentication/api/v1/systemuser/request/vendor", request);

        string responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            SystemUserRequestResponse? requestResponse =
                System.Text.Json.JsonSerializer.Deserialize<SystemUserRequestResponse>(responseBody);

            Console.WriteLine($"System user request created successfully!");
            Console.WriteLine($"Request ID: {requestResponse?.Id}");
            Console.WriteLine();
            Console.WriteLine($"=== APPROVAL URL ===");
            Console.WriteLine(requestResponse?.ConfirmUrl);
            Console.WriteLine($"====================");
            Console.WriteLine();
            Console.WriteLine("Open the URL above in a browser to approve the system user request.");
        }
        else
        {
            Console.WriteLine($"Failed to create system user request ({response.StatusCode})");
            Console.WriteLine($"Response: {responseBody}");
        }
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using OnPremOnboarder.Config;
using OnPremOnboarder.Models;

namespace OnPremOnboarder.Services;

public class MaskinportenService
{
    private readonly HttpClient _client;
    private readonly OnboarderConfig _config;

    public MaskinportenService(HttpClient httpClient, OnboarderConfig config)
    {
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client = httpClient;
        _config = config;
    }

    /// <summary>
    /// Gets a Maskinporten token for the given scope (no system user claims)
    /// </summary>
    public async Task<TokenResponse> GetToken(string scope)
    {
        JsonWebKey jwk = DecodeJwk();
        string jwtAssertion = BuildJwtAssertion(jwk, scope, systemUserOrgno: null);
        FormUrlEncodedContent content = BuildFormContent(jwtAssertion);
        return await PostToken(content);
    }

    /// <summary>
    /// Gets a Maskinporten token with system user authorization_details
    /// </summary>
    public async Task<TokenResponse> GetSystemUserToken(string scope, string systemUserOrgno)
    {
        JsonWebKey jwk = DecodeJwk();
        string jwtAssertion = BuildJwtAssertion(jwk, scope, systemUserOrgno);
        FormUrlEncodedContent content = BuildFormContent(jwtAssertion);
        return await PostToken(content);
    }

    private JsonWebKey DecodeJwk()
    {
        byte[] bytes = Convert.FromBase64String(_config.EncodedJwk);
        string jwkJson = Encoding.UTF8.GetString(bytes);
        return new JsonWebKey(jwkJson);
    }

    private string BuildJwtAssertion(JsonWebKey jwk, string scope, string? systemUserOrgno)
    {
        DateTimeOffset now = new DateTimeOffset(DateTime.UtcNow);

        JwtHeader header = new JwtHeader(new SigningCredentials(jwk, SecurityAlgorithms.RsaSha256));

        JwtPayload payload = new JwtPayload
        {
            { "aud", GetAudience() },
            { "scope", scope },
            { "iss", _config.ClientId },
            { "exp", now.ToUnixTimeSeconds() + 10 },
            { "iat", now.ToUnixTimeSeconds() },
            { "jti", Guid.NewGuid().ToString() },
        };

        if (systemUserOrgno != null)
        {
            JwtPayload systemUserOrg = new JwtPayload
            {
                { "authority", "iso6523-actorid-upis" },
                { "ID", $"0192:{systemUserOrgno}" },
            };

            JwtPayload authorizationDetail = new JwtPayload
            {
                { "systemuser_org", systemUserOrg },
                { "type", "urn:altinn:systemuser" }
            };

            payload.Add("authorization_details", new List<JwtPayload> { authorizationDetail });
        }

        JwtSecurityToken securityToken = new JwtSecurityToken(header, payload);
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(securityToken);
    }

    private FormUrlEncodedContent BuildFormContent(string assertion)
    {
        return new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
        {
            new("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
            new("assertion", assertion),
        });
    }

    private async Task<TokenResponse> PostToken(FormUrlEncodedContent content)
    {
        HttpRequestMessage request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(GetTokenEndpoint()),
            Content = content
        };

        HttpResponseMessage response = await _client.SendAsync(request);
        string responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<TokenResponse>(responseBody)
                ?? throw new Exception("Failed to deserialize Maskinporten token response");
        }

        ErrorResponse? error = JsonSerializer.Deserialize<ErrorResponse>(responseBody);
        throw new Exception($"Maskinporten error: {error?.ErrorType} - {error?.Description ?? responseBody}");
    }

    private string GetAudience() => _config.Environment switch
    {
        "prod" => "https://maskinporten.no/",
        "test" => "https://test.maskinporten.no/",
        _ => throw new ArgumentException($"Invalid environment: {_config.Environment}. Valid: prod, test")
    };

    private string GetTokenEndpoint() => _config.Environment switch
    {
        "prod" => "https://maskinporten.no/token",
        "test" => "https://test.maskinporten.no/token",
        _ => throw new ArgumentException($"Invalid environment: {_config.Environment}. Valid: prod, test")
    };
}

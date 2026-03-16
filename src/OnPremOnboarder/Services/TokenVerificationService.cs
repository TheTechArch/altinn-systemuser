using System.IdentityModel.Tokens.Jwt;
using OnPremOnboarder.Config;
using OnPremOnboarder.Models;

namespace OnPremOnboarder.Services;

public class TokenVerificationService
{
    private readonly MaskinportenService _maskinportenService;
    private readonly OnboarderConfig _config;

    public TokenVerificationService(MaskinportenService maskinportenService, OnboarderConfig config)
    {
        _maskinportenService = maskinportenService;
        _config = config;
    }

    /// <summary>
    /// Verifies that a system user token can be obtained for the configured org.
    /// This confirms the system user has been approved and is functional.
    /// </summary>
    public async Task VerifyToken()
    {
        string scope = _config.SystemRegisterScope;

        Console.WriteLine($"Requesting system user token for org: {_config.EffectiveSystemUserOrgNumber}");
        Console.WriteLine($"Scope: {scope}");

        try
        {
            TokenResponse tokenResponse = await _maskinportenService.GetSystemUserToken(
                scope, _config.EffectiveSystemUserOrgNumber);

            Console.WriteLine();
            Console.WriteLine("Token obtained successfully!");
            Console.WriteLine($"Token type: {tokenResponse.TokenType}");
            Console.WriteLine($"Scope: {tokenResponse.Scope}");
            Console.WriteLine($"Expires in: {tokenResponse.ExpiresIn}s");

            // Decode and display token claims
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(tokenResponse.AccessToken))
            {
                JwtSecurityToken jwt = handler.ReadJwtToken(tokenResponse.AccessToken);
                Console.WriteLine();
                Console.WriteLine("=== TOKEN CLAIMS ===");
                foreach (var claim in jwt.Claims)
                {
                    Console.WriteLine($"  {claim.Type}: {claim.Value}");
                }
                Console.WriteLine("====================");
            }

            Console.WriteLine();
            Console.WriteLine("System user is working correctly!");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"Failed to obtain system user token: {ex.Message}");
            Console.WriteLine("This likely means the system user request has not been approved yet.");
        }
    }
}

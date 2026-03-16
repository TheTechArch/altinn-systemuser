using Microsoft.Extensions.Configuration;
using OnPremOnboarder.Config;
using OnPremOnboarder.Services;

// Handle help before config validation
string command = args.Length > 0 ? args[0] : "help";
if (command.Equals("help", StringComparison.OrdinalIgnoreCase) || args.Length == 0)
{
    PrintHelp();
    return;
}

// Build configuration
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddUserSecrets<OnboarderConfig>(optional: true)
    .Build();

OnboarderConfig config = new OnboarderConfig
{
    ClientId = "",
    EncodedJwk = "",
    SystemOwnerOrgNumber = "",
    SystemId = "",
    SystemName = ""
};
configuration.GetSection("OnboarderConfig").Bind(config);

// Validate required config
if (string.IsNullOrEmpty(config.ClientId) || string.IsNullOrEmpty(config.EncodedJwk)
    || string.IsNullOrEmpty(config.SystemOwnerOrgNumber) || string.IsNullOrEmpty(config.SystemId)
    || string.IsNullOrEmpty(config.SystemName))
{
    Console.WriteLine("ERROR: Missing required configuration. Please set the following in appsettings.json or user secrets:");
    Console.WriteLine("  - OnboarderConfig:ClientId");
    Console.WriteLine("  - OnboarderConfig:EncodedJwk");
    Console.WriteLine("  - OnboarderConfig:SystemOwnerOrgNumber");
    Console.WriteLine("  - OnboarderConfig:SystemId");
    Console.WriteLine("  - OnboarderConfig:SystemName");
    return;
}

// Create services
MaskinportenService maskinportenService = new MaskinportenService(new HttpClient(), config);
TokenExchangeService tokenExchangeService = new TokenExchangeService(new HttpClient(), config);
SystemRegisterService systemRegisterService = new SystemRegisterService(
    new HttpClient(), maskinportenService, tokenExchangeService, config);
SystemUserRequestService systemUserRequestService = new SystemUserRequestService(
    new HttpClient(), maskinportenService, tokenExchangeService, config);
TokenVerificationService tokenVerificationService = new TokenVerificationService(maskinportenService, config);

// Route commands
try
{
    switch (command.ToLower())
    {
        case "create-system":
            await systemRegisterService.CreateSystem();
            break;

        case "create-request":
            await systemUserRequestService.CreateRequest();
            break;

        case "verify-token":
            await tokenVerificationService.VerifyToken();
            break;

        default:
            PrintHelp();
            break;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
}

void PrintHelp()
{
    Console.WriteLine("OnPremOnboarder - Altinn System User Onboarding Tool");
    Console.WriteLine();
    Console.WriteLine("Usage: OnPremOnboarder <command>");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  create-system    Register a system in the Altinn System Register");
    Console.WriteLine("                   Creates system with ID: {orgNumber}_{systemId}");
    Console.WriteLine("                   Adds the configured Maskinporten ClientId as allowed client");
    Console.WriteLine();
    Console.WriteLine("  create-request   Create a system user request for the organization");
    Console.WriteLine("                   Returns an approval URL that the org admin must open");
    Console.WriteLine();
    Console.WriteLine("  verify-token     Verify the system user works by obtaining a token");
    Console.WriteLine("                   Confirms the system user has been approved");
    Console.WriteLine();
    Console.WriteLine("Configuration (appsettings.json):");
    Console.WriteLine("  ClientId            - Maskinporten client ID");
    Console.WriteLine("  EncodedJwk          - Base64-encoded JWK for Maskinporten");
    Console.WriteLine("  Environment         - 'test' or 'prod' (default: test)");
    Console.WriteLine("  SystemOwnerOrgNumber  - Vendor organization number (required)");
    Console.WriteLine("  SystemUserOrgNumber   - Org number for system user (defaults to SystemOwnerOrgNumber)");
    Console.WriteLine("  SystemId            - Short identifier for the system (no spaces)");
    Console.WriteLine("  SystemName          - Display name of the system (can have spaces)");
    Console.WriteLine("  SystemUserType      - Type of system user (default: standard)");
    Console.WriteLine("  Packages            - List of access package URNs");
    Console.WriteLine("  SingleResources     - List of single resource identifiers");
    Console.WriteLine("  RedirectUrl         - Optional redirect URL for approval flow");
}

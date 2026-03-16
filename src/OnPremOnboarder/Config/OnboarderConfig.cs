namespace OnPremOnboarder.Config;

public class OnboarderConfig
{
    /// <summary>
    /// Maskinporten client ID
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// Base64-encoded JSON Web Key for Maskinporten authentication
    /// </summary>
    public required string EncodedJwk { get; set; }

    /// <summary>
    /// Maskinporten environment: "test" or "prod"
    /// </summary>
    public string Environment { get; set; } = "test";

    /// <summary>
    /// Organization number of the vendor (system owner). Required.
    /// Used for system registration and as the Vendor identity.
    /// </summary>
    public required string SystemOwnerOrgNumber { get; set; }

    /// <summary>
    /// Organization number of the party that will use the system user.
    /// If not set, defaults to SystemOwnerOrgNumber.
    /// </summary>
    public string? SystemUserOrgNumber { get; set; }

    /// <summary>
    /// Resolved org number for system user operations.
    /// Returns SystemUserOrgNumber if set, otherwise SystemOwnerOrgNumber.
    /// </summary>
    public string EffectiveSystemUserOrgNumber =>
        string.IsNullOrEmpty(SystemUserOrgNumber) ? SystemOwnerOrgNumber : SystemUserOrgNumber;

    /// <summary>
    /// Short identifier for the system (no spaces). Prefixed with org number to form the system ID.
    /// Example: "onprem-eksempel" becomes "312268876_onprem-eksempel"
    /// </summary>
    public required string SystemId { get; set; }

    /// <summary>
    /// Display name of the system (can contain spaces). Shown to users in Altinn.
    /// </summary>
    public required string SystemName { get; set; }

    /// <summary>
    /// Type of system user to create: "standard" or other supported types
    /// </summary>
    public string SystemUserType { get; set; } = "standard";

    /// <summary>
    /// Altinn platform base address
    /// </summary>
    public string AltinnBaseAddress { get; set; } = "https://platform.tt02.altinn.no";

    /// <summary>
    /// Scope needed for system register operations
    /// </summary>
    public string SystemRegisterScope { get; set; } = "altinn:authentication/systemregister.admin";

    /// <summary>
    /// Scope needed for system user request operations
    /// </summary>
    public string SystemUserRequestScope { get; set; } = "altinn:authentication/systemuser.request.write";

    /// <summary>
    /// Packages (access packages) to assign to the system
    /// </summary>
    public List<string> Packages { get; set; } = [];

    /// <summary>
    /// Single resources (urn format) to assign to the system
    /// </summary>
    public List<string> SingleResources { get; set; } = [];

    /// <summary>
    /// Redirect URL for system user request approval flow
    /// </summary>
    public string? RedirectUrl { get; set; }
}

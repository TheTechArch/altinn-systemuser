using System.Text.Json.Serialization;

namespace OnPremOnboarder.Models;

public class SystemUserRequestCreate
{
    [JsonPropertyName("systemId")]
    public required string SystemId { get; set; }

    [JsonPropertyName("partyOrgNo")]
    public required string PartyOrgNo { get; set; }

    [JsonPropertyName("rights")]
    public List<Right> Rights { get; set; } = [];

    [JsonPropertyName("accessPackages")]
    public List<AccessPackage> AccessPackages { get; set; } = [];

    [JsonPropertyName("redirectUrl")]
    public string? RedirectUrl { get; set; }
}

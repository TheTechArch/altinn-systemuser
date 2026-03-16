using System.Text.Json.Serialization;

namespace OnPremOnboarder.Models;

public class AccessPackage
{
    [JsonPropertyName("urn")]
    public string Urn { get; set; } = string.Empty;
}

using System.Text.Json.Serialization;

namespace OnPremOnboarder.Models;

public class AttributePair
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

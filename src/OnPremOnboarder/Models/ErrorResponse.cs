using System.Text.Json.Serialization;

namespace OnPremOnboarder.Models;

public class ErrorResponse
{
    [JsonPropertyName("error")]
    public string ErrorType { get; set; } = string.Empty;

    [JsonPropertyName("error_description")]
    public string Description { get; set; } = string.Empty;
}

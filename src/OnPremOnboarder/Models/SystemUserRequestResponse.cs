using System.Text.Json.Serialization;

namespace OnPremOnboarder.Models;

public class SystemUserRequestResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("confirmUrl")]
    public string ConfirmUrl { get; set; } = string.Empty;

    [JsonPropertyName("systemId")]
    public string SystemId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("redirectUrl")]
    public string? RedirectUrl { get; set; }
}

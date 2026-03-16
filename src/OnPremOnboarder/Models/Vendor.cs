using System.Text.Json.Serialization;

namespace OnPremOnboarder.Models;

public class Vendor
{
    [JsonPropertyName("authority")]
    public string Authority => "iso6523-actorid-upis";

    [JsonPropertyName("ID")]
    public string ID { get; set; } = string.Empty;
}

using System.Text.Json.Serialization;

namespace ASFBuffBot.Data;

public record BaseBuffResponse
{
    [JsonPropertyName("msg")]
    public string? Message { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }
}
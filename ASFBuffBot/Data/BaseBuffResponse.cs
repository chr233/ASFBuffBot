using Newtonsoft.Json;

namespace ASFBuffBot.Data;

public record BaseBuffResponse
{
    [JsonProperty(PropertyName = "msg", Required = Required.Default)]
    public string? Message { get; set; }

    [JsonProperty(PropertyName = "code", Required = Required.Default)]
    public string? Code { get; set; }
}
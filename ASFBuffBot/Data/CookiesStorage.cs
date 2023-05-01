using Newtonsoft.Json;

namespace ASFBuffBot.Data;

public sealed record CookiesStorage
{
    [JsonProperty(Required = Required.Default)]
    public Dictionary<string, string>? Data { get; set; }
}

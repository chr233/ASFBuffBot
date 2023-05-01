using Newtonsoft.Json;

namespace ASFBuffBot.Data;

public sealed record PluginConfig
{
    [JsonProperty(Required = Required.DisallowNull)]
    public bool Statistic { get; set; } = true;

    [JsonProperty(Required = Required.Default)]
    public List<string>? DisabledCmds { get; set; }

    [JsonProperty(Required = Required.Default)]
    public List<string>? EnabledBotNames { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public uint BuffCheckInterval { get; set; } = 180;

    [JsonProperty(Required = Required.Default)]
    public string? CustomUserAgent { get; set; }

    //[JsonProperty(Required = Required.Default)]
    //public string? SuccessWebHook { get; set; }

    //[JsonProperty(Required = Required.Default)]
    //public string? FailedWebHook { get; set; }
}

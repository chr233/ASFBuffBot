using Newtonsoft.Json;

namespace ASFBuffBot.Data;

public sealed record PluginConfig
{
    /// <summary>
    /// 启用统计信息
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public bool Statistic { get; set; } = true;

    /// <summary>
    /// 禁用命令表
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public List<string>? DisabledCmds { get; set; }

    /// <summary>
    /// Buff检测检测间隔
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public uint BuffCheckInterval { get; set; } = 180;

    /// <summary>
    /// 每个Bot检测间隔
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public uint BotInterval { get; set; } = 180;

    /// <summary>
    /// 自定义浏览器UA
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public string? CustomUserAgent { get; set; }
}

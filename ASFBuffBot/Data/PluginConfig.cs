using Newtonsoft.Json;

namespace ASFBuffBot.Data;

public sealed record PluginConfig
{
    /// <summary>
    /// 是否同意使用协议
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public bool EULA { get; set; }
    /// <summary>
    /// 启用统计信息
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public bool Statistic { get; set; } = true;

    /// <summary>
    /// Buff检测检测间隔
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public uint BuffCheckInterval { get; set; } = 180;

    /// <summary>
    /// 每个Bot检测间隔
    /// </summary>
    [JsonProperty(Required = Required.DisallowNull)]
    public uint BotInterval { get; set; } = 30;

    /// <summary>
    /// 自定义浏览器UA
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public string? CustomUserAgent { get; set; }

    /// <summary>
    /// 交易不匹配时自动拒绝
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public bool RejectNotMatch { get; set; }

    /// <summary>
    /// 总是发送手机验证码
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public bool AlwaysSendSmsCode { get; set; }
}

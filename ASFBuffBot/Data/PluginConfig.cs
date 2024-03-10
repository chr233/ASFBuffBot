using System.Text.Json.Serialization;

namespace ASFBuffBot.Data;

/// <summary>
/// 插件配置
/// </summary>
public sealed record PluginConfig
{
    /// <summary>
    /// 是否同意使用协议
    /// </summary>
    public bool EULA { get; set; }
    /// <summary>
    /// 启用统计信息
    /// </summary>
    public bool Statistic { get; set; } = true;

    /// <summary>
    /// Buff检测检测间隔
    /// </summary>
    public uint BuffCheckInterval { get; set; } = 180;

    /// <summary>
    /// 每个Bot检测间隔
    /// </summary>
    public uint BotInterval { get; set; } = 30;

    /// <summary>
    /// 自定义浏览器UA
    /// </summary>
    public string? CustomUserAgent { get; set; }

    /// <summary>
    /// 交易不匹配时自动拒绝
    /// </summary>
    public bool RejectNotMatch { get; set; }

    /// <summary>
    /// 总是发送手机验证码
    /// </summary>
    public bool AlwaysSendSmsCode { get; set; }
}

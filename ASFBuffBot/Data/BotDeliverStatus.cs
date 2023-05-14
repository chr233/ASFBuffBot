namespace ASFBuffBot.Data;

public sealed record BotDeliverStatus
{
    /// <summary>
    /// 自动发货数量统计
    /// </summary>
    public int DeliverAcceptCount { get; set; }
    /// <summary>
    /// 自动发货数量统计
    /// </summary>
    public int DeliverRejectCount { get; set; }
    /// <summary>
    /// 警告信息
    /// </summary>
    public string? Message { get; set; }
}
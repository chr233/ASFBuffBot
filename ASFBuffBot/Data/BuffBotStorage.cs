namespace ASFBuffBot.Data;

public sealed class BuffBotStorage : Dictionary<string, BotStorage>
{
}

public sealed record BotStorage
{
    public bool Enabled { get; set; }
    public string? Cookies { get; set; }
}
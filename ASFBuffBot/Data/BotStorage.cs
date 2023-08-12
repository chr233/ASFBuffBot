namespace ASFBuffBot.Data;

public sealed record BotStorage
{
    public bool Enabled { get; set; }
    public string? Cookies { get; set; }
}
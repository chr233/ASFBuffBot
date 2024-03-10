using System.Text.Json.Serialization;

namespace ASFBuffBot.Data;
public sealed record BuffNotificationResponse : BaseBuffResponse
{
    [JsonPropertyName("data")]
    public NotificationData? Data { get; set; }
}

public sealed record NotificationData
{
    [JsonPropertyName("to_deliver_order")]
    public MessageData? ToDeliverOrder { get; set; }
}

public sealed record MessageData
{
    [JsonPropertyName("csgo")]
    public int Csgo { get; set; }

    [JsonPropertyName("pubg")]
    public int Pubg { get; set; }

    [JsonPropertyName("dota2")]
    public int Dota2 { get; set; }
}
using Newtonsoft.Json;

namespace ASFBuffBot.Data;
public sealed record BuffNotificationResponse
{
    [JsonProperty(PropertyName = "msg", Required = Required.Default)]
    public string? Message { get; set; }

    [JsonProperty(PropertyName = "code", Required = Required.Default)]
    public string? Code { get; set; }

    [JsonProperty(PropertyName = "data", Required = Required.Default)]
    public NotificationData? Data { get; set; }

    public sealed record NotificationData
    {
        [JsonProperty(PropertyName = "to_deliver_order", Required = Required.Default)]
        public MessageData? ToDeliverOrder { get; set; }
    }

    public sealed record MessageData
    {
        [JsonProperty(PropertyName = "csgo", Required = Required.Default)]
        public int Csgo { get; set; }

        [JsonProperty(PropertyName = "pubg", Required = Required.Default)]
        public int Pubg { get; set; }

        [JsonProperty(PropertyName = "dota2", Required = Required.Default)]
        public int Dota2 { get; set; }
    }
}


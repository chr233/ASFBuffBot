using System.Text.Json.Serialization;

namespace ASFBuffBot.Data;
public sealed record BuffUserInfoResponse : BaseBuffResponse
{
    [JsonPropertyName("data")]
    public UserInfoData? Data { get; set; }
}
public sealed record UserInfoData
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("nickname")]
    public string? NickName { get; set; }

    [JsonPropertyName("steamid")]
    public string? SteamId { get; set; }
}
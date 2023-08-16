using Newtonsoft.Json;

namespace ASFBuffBot.Data;
public sealed record BuffUserInfoResponse : BaseBuffResponse
{
    [JsonProperty(PropertyName = "data", Required = Required.Default)]
    public UserInfoData? Data { get; set; }
}
public sealed record UserInfoData
{
    [JsonProperty(PropertyName = "id", Required = Required.Default)]
    public string? Id { get; set; }

    [JsonProperty(PropertyName = "nickname", Required = Required.Default)]
    public string? NickName { get; set; }

    [JsonProperty(PropertyName = "steamid", Required = Required.Default)]
    public string? SteamId { get; set; }
}
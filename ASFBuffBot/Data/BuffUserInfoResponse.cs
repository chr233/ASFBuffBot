using Newtonsoft.Json;

namespace ASFBuffBot.Data;
public sealed record BuffUserInfoResponse
{
    [JsonProperty(PropertyName = "msg", Required = Required.AllowNull)]
    public string? Message { get; set; }

    [JsonProperty(PropertyName = "code", Required = Required.AllowNull)]
    public string? Code { get; set; }

    [JsonProperty(PropertyName = "data", Required = Required.AllowNull)]
    public UserInfoData? Data { get; set; }

    public sealed record UserInfoData
    {
        [JsonProperty(PropertyName = "id", Required = Required.AllowNull)]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "nickname", Required = Required.AllowNull)]
        public string? NickName { get; set; }

        [JsonProperty(PropertyName = "steamid", Required = Required.AllowNull)]
        public ulong SteamId { get; set; }
    }
}


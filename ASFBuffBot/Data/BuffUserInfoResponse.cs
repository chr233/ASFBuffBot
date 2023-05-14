using Newtonsoft.Json;

namespace ASFBuffBot.Data;
public sealed record BuffUserInfoResponse
{
    [JsonProperty(PropertyName = "msg", Required = Required.Default)]
    public string? Message { get; set; }

    [JsonProperty(PropertyName = "code", Required = Required.Default)]
    public string? Code { get; set; }

    [JsonProperty(PropertyName = "data", Required = Required.Default)]
    public UserInfoData? Data { get; set; }

    public sealed record UserInfoData
    {
        [JsonProperty(PropertyName = "id", Required = Required.Default)]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "nickname", Required = Required.Default)]
        public string? NickName { get; set; }

        [JsonProperty(PropertyName = "steamid", Required = Required.Default)]
        public ulong SteamId { get; set; }
    }
}


using Newtonsoft.Json;
using static ASFBuffBot.Data.BuffUserInfoResponse;

namespace ASFBuffBot.Data;
public sealed record BuffUserInfoResponse : AbstractBuffResponse<UserInfoData>
{
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


using Newtonsoft.Json;

namespace ASFBuffBot.Data;
internal sealed record TradeOfferAcceptResponse
{
    [JsonProperty("strError", Required = Required.DisallowNull)]
    public string ErrorText { get; set; } = "";

    [JsonProperty("needs_mobile_confirmation", Required = Required.DisallowNull)]
    public bool RequiresMobileConfirmation { get; set; }
}


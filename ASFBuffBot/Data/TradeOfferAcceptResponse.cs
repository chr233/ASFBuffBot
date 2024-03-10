using System.Text.Json.Serialization;

namespace ASFBuffBot.Data;
public sealed record TradeOfferAcceptResponse
{
    [JsonPropertyName("strError")]
    public string ErrorText { get; set; } = "";

    [JsonPropertyName("needs_mobile_confirmation")]
    public bool RequiresMobileConfirmation { get; set; }
}


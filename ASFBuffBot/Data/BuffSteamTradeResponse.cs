using System.Text.Json.Serialization;

namespace ASFBuffBot.Data;

public sealed record BuffSteamTradeResponse : BaseBuffResponse
{
    [JsonPropertyName("data")]
    public List<SteamTradeData>? Data { get; set; }
}

public sealed record SteamTradeData
{
    /// <summary>
    /// 账户头像
    /// </summary>
    [JsonPropertyName("bot_avatar")]
    public string BotAvatar { get; set; } = "";

    /// <summary>
    /// 账户额外信息
    /// </summary>
    [JsonPropertyName("bot_extra_info")]
    public string BotExtraInfo { get; set; } = "";

    /// <summary>
    /// 账户昵称
    /// </summary>
    [JsonPropertyName("bot_name")]
    public string BotName { get; set; } = "";

    /// <summary>
    /// 账户注册时间
    /// </summary>
    [JsonPropertyName("bot_steam_created_at")]
    public long BotSteamCreatedAt { get; set; }

    /// <summary>
    /// 订单创建时间
    /// </summary>
    [JsonPropertyName("created_at")]
    public long CreateAt { get; set; }

    /// <summary>
    /// 游戏类型
    /// </summary>
    [JsonPropertyName("game")]
    public string Game { get; set; } = "";

    /// <summary>
    /// Buff交易ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    /// <summary>
    /// 待交易物品信息
    /// </summary>
    [JsonPropertyName("items_to_trade")]
    public List<IteamToTradeData> ItemsToTrade { get; set; } = new();

    [JsonPropertyName("state")]
    public int State { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("trace_url")]
    public string TraceUrl { get; set; } = "";

    [JsonPropertyName("tradeofferid")]
    public string TradeOfferId { get; set; } = "";

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";

    [JsonPropertyName("verify_code")]
    public string VerifyCode { get; set; } = "";
}

public sealed record GoodInfoData
{
    [JsonPropertyName("appid")]
    public uint AppId { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("game")]
    public string Game { get; set; } = "";

    [JsonPropertyName("goods_id")]
    public long GoodsId { get; set; }

    [JsonPropertyName("icon_url")]
    public string IconUrl { get; set; } = "";

    [JsonPropertyName("market_hash_name")]
    public string MarketHashName { get; set; } = "";

    [JsonPropertyName("market_min_price")]
    public double MarketHashPrice { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("original_icon_url")]
    public string OriginalIconUrl { get; set; } = "";

    [JsonPropertyName("short_name")]
    public string ShortName { get; set; } = "";

    [JsonPropertyName("steam_price")]
    public double SteamPrice { get; set; }

    [JsonPropertyName("steam_price_cny")]
    public double SteamPriceCny { get; set; }

    [JsonPropertyName("tags")]
    public Dictionary<string, TagData>? Tags { get; set; }
}

public sealed record IteamToTradeData
{
    [JsonPropertyName("appid")]
    public uint AppId { get; set; }

    [JsonPropertyName("assetid")]
    public ulong AssetId { get; set; }

    [JsonPropertyName("classid")]
    public ulong ClassId { get; set; }

    [JsonPropertyName("contextid")]
    public ulong ContextId { get; set; }

    //[JsonPropertyName("goods_id")]
    //public long GoodsId { get; set; }

    [JsonPropertyName("instanceid")]
    public ulong InstanceID { get; set; }
}

public sealed record TagData
{
    [JsonPropertyName("category")]
    public string Category { get; set; } = "";

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("internal_name")]
    public string InternalName { get; set; } = "";

    [JsonPropertyName("localized_name")]
    public string LocalizedName { get; set; } = "";
}

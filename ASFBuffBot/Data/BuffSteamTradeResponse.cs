using Newtonsoft.Json;

namespace ASFBuffBot.Data;
public sealed record BuffSteamTradeResponse
{

    [JsonProperty(PropertyName = "msg", Required = Required.AllowNull)]
    public string? Message { get; set; }

    [JsonProperty(PropertyName = "code", Required = Required.AllowNull)]
    public string? Code { get; set; }

    [JsonProperty(PropertyName = "data", Required = Required.AllowNull)]
    public List<SteamTradeData>? Data { get; set; }

    public sealed record SteamTradeData
    {
        /// <summary>
        /// AppId
        /// </summary>
        [JsonProperty(PropertyName = "appid", Required = Required.Always)]
        public uint AppId { get; set; }
        /// <summary>
        /// 账户年限
        /// </summary>
        [JsonProperty(PropertyName = "bot_age", Required = Required.Always)]
        public uint BotAge { get; set; }
        /// <summary>
        /// 账户年限图标
        /// </summary>
        [JsonProperty(PropertyName = "bot_age_icon", Required = Required.Always)]
        public string BotAgeIcon { get; set; } = "";

        /// <summary>
        /// 账户头像
        /// </summary>
        [JsonProperty(PropertyName = "bot_avatar", Required = Required.Always)]
        public string BotAvatar { get; set; } = "";

        /// <summary>
        /// 账户额外信息
        /// </summary>
        [JsonProperty(PropertyName = "bot_extra_info", Required = Required.Always)]
        public string BotExtraInfo { get; set; } = "";

        /// <summary>
        /// 账户等级
        /// </summary>
        [JsonProperty(PropertyName = "bot_level", Required = Required.Always)]
        public int BotLevel { get; set; }

        /// <summary>
        /// 账户昵称
        /// </summary>
        [JsonProperty(PropertyName = "bot_name", Required = Required.Always)]
        public string BotName { get; set; } = "";

        /// <summary>
        /// 账户注册时间
        /// </summary>
        [JsonProperty(PropertyName = "bot_steam_created_at", Required = Required.Always)]
        public long BotSteamCreatedAt { get; set; }

        /// <summary>
        /// 订单创建时间
        /// </summary>
        [JsonProperty(PropertyName = "created_at", Required = Required.Always)]
        public long CreateAt { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        [JsonProperty(PropertyName = "game", Required = Required.Always)]
        public string Game { get; set; } = "";

        /// <summary>
        /// 商品信息
        /// </summary>
        [JsonProperty(PropertyName = "goods_infos", Required = Required.Always)]
        public Dictionary<string, GoodInfoData> GoodsInfos { get; set; } = new();

        /// <summary>
        /// Buff交易ID
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id { get; set; } = "";

        /// <summary>
        /// 待交易物品信息
        /// </summary>
        [JsonProperty(PropertyName = "items_to_trade", Required = Required.Always)]
        public List<IteamToTradeData> ItemsToTrade { get; set; } = new();

        [JsonProperty(PropertyName = "state", Required = Required.Always)]
        public int State { get; set; }

        [JsonProperty(PropertyName = "text", Required = Required.Always)]
        public string Text { get; set; } = "";

        [JsonProperty(PropertyName = "title", Required = Required.Always)]
        public string Title { get; set; } = "";

        [JsonProperty(PropertyName = "trace_url", Required = Required.Always)]
        public string TraceUrl { get; set; } = "";

        [JsonProperty(PropertyName = "tradeofferid", Required = Required.Always)]
        public string TradeOfferId { get; set; } = "";

        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public int Type { get; set; }

        [JsonProperty(PropertyName = "url", Required = Required.Always)]
        public string Url { get; set; } = "";

        [JsonProperty(PropertyName = "verify_code", Required = Required.Always)]
        public string VerifyCode { get; set; } = "";
    }

    public sealed record GoodInfoData
    {
        [JsonProperty(PropertyName = "appid", Required = Required.Always)]
        public uint AppId { get; set; }

        [JsonProperty(PropertyName = "description", Required = Required.AllowNull)]
        public string? Description { get; set; }

        [JsonProperty(PropertyName = "game", Required = Required.Always)]
        public string Game { get; set; } = "";

        [JsonProperty(PropertyName = "goods_id", Required = Required.Always)]
        public long GoodsId { get; set; }

        [JsonProperty(PropertyName = "icon_url", Required = Required.Always)]
        public string IconUrl { get; set; } = "";

        [JsonProperty(PropertyName = "market_hash_name", Required = Required.AllowNull)]
        public string MarketHashName { get; set; } = "";

        [JsonProperty(PropertyName = "market_min_price", Required = Required.AllowNull)]
        public double MarketHashPrice { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; } = "";

        [JsonProperty(PropertyName = "original_icon_url", Required = Required.AllowNull)]
        public string OriginalIconUrl { get; set; } = "";

        [JsonProperty(PropertyName = "short_name", Required = Required.AllowNull)]
        public string ShortName { get; set; } = "";

        [JsonProperty(PropertyName = "steam_price", Required = Required.AllowNull)]
        public double SteamPrice { get; set; }

        [JsonProperty(PropertyName = "steam_price_cny", Required = Required.AllowNull)]
        public double SteamPriceCny { get; set; }

        [JsonProperty(PropertyName = "tags", Required = Required.Default)]
        public Dictionary<string, TagData>? Tags { get; set; }
    }

    public sealed record IteamToTradeData
    {
        [JsonProperty(PropertyName = "appid", Required = Required.Always)]
        public uint AppId { get; set; }

        [JsonProperty(PropertyName = "assetid", Required = Required.Always)]
        public ulong AssetId { get; set; }

        [JsonProperty(PropertyName = "classid", Required = Required.Always)]
        public ulong ClassId { get; set; }

        [JsonProperty(PropertyName = "contextid", Required = Required.Always)]
        public ulong ContextId { get; set; }

        [JsonProperty(PropertyName = "goods_id", Required = Required.Always)]
        public long GoodsId { get; set; }

        [JsonProperty(PropertyName = "instanceid", Required = Required.Always)]
        public ulong InstanceID { get; set; }
    }

    public sealed record TagData
    {
        [JsonProperty(PropertyName = "category", Required = Required.Always)]
        public string Category { get; set; } = "";

        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "internal_name", Required = Required.Always)]
        public string InternalName { get; set; } = "";

        [JsonProperty(PropertyName = "localized_name", Required = Required.Default)]
        public string LocalizedName { get; set; } = "";
    }
}


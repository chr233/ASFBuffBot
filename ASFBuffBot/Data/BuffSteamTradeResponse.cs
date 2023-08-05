using Newtonsoft.Json;
using static ASFBuffBot.Data.BuffSteamTradeResponse;

namespace ASFBuffBot.Data;

public sealed record BuffSteamTradeResponse : AbstractBuffResponse<List<SteamTradeData>>
{
    public sealed record SteamTradeData
    {
        /// <summary>
        /// AppId
        /// </summary>
        [JsonProperty(PropertyName = "appid", Required = Required.Default)]
        public uint AppId { get; set; }
        /// <summary>
        /// 账户年限
        /// </summary>
        [JsonProperty(PropertyName = "bot_age", Required = Required.Default)]
        public uint BotAge { get; set; }
        /// <summary>
        /// 账户年限图标
        /// </summary>
        [JsonProperty(PropertyName = "bot_age_icon", Required = Required.Default)]
        public string BotAgeIcon { get; set; } = "";

        /// <summary>
        /// 账户头像
        /// </summary>
        [JsonProperty(PropertyName = "bot_avatar", Required = Required.Default)]
        public string BotAvatar { get; set; } = "";

        /// <summary>
        /// 账户额外信息
        /// </summary>
        [JsonProperty(PropertyName = "bot_extra_info", Required = Required.Default)]
        public string BotExtraInfo { get; set; } = "";

        /// <summary>
        /// 账户等级
        /// </summary>
        [JsonProperty(PropertyName = "bot_level", Required = Required.Default)]
        public int BotLevel { get; set; }

        /// <summary>
        /// 账户昵称
        /// </summary>
        [JsonProperty(PropertyName = "bot_name", Required = Required.Default)]
        public string BotName { get; set; } = "";

        /// <summary>
        /// 账户注册时间
        /// </summary>
        [JsonProperty(PropertyName = "bot_steam_created_at", Required = Required.Default)]
        public long BotSteamCreatedAt { get; set; }

        /// <summary>
        /// 订单创建时间
        /// </summary>
        [JsonProperty(PropertyName = "created_at", Required = Required.Default)]
        public long CreateAt { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        [JsonProperty(PropertyName = "game", Required = Required.Default)]
        public string Game { get; set; } = "";

        /// <summary>
        /// 商品信息
        /// </summary>
        [JsonProperty(PropertyName = "goods_infos", Required = Required.Default)]
        public Dictionary<string, GoodInfoData> GoodsInfos { get; set; } = new();

        /// <summary>
        /// Buff交易ID
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Default)]
        public string Id { get; set; } = "";

        /// <summary>
        /// 待交易物品信息
        /// </summary>
        [JsonProperty(PropertyName = "items_to_trade", Required = Required.Default)]
        public List<IteamToTradeData> ItemsToTrade { get; set; } = new();

        [JsonProperty(PropertyName = "state", Required = Required.Default)]
        public int State { get; set; }

        [JsonProperty(PropertyName = "text", Required = Required.Default)]
        public string Text { get; set; } = "";

        [JsonProperty(PropertyName = "title", Required = Required.Default)]
        public string Title { get; set; } = "";

        [JsonProperty(PropertyName = "trace_url", Required = Required.Default)]
        public string TraceUrl { get; set; } = "";

        [JsonProperty(PropertyName = "tradeofferid", Required = Required.Default)]
        public string TradeOfferId { get; set; } = "";

        [JsonProperty(PropertyName = "type", Required = Required.Default)]
        public int Type { get; set; }

        [JsonProperty(PropertyName = "url", Required = Required.Default)]
        public string Url { get; set; } = "";

        [JsonProperty(PropertyName = "verify_code", Required = Required.Default)]
        public string VerifyCode { get; set; } = "";
    }

    public sealed record GoodInfoData
    {
        [JsonProperty(PropertyName = "appid", Required = Required.Default)]
        public uint AppId { get; set; }

        [JsonProperty(PropertyName = "description", Required = Required.Default)]
        public string? Description { get; set; }

        [JsonProperty(PropertyName = "game", Required = Required.Default)]
        public string Game { get; set; } = "";

        [JsonProperty(PropertyName = "goods_id", Required = Required.Default)]
        public long GoodsId { get; set; }

        [JsonProperty(PropertyName = "icon_url", Required = Required.Default)]
        public string IconUrl { get; set; } = "";

        [JsonProperty(PropertyName = "market_hash_name", Required = Required.Default)]
        public string MarketHashName { get; set; } = "";

        [JsonProperty(PropertyName = "market_min_price", Required = Required.Default)]
        public double MarketHashPrice { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        public string Name { get; set; } = "";

        [JsonProperty(PropertyName = "original_icon_url", Required = Required.Default)]
        public string OriginalIconUrl { get; set; } = "";

        [JsonProperty(PropertyName = "short_name", Required = Required.Default)]
        public string ShortName { get; set; } = "";

        [JsonProperty(PropertyName = "steam_price", Required = Required.Default)]
        public double SteamPrice { get; set; }

        [JsonProperty(PropertyName = "steam_price_cny", Required = Required.Default)]
        public double SteamPriceCny { get; set; }

        [JsonProperty(PropertyName = "tags", Required = Required.Default)]
        public Dictionary<string, TagData>? Tags { get; set; }
    }

    public sealed record IteamToTradeData
    {
        [JsonProperty(PropertyName = "appid", Required = Required.Default)]
        public uint AppId { get; set; }

        [JsonProperty(PropertyName = "assetid", Required = Required.Default)]
        public ulong AssetId { get; set; }

        [JsonProperty(PropertyName = "classid", Required = Required.Default)]
        public ulong ClassId { get; set; }

        [JsonProperty(PropertyName = "contextid", Required = Required.Default)]
        public ulong ContextId { get; set; }

        [JsonProperty(PropertyName = "goods_id", Required = Required.Default)]
        public long GoodsId { get; set; }

        [JsonProperty(PropertyName = "instanceid", Required = Required.Default)]
        public ulong InstanceID { get; set; }
    }

    public sealed record TagData
    {
        [JsonProperty(PropertyName = "category", Required = Required.Default)]
        public string Category { get; set; } = "";

        [JsonProperty(PropertyName = "id", Required = Required.Default)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "internal_name", Required = Required.Default)]
        public string InternalName { get; set; } = "";

        [JsonProperty(PropertyName = "localized_name", Required = Required.Default)]
        public string LocalizedName { get; set; } = "";
    }
}


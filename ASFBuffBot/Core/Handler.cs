using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Data;
using ArchiSteamFarm.Steam.Exchange;
using ArchiSteamFarm.Steam.Security;
using System.Collections.Concurrent;

namespace ASFBuffBot.Core;

internal static class Handler
{
    private static ConcurrentDictionary<string, ConcurrentDictionary<string, TradeOffer>> BotTradeCache { get; } = new();

    private static int CheckCount { get; set; }

    private const int CheckCountMax = 20;

    /// <summary>
    /// 读取Buff发货详情
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task CheckDeliver(Bot bot)
    {
        if (!bot.HasMobileAuthenticator)
        {
            Utils.Logger.LogGenericWarning("当前Bot未设置两步验证令牌, 跳过执行");
            return;
        }

        if (!Utils.BuffCookies.TryGetValue(bot.BotName, out string? cookies))
        {
            Utils.Logger.LogGenericWarning("未设置有效的 BuffCookies, 跳过执行");
            return;
        }

        //验证Cookies
        if (CheckCount++ >= CheckCountMax)
        {
            var valid = await WebRequest.CheckCookiesValid(bot).ConfigureAwait(false);
            if (!valid)
            {
                Utils.BuffCookies[bot.BotName] = null;
            }
            CheckCount = 0;
        }

        //检查通知
        var notifResponse = await WebRequest.FetchBuffNotification(bot).ConfigureAwait(false);
        if (notifResponse?.Code != "OK" || notifResponse?.Data?.ToDeliverOrder == null)
        {
            Utils.Logger.LogGenericWarning(string.Format("BuffNotification网络请求失败, Code: {0}", notifResponse?.Code));
            return;
        }

        int csgo = notifResponse.Data.ToDeliverOrder.Csgo;
        int dota2 = notifResponse.Data.ToDeliverOrder.Dota2;

        if (csgo + dota2 == 0)
        {
            Utils.Logger.LogGenericDebug("无等待自动发货的物品");
            return;
        }
        else
        {
            Utils.Logger.LogGenericInfo(string.Format("检测到共有 {0} 个Csgo订单, {1} 个Dota2订单等待发货", csgo, dota2));
        }

        //检查待发货订单
        var tradeResponse = await WebRequest.FetchBuffSteamTrade(bot).ConfigureAwait(false);
        if (tradeResponse?.Code != "OK" || tradeResponse?.Data == null)
        {
            Utils.Logger.LogGenericWarning(string.Format("BuffSteamTrade网络请求失败, Code: {0}", tradeResponse?.Code));
            return;
        }
        else
        {
            int totalItems = tradeResponse.Data.Select(x => x.ItemsToTrade.Count).Sum();
            Utils.Logger.LogGenericInfo(string.Format("检测到共有 {0} 个物品等待发货", totalItems));
        }

        foreach (var buffTrade in tradeResponse.Data)
        {
            var tradeId = buffTrade.TradeOfferId;
            if (BotTradeCache.TryGetValue(tradeId, out var steamTrade))
            {
                //检查物品信息是否匹配
                int matchCount = 0;
                if (buffTrade.ItemsToTrade.Count == steamTrade.ItemsToGiveReadOnly.Count)
                {
                    foreach (var buffItem in buffTrade.ItemsToTrade)
                    {
                        foreach (var steamItem in steamTrade.ItemsToGiveReadOnly)
                        {
                            if ((steamItem.AppID == buffItem.AppId || steamItem.RealAppID == buffItem.AppId) &&
                                steamItem.AssetID == buffItem.AssetId &&
                                steamItem.ClassID == buffItem.ClassId &&
                                steamItem.ContextID == buffItem.ContextId &&
                                steamItem.InstanceID == buffItem.InstanceID)
                            {
                                matchCount++;
                                break;
                            }
                        }
                    }
                }

                bool accept = matchCount == steamTrade.ItemsToGiveReadOnly.Count;
                if (accept)
                {
                    Utils.Logger.LogGenericInfo(string.Format("交易物品匹配, 自动发货, Id: {0}", tradeId));
                }
                else
                {
                    Utils.Logger.LogGenericWarning(string.Format("交易物品不匹配, 自动拒绝报价, Id: {0}", tradeId));
                }

                if (accept)
                {
                    var response = await WebRequest.AcceptTradeOffer(bot, tradeId).ConfigureAwait(false);
                    if (response == null)
                    {
                        Utils.Logger.LogGenericError(string.Format("同意报价失败, ID: {0}", tradeId));
                        continue;
                    }
                    else
                    {
                        if (response.RequiresMobileConfirmation)
                        {
                            Utils.Logger.LogGenericInfo(string.Format("同意报价成功, 需要两步验证, ID: {0}", tradeId));

                            var offerIDs = new List<ulong> { steamTrade.TradeOfferID };
                            (bool success, _, string message) = await bot.Actions.HandleTwoFactorAuthenticationConfirmations(accept, Confirmation.EType.Trade, offerIDs, true).ConfigureAwait(false);

                            Utils.Logger.LogGenericWarning(string.Format("{0}交易报价{1}, Msg: {2}, Id: {2}", accept ? "接受" : "拒绝", success ? Langs.Success : Langs.Failure, message, tradeId));

                            if (success)
                            {
                                BotTradeCache.TryRemove(tradeId, out _);
                            }
                            continue;
                        }
                        else
                        {
                            Utils.Logger.LogGenericInfo(string.Format("同意报价成功, 无需两步验证, Id: {0}", tradeId));
                            BotTradeCache.TryRemove(tradeId, out _);
                            continue;
                        }

                    }


                }
                else
                {
                    var result = await WebRequest.DeclineTradeOffer(bot, tradeId).ConfigureAwait(false);
                    Utils.Logger.LogGenericWarning(string.Format("拒绝交易{0}, Id: {1}", result ? Langs.Success : Langs.Failure, tradeId));
                    BotTradeCache.TryRemove(tradeId, out _);
                    continue;
                }
            }
            else
            {
                Utils.Logger.LogGenericInfo(string.Format("为找到匹配的交易报价, 可能交易报价已取消, Id: {0}", tradeId));
                continue;
            }
        }
    }

    internal static void AddTradeCache(Bot bot, TradeOffer tradeOffer)
    {
        if (!BotTradeCache.TryGetValue(bot.BotName, out var tradeCache))
        {
            tradeCache = new ConcurrentDictionary<string, TradeOffer>();
            BotTradeCache[bot.BotName] = tradeCache;
        }

        var tradeId = tradeOffer.TradeOfferID.ToString();
        if (!tradeCache.TryAdd(tradeId, tradeOffer))
        {
            tradeCache[tradeId] = tradeOffer;
            Utils.Logger.LogGenericDebug(string.Format("更新交易, ID: {0}", tradeId));
        }
        else
        {
            Utils.Logger.LogGenericDebug(string.Format("收到新交易, ID:{0}", tradeId));
        }
    }

    internal static void RemoveTradeCache(Bot bot, IReadOnlyCollection<ParseTradeResult> tradeResult)
    {
        if (!BotTradeCache.TryGetValue(bot.BotName, out var tradeCache))
        {
            tradeCache = new ConcurrentDictionary<string, TradeOffer>();
            BotTradeCache[bot.BotName] = tradeCache;
        }

        foreach (var trade in tradeResult)
        {
            var tradeId = trade.TradeOfferID.ToString();
            tradeCache.TryRemove(tradeId, out _);
            Utils.Logger.LogGenericDebug(string.Format("交易已完成, ID: {0} => {1}", tradeId, trade.Result.ToString()));
        }
    }
}

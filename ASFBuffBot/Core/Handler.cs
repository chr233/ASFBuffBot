using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Data;
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
            Utils.Logger.LogGenericWarning(Langs.No2FaSetSkip);
            return;
        }

        if (!Utils.BuffCookies.TryGetValue(bot.BotName, out string? cookies) || string.IsNullOrEmpty(cookies))
        {
            Utils.Logger.LogGenericWarning(Langs.NoBuffCookiesSkip);
            return;
        }

        if (!BotTradeCache.TryGetValue(bot.BotName, out var tradeCache))
        {
            tradeCache = new();
            BotTradeCache[bot.BotName] = tradeCache;
            Utils.Logger.LogGenericWarning(Langs.NoTradeCacheSkip);
            return;
        }

        //验证Cookies
        if (CheckCount++ >= CheckCountMax)
        {
            var valid = await WebRequest.CheckCookiesValid(bot).ConfigureAwait(false);
            if (!valid)
            {
                Utils.BuffCookies[bot.BotName] = null;
                await Utils.SaveCookiesFile().ConfigureAwait(false);
            }
            CheckCount = 0;
        }

        //检查通知
        var notifResponse = await WebRequest.FetchBuffNotification(bot).ConfigureAwait(false);
        if (notifResponse?.Code != "OK" || notifResponse?.Data?.ToDeliverOrder == null)
        {
            Utils.Logger.LogGenericWarning(string.Format(Langs.BotNotificationRequestFailedSkip, notifResponse?.Code));
            return;
        }

        int csgo = notifResponse.Data.ToDeliverOrder.Csgo;
        int dota2 = notifResponse.Data.ToDeliverOrder.Dota2;

        if (csgo + dota2 == 0)
        {
            Utils.Logger.LogGenericDebug(Langs.NoItemToDeliver);
            return;
        }
        else
        {
            Utils.Logger.LogGenericInfo(string.Format(Langs.BuffDeliverCount, csgo, dota2));
        }

        //检查待发货订单
        var tradeResponse = await WebRequest.FetchBuffSteamTrade(bot).ConfigureAwait(false);
        if (tradeResponse?.Code != "OK" || tradeResponse?.Data == null)
        {
            Utils.Logger.LogGenericWarning(string.Format(Langs.BuffSteamTradeRequestFailed, tradeResponse?.Code));
            return;
        }
        else
        {
            int totalItems = tradeResponse.Data.Select(x => x.ItemsToTrade.Count).Sum();
            Utils.Logger.LogGenericInfo(string.Format(Langs.BuffDeliverItemCount, totalItems));
        }


        foreach (var buffTrade in tradeResponse.Data)
        {
            var tradeId = buffTrade.TradeOfferId;
            if (tradeCache.TryGetValue(tradeId, out var steamTrade))
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
                    Utils.Logger.LogGenericInfo(string.Format(Langs.TradeMatchAtuoAccept, tradeId));
                }
                else
                {
                    Utils.Logger.LogGenericWarning(string.Format(Langs.TradeDismatchAutoReject, tradeId));
                }

                if (accept)
                {
                    var response = await WebRequest.AcceptTradeOffer(bot, tradeId).ConfigureAwait(false);
                    if (response == null)
                    {
                        Utils.Logger.LogGenericError(string.Format(Langs.ConfitmTradeFailed, tradeId));
                        continue;
                    }
                    else
                    {
                        if (response.RequiresMobileConfirmation)
                        {
                            Utils.Logger.LogGenericInfo(string.Format(Langs.AcceptTradeSuccess2FaRequired, tradeId));

                            var offerIDs = new List<ulong> { steamTrade.TradeOfferID };
                            (bool success, _, string message) = await bot.Actions.HandleTwoFactorAuthenticationConfirmations(accept, Confirmation.EType.Trade, offerIDs, true).ConfigureAwait(false);

                            Utils.Logger.LogGenericWarning(string.Format(Langs.SteamTradeDetail, accept ? Langs.Approve : Langs.Reject, success ? Langs.Success : Langs.Failure, message, tradeId));

                            if (success)
                            {
                                BotTradeCache.TryRemove(tradeId, out _);
                            }
                            continue;
                        }
                        else
                        {
                            Utils.Logger.LogGenericInfo(string.Format(Langs.AcceptTradeSuccess, tradeId));
                            BotTradeCache.TryRemove(tradeId, out _);
                            continue;
                        }

                    }


                }
                else
                {
                    var result = await WebRequest.DeclineTradeOffer(bot, tradeId).ConfigureAwait(false);
                    Utils.Logger.LogGenericWarning(string.Format(Langs.RejectTrade, result ? Langs.Success : Langs.Failure, tradeId));
                    BotTradeCache.TryRemove(tradeId, out _);
                    continue;
                }
            }
            else
            {
                Utils.Logger.LogGenericInfo(string.Format(Langs.NoMatchTradeFound, tradeId));
                continue;
            }
        }
    }

    internal static void AddTradeCache(Bot bot, TradeOffer tradeOffer)
    {
        if (!BotTradeCache.TryGetValue(bot.BotName, out var tradeCache))
        {
            InitTradeCache(bot);
            if (!BotTradeCache.TryGetValue(bot.BotName, out tradeCache))
            {
                Utils.Logger.LogGenericWarning(string.Format(Langs.TradeCacheNotInit, bot.BotName));
                return;
            }
        }

        var tradeId = tradeOffer.TradeOfferID.ToString();

        bool hasTargetItem = tradeOffer.ItemsToGiveReadOnly.Any(x => x.AppID == 730 || x.AppID == 530);

        if (hasTargetItem)
        {
            if (!tradeCache.TryAdd(tradeId, tradeOffer))
            {
                tradeCache[tradeId] = tradeOffer;
                Utils.Logger.LogGenericDebug(string.Format(Langs.UpdateTradeCache, tradeId));
            }
            else
            {
                Utils.Logger.LogGenericDebug(string.Format(Langs.ReceivedNewTradeCache, tradeId));
            }
        }
    }

    internal static void InitTradeCache(Bot bot)
    {
        if (!BotTradeCache.ContainsKey(bot.BotName))
        {
            BotTradeCache[bot.BotName] = new ConcurrentDictionary<string, TradeOffer>();
        }
        else
        {
            Utils.Logger.LogGenericWarning(string.Format(Langs.TradeCacheAlreadyInit, bot.BotName));
        }
    }

    internal static void ClearTradeCache(Bot bot)
    {
        if (!BotTradeCache.TryRemove(bot.BotName, out _))
        {
            Utils.Logger.LogGenericWarning(string.Format(Langs.TradeCacheNotInitCantClear, bot.BotName));
        }
    }

    internal static int TradeCacheCount(Bot bot)
    {
        if (BotTradeCache.TryGetValue(bot.BotName, out var tradeCache))
        {
            return tradeCache.Count;
        }
        else
        {
            return -1;
        }
    }
}

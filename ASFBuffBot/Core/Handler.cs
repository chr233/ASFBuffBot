using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Data;
using ASFBuffBot.Data;
using System.Collections.Concurrent;

namespace ASFBuffBot.Core;

internal static class Handler
{
    private static ConcurrentDictionary<string, ConcurrentDictionary<string, TradeOffer>> BotTradeCache { get; } = new();

    private static ConcurrentDictionary<string, BotDeliverStatus> BotDeliverStatus { get; } = new();

    private static int CheckCount { get; set; }

    private const int CheckCountMax = 20;

    internal static async void OnBuffTimer(object? _ = null)
    {
        var bots = Bot.BotsReadOnly;
        if (bots != null)
        {
            foreach (var (_, bot) in bots)
            {
                if (Utils.BuffBotStorage.TryGetValue(bot.BotName, out var storage))
                {
                    //if (storage.Enabled)
                    //{
                    try
                    {
                        Utils.Logger.LogGenericInfo(string.Format(Langs.StartDeliverCheck, bot.BotName));

                        bool delay = await CheckDeliver(bot).ConfigureAwait(false);

                        Utils.Logger.LogGenericInfo(Langs.EndDeliverCheck);

                        if (delay)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(Utils.Config.BotInterval)).ConfigureAwait(false);
                        }

                        storage.Cookies = bot.ArchiWebHandler.WebBrowser.GetBuffCookies();
                    }
                    catch (Exception ex)
                    {
                        Utils.Logger.LogGenericException(ex, Langs.ErrorDeliverCheck);
                    }
                    //}
                    //else
                    //{
                    //    Utils.Logger.LogGenericInfo(string.Format(Langs.BotDisabledBuff, bot.BotName));
                    //}
                }
            }

            await Utils.SaveFile().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 读取Buff发货详情
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<bool> CheckDeliver(Bot bot)
    {
        if (!bot.IsConnectedAndLoggedOn)
        {
            Utils.Logger.LogGenericInfo(Langs.NotLoginSkip);
            return false;
        }

        if (!bot.HasMobileAuthenticator)
        {
            Utils.Logger.LogGenericWarning(Langs.No2FaSetSkip);
            return false;
        }

        string name = bot.BotName;
        //读取交易缓存
        if (!BotTradeCache.TryGetValue(name, out var tradeCache) || !BotDeliverStatus.TryGetValue(name, out var status))
        {
            InitTradeCache(bot);
            if (!BotTradeCache.TryGetValue(name, out tradeCache) || !BotDeliverStatus.TryGetValue(name, out status))
            {
                Utils.Logger.LogGenericWarning(string.Format(Langs.NoTradeCacheSkip, name));
                return false;
            }
        }

        //无交易信息, 跳过
        if (!tradeCache.Any())
        {
            Utils.Logger.LogGenericInfo(string.Format(Langs.NoTradeCacheSkip, name));
        }

        //验证Buff登录状态
        if (CheckCount++ == 0)
        {
            var login = await WebRequest.CheckCookiesValid(bot).ConfigureAwait(false);
            if (!login)
            {
                await WebRequest.LoginToBuffViaSteam(bot).ConfigureAwait(false);
                login = await WebRequest.CheckCookiesValid(bot).ConfigureAwait(false);

                if (!login)
                {
                    status.Message = string.Format(Langs.AutoLoginFailedNeedCode, name);
                    //if (Utils.BuffBotStorage.TryGetValue(name, out var storage))
                    //{
                    //    storage.Enabled = false;
                    //}
                    return true;
                }
            }
            status.Message = Langs.AutoLoginSuccess;
        }
        else if (CheckCount >= CheckCountMax)
        {
            CheckCount = 0;
        }

        //检查通知
        var notifResponse = await WebRequest.FetchBuffNotification(bot).ConfigureAwait(false);
        if (notifResponse?.Code != "OK" || notifResponse?.Data?.ToDeliverOrder == null)
        {
            Utils.Logger.LogGenericWarning(string.Format(Langs.BotNotificationRequestFailedSkip, notifResponse?.Code));
            status.Message = Langs.FetchBuffNotificationFailed;
            return true;
        }

        int csgo = notifResponse.Data.ToDeliverOrder.Csgo;
        int dota2 = notifResponse.Data.ToDeliverOrder.Dota2;

        if (csgo + dota2 == 0)
        {
            Utils.Logger.LogGenericInfo(Langs.NoItemToDeliver);
            return true;
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
            status.Message = Langs.FetchBuffDeliverFailed;
            return true;
        }
        else
        {
            int totalItems = 0;
            foreach (var item in tradeResponse.Data)
            {
                totalItems += item.ItemsToTrade.Count;
            }
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
                    var response = await WebRequest.AcceptTradeOffer(bot, tradeId).ConfigureAwait(false);
                    if (response == null)
                    {
                        Utils.Logger.LogGenericError(string.Format(Langs.ConfitmTradeFailed, tradeId));
                        status.Message = Langs.ConfirmTradeFailed;
                        continue;
                    }
                    else
                    {
                        if (response.RequiresMobileConfirmation)
                        {
                            Utils.Logger.LogGenericInfo(string.Format(Langs.AcceptTradeSuccess2FaRequired, tradeId));

                            var offerIDs = new List<ulong> { steamTrade.TradeOfferID };
                            (bool success, _, string message) = await bot.Actions.HandleTwoFactorAuthenticationConfirmations(accept, Confirmation.EConfirmationType.Trade, offerIDs, true).ConfigureAwait(false);

                            Utils.Logger.LogGenericWarning(string.Format(Langs.SteamTradeDetail, accept ? Langs.Approve : Langs.Reject, success ? Langs.Success : Langs.Failure, message, tradeId));

                            if (success)
                            {
                                BotTradeCache.TryRemove(tradeId, out _);
                                status.DeliverAcceptCount++;
                            }
                            continue;
                        }
                        else
                        {
                            Utils.Logger.LogGenericInfo(string.Format(Langs.AcceptTradeSuccess, tradeId));
                            BotTradeCache.TryRemove(tradeId, out _);
                            status.DeliverAcceptCount++;
                            continue;
                        }
                    }
                }
                else
                {
                    if (Utils.Config.RejectNotMatch)
                    {
                        Utils.Logger.LogGenericWarning(string.Format(Langs.TradeDismatchAutoReject, tradeId));

                        var result = await WebRequest.DeclineTradeOffer(bot, tradeId).ConfigureAwait(false);
                        Utils.Logger.LogGenericWarning(string.Format(Langs.RejectTrade, result ? Langs.Success : Langs.Failure, tradeId));
                        BotTradeCache.TryRemove(tradeId, out _);
                        status.DeliverRejectCount++;
                    }
                    else
                    {
                        Utils.Logger.LogGenericWarning(string.Format(Langs.TradeDismatch, tradeId));

                    }

                    continue;
                }
            }
            else
            {
                Utils.Logger.LogGenericInfo(string.Format(Langs.NoMatchTradeFound, tradeId));
                continue;
            }
        }

        return true;
    }

    /// <summary>
    /// 添加交易缓存
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="tradeOffer"></param>
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

    /// <summary>
    /// 初始化交易缓存
    /// </summary>
    /// <param name="bot"></param>
    internal static void InitTradeCache(Bot bot)
    {
        var name = bot.BotName;
        if (!BotTradeCache.TryAdd(name, new ConcurrentDictionary<string, TradeOffer>()) || !BotDeliverStatus.TryAdd(name, new BotDeliverStatus()))
        {
            Utils.Logger.LogGenericWarning(string.Format(Langs.TradeCacheAlreadyInit, name));
        }
    }

    /// <summary>
    /// 刷新交易缓存
    /// </summary>
    /// <param name="bot"></param>
    internal static async Task FreshTradeCache(Bot bot)
    {
        var name = bot.BotName;
        if (bot.IsConnectedAndLoggedOn && BotTradeCache.TryGetValue(name, out var tradeCache))
        {
            tradeCache.Clear();
            var tradeOffers = await bot.ArchiWebHandler.GetTradeOffers(true, true, false, true).ConfigureAwait(false);
            if (tradeOffers != null)
            {
                foreach (var tradeOffer in tradeOffers)
                {
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
            }
        }
    }

    /// <summary>
    /// 清空交易缓存
    /// </summary>
    /// <param name="bot"></param>
    internal static void ClearTradeCache(Bot bot)
    {
        var name = bot.BotName;
        if (BotTradeCache.TryGetValue(name, out var tradeCache) && BotDeliverStatus.TryGetValue(name, out var status))
        {
            tradeCache.Clear();
            status.DeliverAcceptCount = 0;
            status.DeliverRejectCount = 0;
            status.Message = null;
        }
        else
        {
            Utils.Logger.LogGenericWarning(string.Format(Langs.TradeCacheNotInitCantClear, name));
        }
    }

    /// <summary>
    /// 获取交易缓存数量
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static int GetTradeCacheCount(Bot bot)
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

    /// <summary>
    /// 获取机器人交易统计
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static BotDeliverStatus? GetBotStatus(Bot bot)
    {
        if (BotDeliverStatus.TryGetValue(bot.BotName, out var status))
        {
            return status;
        }
        else
        {
            return null;
        }
    }
}

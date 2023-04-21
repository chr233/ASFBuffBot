using ArchiSteamFarm.Steam;
using ASFBuffBot.Data;
using System.Net;

namespace ASFBuffBot.Core;

internal static class WebRequest
{
    /// <summary>
    /// 生成Header
    /// </summary>
    /// <param name="cookies"></param>
    /// <returns></returns>
    private static Dictionary<string, string>? GenerateBuffHeader(string? cookies = null)
    {
        if (string.IsNullOrEmpty(Utils.Config.BuffCookies) && string.IsNullOrEmpty(cookies))
        {
            return null;
        }
        else
        {
            var header = new Dictionary<string, string>
            {
                { "user-agent", Utils.Config.CustomUserAgent?? Static.DefaultUserAgent },
                { "cookie", cookies ?? Utils.Config.BuffCookies ?? "" },
            };
            return header;
        }
    }

    /// <summary>
    /// 验证Cookies是否有效
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<bool> CheckCookiesValid(Bot bot, string? cookies = null)
    {
        var request = new Uri("https://buff.163.com/account/api/user/info");

        var headers = GenerateBuffHeader(cookies);
        if (headers == null)
        {
            return false;
        }

        var response = await bot.ArchiWebHandler.UrlGetToHtmlDocumentWithSession(request, headers, requestOptions: ArchiSteamFarm.Web.WebBrowser.ERequestOptions.ReturnRedirections, checkSessionPreemptively: false, allowSessionRefresh: false).ConfigureAwait(false);
        return response?.StatusCode == HttpStatusCode.OK;
    }


    /// <summary>
    /// 读取Buff出售通知
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<BuffNotificationResponse?> FetchBuffNotification(Bot bot)
    {
        var request = new Uri("https://buff.163.com/api/message/notification");

        var headers = GenerateBuffHeader();
        if (headers == null)
        {
            return null;
        }

        var response = await bot.ArchiWebHandler.UrlGetToJsonObjectWithSession<BuffNotificationResponse>(request, headers, requestOptions: ArchiSteamFarm.Web.WebBrowser.ERequestOptions.ReturnRedirections, checkSessionPreemptively: false, allowSessionRefresh: false).ConfigureAwait(false);
        return response?.Content;
    }

    /// <summary>
    /// 读取Buff发货详情
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<BuffSteamTradeResponse?> FetchBuffSteamTrade(Bot bot)
    {
        var request = new Uri("https://buff.163.com/api/market/steam_trade");

        var headers = GenerateBuffHeader();
        if (headers == null)
        {
            return null;
        }

        var response = await bot.ArchiWebHandler.UrlGetToJsonObjectWithSession<BuffSteamTradeResponse>(request, headers, requestOptions: ArchiSteamFarm.Web.WebBrowser.ERequestOptions.ReturnRedirections, checkSessionPreemptively: false, allowSessionRefresh: false).ConfigureAwait(false);
        return response?.Content;
    }

    /// <summary>
    /// 接受交易报价
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="tradeID"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static async Task<TradeOfferAcceptResponse?> AcceptTradeOffer(Bot bot, string tradeID)
    {
        Uri request = new(Utils.SteamCommunityURL, $"/tradeoffer/{tradeID}/accept");
        Uri referer = new(Utils.SteamCommunityURL, $"/tradeoffer/{tradeID}");

        Dictionary<string, string> data = new(3, StringComparer.Ordinal) {
            { "serverid", "1" },
            { "tradeofferid", tradeID }
        };

        var response = await bot.ArchiWebHandler.UrlPostToJsonObjectWithSession<TradeOfferAcceptResponse>(request, data: data, referer: referer).ConfigureAwait(false);

        return response?.Content;
    }

    /// <summary>
    /// 拒绝交易包报价
    /// </summary>
    /// <param name="tradeID"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static async Task<bool> DeclineTradeOffer(Bot bot, string tradeID)
    {
        Uri request = new(Utils.SteamCommunityURL, $"/tradeoffer/{tradeID}/decline");

        return await bot.ArchiWebHandler.UrlPostWithSession(request).ConfigureAwait(false);
    }
}

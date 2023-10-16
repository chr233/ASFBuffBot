using ArchiSteamFarm.Core;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Web.Responses;
using ASFBuffBot.Data;

namespace ASFBuffBot.Core;

internal static class WebRequest
{
    private const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36";

    /// <summary>
    /// 生成Header
    /// </summary>
    /// <param name="cookies"></param>
    /// <returns></returns>
    private static Dictionary<string, string> GenerateBuffHeader()
    {
        var header = new Dictionary<string, string>
        {
            { "user-agent", Utils.Config.CustomUserAgent?? DefaultUserAgent },
        };
        return header;
    }

    private static async Task<T?> GetToObjAsync<T>(this Bot bot, Uri request, Dictionary<string, string>? headers = null) where T : class
    {
        var referer = new Uri(Utils.BuffUrl, "/market/steam_inventory?game=csgo");

        headers ??= GenerateBuffHeader();

        //var response = await bot.ArchiWebHandler.UrlGetToJsonObjectWithSession<T>(request, headers, referer, checkSessionPreemptively: false, maxTries: 1, allowSessionRefresh: false).ConfigureAwait(false);
        var response = await bot.ArchiWebHandler.WebBrowser.UrlGetToJsonObject<T>(request, headers, referer, maxTries: 1).ConfigureAwait(false);
        return response?.Content;
    }

    private static async Task<T?> PostToObjAsync<T>(this Bot bot, Uri request, Dictionary<string, string>? data = null, Dictionary<string, string>? headers = null) where T : class
    {
        var referer = new Uri(Utils.BuffUrl, "/market/steam_inventory?game=csgo");

        headers ??= GenerateBuffHeader();

        //var response = await bot.ArchiWebHandler.UrlPostToJsonObjectWithSession<T>(request, headers, data, referer, session: ArchiSteamFarm.Steam.Integration.ArchiWebHandler.ESession.None, checkSessionPreemptively: false, maxTries: 1).ConfigureAwait(false);
        var response = await bot.ArchiWebHandler.WebBrowser.UrlPostToJsonObject<T, Dictionary<string, string>>(request, headers, data, referer, maxTries: 1).ConfigureAwait(false);
        return response?.Content;
    }

    /// <summary>
    /// 验证Cookies是否有效
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<bool> CheckCookiesValid(Bot bot)
    {
        var response = await FetchBuffSteamTrade(bot).ConfigureAwait(false);
        if (response?.Code != null)
        {
            Utils.ASFLogger.LogGenericInfo(response.Code);
        }
        return response?.Code == "OK";
    }

    internal static async Task<bool> BuffSendSmsCode(Bot bot)
    {
        var request = new Uri(Utils.BuffUrl, "/account/api/logged_in_from_steam/send_authcode");

        var headers = GenerateBuffHeader();
        var cookieValue = bot.ArchiWebHandler.WebBrowser.CookieContainer.GetCookieValue(Utils.BuffUrl, "csrf_token");
        if (string.IsNullOrEmpty(cookieValue))
        {
            return false;
        }
        headers.Add("X-CSRFToken", cookieValue);

        var response = await bot.PostToObjAsync<BaseBuffResponse>(request, null, headers).ConfigureAwait(false);
        return response?.Code == "OK";
    }

    internal static async Task<bool> BuffVerifyAuthCode(Bot bot, string authCode)
    {
        var request = new Uri(Utils.BuffUrl, "/account/api/logged_in_from_steam/verify_authcode");

        var headers = GenerateBuffHeader();
        var cookieValue = bot.ArchiWebHandler.WebBrowser.CookieContainer.GetCookieValue(Utils.BuffUrl, "csrf_token");
        if (string.IsNullOrEmpty(cookieValue))
        {
            return false;
        }

        headers.Add("X-CSRFToken", cookieValue);

        var data = new Dictionary<string, string>
        {
            { "authcode", authCode }
        };

        var response = await bot.PostToObjAsync<BaseBuffResponse>(request, data, headers).ConfigureAwait(false);
        return response?.Code == "OK";
    }

    /// <summary>
    /// 读取Buff出售通知
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<BuffNotificationResponse?> FetchBuffNotification(Bot bot)
    {
        var request = new Uri(Utils.BuffUrl, "/api/message/notification");
        var response = await bot.GetToObjAsync<BuffNotificationResponse>(request, null).ConfigureAwait(false);
        return response;
    }

    /// <summary>
    /// 读取Buff发货详情
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<BuffSteamTradeResponse?> FetchBuffSteamTrade(Bot bot)
    {
        var request = new Uri(Utils.BuffUrl, "/api/market/steam_trade");
        var response = await bot.GetToObjAsync<BuffSteamTradeResponse>(request, null).ConfigureAwait(false);
        return response;
    }

    /// <summary>
    /// 读取Buff用户信息
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="cookies"></param>
    /// <returns></returns>
    internal static async Task<BuffUserInfoResponse?> FetcbBuffUserInfo(Bot bot)
    {
        var request = new Uri(Utils.BuffUrl, "/account/api/user/info");

        var response = await bot.GetToObjAsync<BuffUserInfoResponse>(request, null).ConfigureAwait(false);
        return response;
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
        var request = new Uri(Utils.SteamCommunityURL, $"/tradeoffer/{tradeID}/accept");
        var referer = new Uri(Utils.SteamCommunityURL, $"/tradeoffer/{tradeID}");

        var data = new Dictionary<string, string>(3, StringComparer.Ordinal) {
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
        var request = new Uri(Utils.SteamCommunityURL, $"/tradeoffer/{tradeID}/decline");
        return await bot.ArchiWebHandler.UrlPostWithSession(request).ConfigureAwait(false);
    }

    /// <summary>
    /// Steam OAuth登录
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<HtmlDocumentResponse?> LoginToBuffViaSteam(Bot bot)
    {
        var queries = new List<string>
        {
            "openid.mode=checkid_setup",
            "openid.ns=http://specs.openid.net/auth/2.0",
            "openid.realm=https://buff.163.com/",
            "openid.sreg.required=nickname,email,fullname",
            "openid.assoc_handle=None",
            "openid.return_to=https://buff.163.com/account/login/steam/verification?back_url=/account/steam_bind/finish",
            "openid.ns.sreg=http://openid.net/extensions/sreg/1.1",
            "openid.identity=http://specs.openid.net/auth/2.0/identifier_select",
            "openid.claimed_id=http://specs.openid.net/auth/2.0/identifier_select"
        };

        var request = new Uri(Utils.SteamCommunityURL, "/openid/login?" + string.Join('&', queries));
        var response = await bot.ArchiWebHandler.UrlGetToHtmlDocumentWithSession(request).ConfigureAwait(false);

        var eles = response?.Content?.QuerySelectorAll("#openidForm>input[name][value]");
        if (eles == null)
        {
            return null;
        }

        var formData = new Dictionary<string, string>();
        foreach (var ele in eles)
        {
            var name = ele.GetAttribute("name");
            var value = ele.GetAttribute("value");
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
            {
                formData.Add(name, value);
            }
        }

        request = new Uri(Utils.SteamCommunityURL, "/openid/login");
        var response2 = await bot.ArchiWebHandler.UrlPostToHtmlDocumentWithSession(request, data: formData).ConfigureAwait(false);
        return response2;
    }
}

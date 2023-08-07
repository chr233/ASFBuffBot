using ArchiSteamFarm.Core;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Integration;
using ArchiSteamFarm.Web;
using ArchiSteamFarm.Web.Responses;
using ASFBuffBot.Data;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace ASFBuffBot.Core;

internal static class WebRequest
{
    /// <summary>
    /// 生成Header
    /// </summary>
    /// <param name="cookies"></param>
    /// <returns></returns>
    private static Dictionary<string, string> GenerateBuffHeader()
    {
        var header = new Dictionary<string, string>
        {
            { "user-agent", Utils.Config.CustomUserAgent?? Static.DefaultUserAgent },
        };
        return header;
    }

    /// <summary>
    /// 验证Cookies是否有效
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<bool> CheckCookiesValid(Bot bot)
    {
        var response = await FetcbBuffUserInfo(bot).ConfigureAwait(false);
        return response?.Code == "OK";
    }

    internal static async Task<bool> BuffSendSmsCode(Bot bot)
    {
        var request = new Uri(Utils.BuffUrl, "/account/api/logged_in_from_steam/send_authcode");
        var referer = new Uri(Utils.BuffUrl, "/market/steam_inventory?game=csgo");

        var headers = GenerateBuffHeader();
        var cookieValue = bot.ArchiWebHandler.WebBrowser.CookieContainer.GetCookieValue(Utils.BuffUrl, "csrf_token");
        if (string.IsNullOrEmpty(cookieValue))
        {
            return false;
        }

        headers.Add("X-CSRFToken", cookieValue);

        var response = await bot.ArchiWebHandler.UrlPostToJsonObjectWithSession<BaseBuffResponse>(request, headers: headers, null, referer: referer, session: ArchiWebHandler.ESession.None).ConfigureAwait(false);
        return response?.Content?.Code == "OK";
    }

    internal static async Task<bool> BuffVerifyAuthCode(Bot bot, string authCode)
    {
        var request = new Uri(Utils.BuffUrl, "/account/api/logged_in_from_steam/verify_authcode");
        var referer = new Uri(Utils.BuffUrl, "/market/steam_inventory?game=csgo");

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
        var response = await bot.ArchiWebHandler.UrlPostToJsonObjectWithSession<BaseBuffResponse>(request, headers, data, referer: referer, session: ArchiWebHandler.ESession.None).ConfigureAwait(false);
        return response?.Content?.Code == "OK";
    }

    /// <summary>
    /// 读取Buff出售通知
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<BuffNotificationResponse?> FetchBuffNotification(Bot bot)
    {
        var request = new Uri(Utils.BuffUrl, "/api/message/notification");
        var headers = GenerateBuffHeader();
        var response = await bot.ArchiWebHandler.UrlGetToJsonObjectWithSession<BuffNotificationResponse>(request, headers, requestOptions: WebBrowser.ERequestOptions.ReturnRedirections, checkSessionPreemptively: false, allowSessionRefresh: false).ConfigureAwait(false);
        return response?.Content;
    }

    /// <summary>
    /// 读取Buff发货详情
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<BuffSteamTradeResponse?> FetchBuffSteamTrade(Bot bot)
    {
        var request = new Uri(Utils.BuffUrl, "/api/market/steam_trade");
        var headers = GenerateBuffHeader();
        var response = await bot.ArchiWebHandler.UrlGetToJsonObjectWithSession<BuffSteamTradeResponse>(request, headers, requestOptions: WebBrowser.ERequestOptions.ReturnRedirections, checkSessionPreemptively: false, allowSessionRefresh: false).ConfigureAwait(false);
        return response?.Content;
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
        var headers = GenerateBuffHeader();
        var response = await bot.ArchiWebHandler.UrlGetToJsonObjectWithSession<BuffUserInfoResponse>(request, headers, requestOptions: WebBrowser.ERequestOptions.ReturnRedirections, checkSessionPreemptively: false, allowSessionRefresh: false).ConfigureAwait(false);
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

using ArchiSteamFarm.Core;
using ArchiSteamFarm.Localization;
using ArchiSteamFarm.Steam;

namespace ASFBuffBot.Core;

internal static partial class Command
{
    /// <summary>
    /// 启用自动发货功能
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<string?> ResponseEnableBuffBot(Bot bot)
    {
        var name = bot.BotName;
        if (Utils.BuffBots.Contains(name))
        {
            return bot.FormatBotResponse(Langs.AlreadyEnabledBuff);
        }

        if (!bot.HasMobileAuthenticator)
        {
            return bot.FormatBotResponse(Langs.Need2FaToEnableBuff);
        }

        if (!bot.IsConnectedAndLoggedOn)
        {
            return bot.FormatBotResponse(Strings.BotNotConnected);
        }

        await WebRequest.LoginToBuffViaSteam(bot).ConfigureAwait(false);

        bool login = await WebRequest.CheckCookiesValid(bot).ConfigureAwait(false);

        if (!login)
        {
            await WebRequest.LoginToBuffViaSteam(bot).ConfigureAwait(false);
            login = await WebRequest.CheckCookiesValid(bot).ConfigureAwait(false);
        }

        if (login)
        {
            Handler.InitTradeCache(bot);
            await Handler.FreshTradeCache(bot).ConfigureAwait(false);
            Utils.BuffBots.Add(name);
            await Utils.SaveFile().ConfigureAwait(false);
            return bot.FormatBotResponse(Langs.EnableBuffSuccess);
        }
        else
        {
            return bot.FormatBotResponse(string.Format(Langs.EnableBuffFailed, bot.SteamID));

        }
    }

    /// <summary>
    /// 启用自动发货功能
    /// </summary>
    /// <param name="botNames"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal static async Task<string?> ResponseEnableBuffBot(string botNames)
    {
        if (string.IsNullOrEmpty(botNames))
        {
            throw new ArgumentNullException(nameof(botNames));
        }

        HashSet<Bot>? bots = Bot.GetBots(botNames);

        if ((bots == null) || (bots.Count == 0))
        {
            return Utils.FormatStaticResponse(string.Format(Strings.BotNotFound, botNames));
        }

        IList<string?>? results = await Utilities.InParallel(bots.Select(bot => ResponseEnableBuffBot(bot))).ConfigureAwait(false);

        List<string> responses = new(results.Where(result => !string.IsNullOrEmpty(result))!);

        return responses.Count > 0 ? string.Join(Environment.NewLine, responses) : null;
    }

    /// <summary>
    /// 禁用自动发货功能
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static Task<string> ResponseDisableBuffBot(Bot bot)
    {
        var name = bot.BotName;
        if (!Utils.BuffBots.Remove(name))
        {
            return Task.FromResult(bot.FormatBotResponse(Langs.NotEnabledBuff));
        }
        else
        {
            Handler.ClearTradeCache(bot);
            return Task.FromResult(bot.FormatBotResponse(Langs.DisableBuffSuccess));
        }
    }

    /// <summary>
    /// 禁用自动发货功能
    /// </summary>
    /// <param name="botNames"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal static async Task<string?> ResponseDisableBuffBot(string botNames)
    {
        if (string.IsNullOrEmpty(botNames))
        {
            throw new ArgumentNullException(nameof(botNames));
        }

        var bots = Bot.GetBots(botNames);

        if ((bots == null) || (bots.Count == 0))
        {
            return Utils.FormatStaticResponse(string.Format(Strings.BotNotFound, botNames));
        }

        var results = await Utilities.InParallel(bots.Select(bot => ResponseDisableBuffBot(bot))).ConfigureAwait(false);

        List<string> responses = new(results.Where(result => !string.IsNullOrEmpty(result))!);

        return responses.Count > 0 ? string.Join(Environment.NewLine, responses) : null;
    }

    /// <summary>
    /// 获取机器人状态
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<string> ResponseBotStatus(Bot bot)
    {
        var name = bot.BotName;
        if (!Utils.BuffBots.Contains(name))
        {
            return bot.FormatBotResponse(Langs.NotEnabledBuff);
        }

        if (!bot.IsConnectedAndLoggedOn)
        {
            return bot.FormatBotResponse(Langs.EnabledButOffline);
        }

        bool login = await WebRequest.CheckCookiesValid(bot).ConfigureAwait(false);
        var cacheCount = Handler.GetTradeCacheCount(bot);
        var status = Handler.GetBotStatus(bot);
        if (cacheCount >= 0 && status != null)
        {
            return bot.FormatBotResponse(string.Format(Langs.BuffStatus,
                login ? Langs.Valid : Langs.Invalid, cacheCount, status.DeliverAcceptCount, status.DeliverRejectCount, status.Message ?? Langs.None));
        }
        else
        {
            return bot.FormatBotResponse(Langs.InternalError);
        }
    }

    /// <summary>
    /// 获取机器人状态 (多个Bot)
    /// </summary>
    /// <param name="botNames"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal static async Task<string?> ResponseBotStatus(string botNames)
    {
        if (string.IsNullOrEmpty(botNames))
        {
            throw new ArgumentNullException(nameof(botNames));
        }

        HashSet<Bot>? bots = Bot.GetBots(botNames);

        if ((bots == null) || (bots.Count == 0))
        {
            return Utils.FormatStaticResponse(string.Format(Strings.BotNotFound, botNames));
        }

        IList<string> results = await Utilities.InParallel(bots.Select(bot => ResponseBotStatus(bot))).ConfigureAwait(false);

        List<string> responses = new(results.Where(result => !string.IsNullOrEmpty(result))!);

        return responses.Count > 0 ? string.Join(Environment.NewLine, responses) : null;
    }

    /// <summary>
    /// 命令更新Cookies
    /// </summary>
    /// <param name="cookies"></param>
    /// <returns></returns>
    internal static async Task<string?> ResponseUpdateCoolies(string botName, string cookies)
    {
        var bot = Bot.GetBot(botName);
        if (bot == null)
        {
            return Utils.FormatStaticResponse(string.Format(Strings.BotNotFound, botName));
        }

        bot.ArchiWebHandler.WebBrowser.CookieContainer.SetCookies(new Uri("https://buff.163.com"), cookies);

        var valid = await WebRequest.CheckCookiesValid(bot).ConfigureAwait(false);
        if (valid)
        {
            return bot.FormatBotResponse(Langs.BuffCookiesValid);
        }
        else
        {
            return bot.FormatBotResponse(Langs.BuffCookiesInvalid);
        }
    }
}

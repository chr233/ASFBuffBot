using ArchiSteamFarm.Core;
using ArchiSteamFarm.Localization;
using ArchiSteamFarm.Steam;
using System.Collections.Concurrent;

namespace ASFBuffBot.Core;

internal static class Command
{
    //internal static ConcurrentDictionary<Bot, BuffHandler> Handlers { get; private set; } = new();

    /// <summary>
    /// 启用自动发货功能
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<string?> ResponseEnableBuffBot(Bot bot)
    {
        var name = bot.BotName;

        if (!bot.HasMobileAuthenticator)
        {
            return bot.FormatBotResponse(Langs.Need2FaToEnableBuff);
        }

        if (!bot.IsConnectedAndLoggedOn)
        {
            return bot.FormatBotResponse(Strings.BotNotConnected);
        }

        bool login = await WebRequest.CheckCookiesValid(bot).ConfigureAwait(false);
        if (login)
        {
            await Utils.SaveFile().ConfigureAwait(false);
            return Utils.BuffBotStorage.ContainsKey(name) ? bot.FormatBotResponse(Langs.AlreadyEnabledBuff) : bot.FormatBotResponse(Langs.EnableBuffSuccess);
        }
        else
        {
            await WebRequest.LoginToBuffViaSteam(bot).ConfigureAwait(false);

            login = await WebRequest.CheckCookiesValid(bot).ConfigureAwait(false);

            if (!login || Utils.Config.AlwaysSendSmsCode)
            {
                // 如果未成功登录就发送验证码
                var success = await WebRequest.BuffSendSmsCode(bot).ConfigureAwait(false);
                if (success)
                {
                    var cookies = bot.ArchiWebHandler.WebBrowser.GetBuffCookies();
                    Utils.BuffBotStorage.TryAdd(name, new Data.BotStorage { Cookies = cookies });
                    Utils.PaddingBots.Add(name);
                    return bot.FormatBotResponse(string.Format(Langs.EnableBuffNeedCode, bot.BotName));
                }
                else
                {
                    return bot.FormatBotResponse(Langs.EnableBuffSendCodeFailed);
                }
            }

            if (login)
            {
                Handler.InitTradeCache(bot);
                await Handler.FreshTradeCache(bot).ConfigureAwait(false);

                var cookies = bot.ArchiWebHandler.WebBrowser.GetBuffCookies();
                Utils.BuffBotStorage.TryAdd(name, new Data.BotStorage { Cookies = cookies });

                await Utils.SaveFile().ConfigureAwait(false);
                return bot.FormatBotResponse(Langs.EnableBuffSuccess);
            }
            else
            {
                return bot.FormatBotResponse(string.Format(Langs.EnableBuffFailed, bot.SteamID));
            }
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

        var bots = Bot.GetBots(botNames);

        if ((bots == null) || (bots.Count == 0))
        {
            return Utils.FormatStaticResponse(string.Format(Strings.BotNotFound, botNames));
        }

        var results = await Utilities.InParallel(bots.Select(bot => ResponseEnableBuffBot(bot))).ConfigureAwait(false);

        return results.Any() ? string.Join(Environment.NewLine, results) : null;
    }

    /// <summary>
    /// 禁用自动发货功能
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static Task<string?> ResponseDisableBuffBot(Bot bot)
    {
        var name = bot.BotName;
        if (!Utils.BuffBotStorage.Remove(name))
        {
            return Task.FromResult<string?>(bot.FormatBotResponse(Langs.NotEnabledBuff));
        }
        else
        {
            Handler.ClearTradeCache(bot);
            return Task.FromResult<string?>(bot.FormatBotResponse(Langs.DisableBuffSuccess));
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

        return results.Any() ? string.Join(Environment.NewLine, results) : null;
    }

    /// <summary>
    /// 获取机器人状态
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<string?> ResponseBotStatus(Bot bot)
    {
        var name = bot.BotName;
        if (!Utils.BuffBotStorage.ContainsKey(name))
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

        var bots = Bot.GetBots(botNames);

        if ((bots == null) || (bots.Count == 0))
        {
            return Utils.FormatStaticResponse(string.Format(Strings.BotNotFound, botNames));
        }

        var results = await Utilities.InParallel(bots.Select(bot => ResponseBotStatus(bot))).ConfigureAwait(false);

        return results.Any() ? string.Join(Environment.NewLine, results) : null;
    }

    /// <summary>
    /// 输入验证码
    /// </summary>
    /// <param name="botName"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    internal static async Task<string?> ResponseVerifyCode(string botName, string code)
    {
        var bot = Bot.GetBot(botName);
        if (bot == null)
        {
            return Utils.FormatStaticResponse(string.Format(Strings.BotNotFound, botName));
        }

        if (!Utils.BuffBotStorage.TryGetValue(bot.BotName, out var storage))
        {
            return bot.FormatBotResponse(string.Format(Langs.VerifyCodeNeedLoginFirst, bot.BotName));
        }

        var result = await WebRequest.BuffVerifyAuthCode(bot, code).ConfigureAwait(false);
        var login = await WebRequest.CheckCookiesValid(bot).ConfigureAwait(false);

        if (result)
        {
            Utils.PaddingBots.Remove(bot.BotName);
            storage.Cookies = bot.ArchiWebHandler.WebBrowser.GetBuffCookies();
        }
        await Utils.SaveFile().ConfigureAwait(false);

        return bot.FormatBotResponse(string.Format(Langs.VerifyCodeAndLoginStatus, result ? Langs.Success : Langs.Failure, login ? Langs.Success : Langs.Failure));
    }

    /// <summary>
    /// 命令更新Cookies
    /// </summary>
    /// <param name="botName"></param>
    /// <param name="cookies"></param>
    /// <returns></returns>
    internal static async Task<string?> ResponseUpdateCoolies(string botName, string cookies)
    {
        var bot = Bot.GetBot(botName);
        if (bot == null)
        {
            return Utils.FormatStaticResponse(string.Format(Strings.BotNotFound, botName));
        }

        bot.ArchiWebHandler.WebBrowser.SetBuffCookies(cookies);

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

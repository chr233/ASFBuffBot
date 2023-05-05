using ArchiSteamFarm.Core;
using ArchiSteamFarm.Localization;
using ArchiSteamFarm.Steam;

namespace ASFBuffBot.Core;

internal static partial class Command
{
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

        var valid = await WebRequest.CheckCookiesValid(bot, cookies).ConfigureAwait(false);
        if (valid)
        {
            bool isAdded = Utils.BuffCookies.ContainsKey(bot.BotName);

            Utils.BuffCookies[bot.BotName] = cookies;

            if (bot.IsConnectedAndLoggedOn && !isAdded)
            {
                //重启Bot
                bot.Actions.Stop();
                bot.Actions.Start();
            }
            bool succ = await Utils.SaveCookiesFile().ConfigureAwait(false);
            return bot.FormatBotResponse(string.Format(Langs.BuffCookiesValid, succ ? Langs.Success : Langs.Failure));
        }
        else
        {
            return bot.FormatBotResponse(Langs.BuffCookiesInvalid);
        }
    }

    internal static async Task<string?> ResponseUpdateCoolies(string cookies)
    {
        try
        {
            var bot = Bot.BotsReadOnly?.FirstOrDefault(x => x.Value.IsConnectedAndLoggedOn).Value;

            if (bot == null)
            {
                return Utils.FormatStaticResponse(Langs.NoBotAvilable);
            }

            var response = await WebRequest.FetcbBuffUserInfo(bot, cookies).ConfigureAwait(false);
            if (response == null)
            {
                return Utils.FormatStaticResponse(Langs.BuffCookiesInvalid);
            }
            var steamId = response.Data?.SteamId;
            var targetBots = Bot.BotsReadOnly?.Where(x => x.Value.SteamID == steamId);
            if (targetBots?.Any() ?? false)
            {
                bot = targetBots.First().Value;

                bool isAdded = Utils.BuffCookies.ContainsKey(bot.BotName);
                Utils.BuffCookies[bot.BotName] = cookies;

                if (bot.IsConnectedAndLoggedOn && !isAdded)
                {
                    //重启Bot
                    bot.Actions.Stop();
                    bot.Actions.Start();
                }

                bool succ = await Utils.SaveCookiesFile().ConfigureAwait(false);
                return bot.FormatBotResponse(string.Format(Langs.BuffCookiesValid, succ ? Langs.Success : Langs.Failure));
            }
            else
            {
                return Utils.FormatStaticResponse(Langs.CookiesValidButNoBotFound);
            }

        }
        catch (Exception ex)
        {
            Utils.Logger.LogGenericException(ex);
            return Utils.FormatStaticResponse(string.Format(Strings.BotNotFound, "NULL"));
        }
    }

    /// <summary>
    /// 手动校验Cookies
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    internal static async Task<string?> ResponseValidCoolies(Bot bot)
    {
        if (Utils.BuffCookies.TryGetValue(bot.BotName, out string? cookies) && !string.IsNullOrEmpty(cookies))
        {
            var valid = await WebRequest.CheckCookiesValid(bot).ConfigureAwait(false);
            if (valid)
            {
                return bot.FormatBotResponse(string.Format(Langs.CurrentCookiesValid));
            }
            else
            {
                return bot.FormatBotResponse(Langs.CurrentCookiesInvalid);
            }
        }
        else
        {
            return bot.FormatBotResponse(Langs.CurrentCookiesNotSet);
        }
    }

    /// <summary>
    /// 手动校验Cookies (多个Bot)
    /// </summary>
    /// <param name="botNames"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal static async Task<string?> ResponseValidCoolies(string botNames)
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

        IList<string?> results = await Utilities.InParallel(bots.Select(bot => ResponseValidCoolies(bot))).ConfigureAwait(false);

        List<string> responses = new(results.Where(result => !string.IsNullOrEmpty(result))!);

        return responses.Count > 0 ? string.Join(Environment.NewLine, responses) : null;
    }
}

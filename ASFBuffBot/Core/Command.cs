using AngleSharp.Common;
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
    internal static async Task<string?> ResponseUpdateCoolies(Bot bot, string cookies)
    {
        var valid = await WebRequest.CheckCookiesValid(bot, cookies).ConfigureAwait(false);
        if (valid)
        {
            bool succ;
            if (bot.IsConnectedAndLoggedOn && !Utils.BuffCookies.ContainsKey(bot.BotName))
            {
                //重启Bot
                bot.Actions.Stop();
                bot.Actions.Start();
            }
            Utils.BuffCookies[bot.BotName] = cookies;
            succ = await Utils.SaveCookiesFile().ConfigureAwait(false);
            return Utils.FormatStaticResponse(string.Format("Cookies有效, Cookies信息保存{0}", succ ? Langs.Success : Langs.Failure));
        }
        else
        {
            return Utils.FormatStaticResponse("Cookies无效, 请检查Buff是否登录以及Cookies是否完整");
        }
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
            return Utils.FormatStaticResponse(Langs.BotNameInvalidCmdTips);
        }

        return await ResponseUpdateCoolies(bot, cookies).ConfigureAwait(false);
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
                return bot.FormatBotResponse(string.Format("当前Cookies有效"));
            }
            else
            {
                Utils.BuffCookies[bot.BotName] = null;
                return bot.FormatBotResponse("当前Cookies无效, 请重新设置");
            }
        }
        else
        {
            return bot.FormatBotResponse("未设置Cookies");
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

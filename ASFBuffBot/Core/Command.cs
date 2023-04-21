namespace ASFBuffBot.Core;

internal static partial class Command
{
    /// <summary>
    /// 命令更新Cookies
    /// </summary>
    /// <param name="cookies"></param>
    /// <returns></returns>
    internal static async Task<string?> ResponseUpdateCoolies(string cookies)
    {
        var bot = Utils.GetTargetBot();
        if (bot == null)
        {
            return Langs.BotNameInvalidCmdTips;
        }

        var valid = await WebRequest.CheckCookiesValid(bot, cookies).ConfigureAwait(false);
        if (valid)
        {
            Utils.Config.BuffCookies = cookies;
            var save = await Utils.SaveCookiesFile(cookies).ConfigureAwait(false);
            return Utils.FormatStaticResponse(string.Format("Cookies有效, Cookies信息保存{0}", save ? Langs.Success : Langs.Failure));
        }
        else
        {
            return Utils.FormatStaticResponse("Cookies无效, 请检查Buff是否登录以及Cookies是否完整");
        }
    }

    /// <summary>
    /// 手动校验Cookies
    /// </summary>
    /// <returns></returns>
    internal static async Task<string?> ResponseValidCoolies()
    {
        var bot = Utils.GetTargetBot();
        if (bot == null)
        {
            return Langs.BotNameInvalidCmdTips;
        }

        var valid = await WebRequest.CheckCookiesValid(bot).ConfigureAwait(false);
        if (valid)
        {
            return Utils.FormatStaticResponse(string.Format("当前设置的Cookies有效"));
        }
        else
        {
            return Utils.FormatStaticResponse("当前设置的Cookies无效, 请重新设置");
        }
    }
}

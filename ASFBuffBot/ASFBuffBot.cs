using ArchiSteamFarm.Core;
using ArchiSteamFarm.Plugins.Interfaces;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Data;
using ArchiSteamFarm.Steam.Exchange;
using ASFBuffBot.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Composition;
using System.Text;

namespace ASFBuffBot;

[Export(typeof(IPlugin))]
internal sealed class ASFBuffBot : IASF, IBotCommand2, IBotConnection, IBotTradeOffer, IBotTradeOfferResults
{
    public string Name => nameof(ASFBuffBot);
    public Version Version => Utils.MyVersion;

    [JsonProperty]
    public static PluginConfig Config => Utils.Config;

    private static Timer? BuffTimer;

    private static Timer? StatisticTimer;

    /// <summary>
    /// ASF启动事件
    /// </summary>
    /// <param name="additionalConfigProperties"></param>
    /// <returns></returns>
    public async Task OnASFInit(IReadOnlyDictionary<string, JToken>? additionalConfigProperties = null)
    {
        PluginConfig? config = null;

        if (additionalConfigProperties != null)
        {
            foreach ((string configProperty, JToken configValue) in additionalConfigProperties)
            {
                if (configProperty == nameof(ASFBuffBot) && configValue.Type == JTokenType.Object)
                {
                    try
                    {
                        config = configValue.ToObject<PluginConfig>();
                        if (config != null)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.Logger.LogGenericException(ex);
                    }
                }
            }
        }

        Utils.Config = config ?? new();

        StringBuilder warning = new();

        //统计
        if (Config.Statistic)
        {
            Uri request = new("https://asfe.chrxw.com/");
            StatisticTimer = new Timer(
                async (_) =>
                {
                    await ASF.WebBrowser!.UrlGetToHtmlDocument(request).ConfigureAwait(false);
                },
                null,
                TimeSpan.FromSeconds(30),
                TimeSpan.FromHours(24)
            );
        }

        //禁用命令
        if (Config.DisabledCmds == null)
        {
            Config.DisabledCmds = new();
        }
        else
        {
            for (int i = 0; i < Config.DisabledCmds.Count; i++)
            {
                Config.DisabledCmds[i] = Config.DisabledCmds[i].ToUpperInvariant();
            }
        }

        if (Config.BuffCheckInterval < 30)
        {
            Config.BuffCheckInterval = 30;
            warning.AppendLine(Static.Line);
            warning.AppendLine(Langs.BuffCheckIntervalWarn);
            warning.AppendLine(Static.Line);
        }

        if (Config.BotInterval < 5)
        {
            Config.BotInterval = 5;
            warning.AppendLine(Static.Line);
            warning.AppendLine(Langs.BotIntervalWarn);
            warning.AppendLine(Static.Line);
        }

        var succ = await Utils.LoadCookiesFile().ConfigureAwait(false);
        if (!succ || Utils.BuffCookies.Count == 0)
        {
            warning.AppendLine(Static.Line);
            warning.AppendLine(Langs.BuffCookiesWarn);
            warning.AppendLine(Langs.BuffCookiesWarn2);
            warning.AppendLine(Langs.BuffCookiesWarn3);
            warning.AppendLine(Langs.BuffCookiesWarn4);
            warning.AppendLine(Static.Line);
        }

        if (string.IsNullOrEmpty(Config.CustomUserAgent))
        {
            Config.CustomUserAgent = Static.DefaultUserAgent;
        }

        if (warning.Length > 0)
        {
            warning.Insert(0, Environment.NewLine);
            Utils.Logger.LogGenericWarning(warning.ToString());
        }

        BuffTimer = new Timer(
           async (_) =>
           {
               var bots = Bot.BotsReadOnly;
               if (bots != null)
               {
                   foreach (var (_, bot) in bots)
                   {
                       if (Utils.BuffCookies.ContainsKey(bot.BotName))
                       {
                           await Core.Handler.CheckDeliver(bot).ConfigureAwait(false);
                           await Task.Delay(TimeSpan.FromSeconds(Utils.Config.BotInterval)).ConfigureAwait(false);
                       }
                   }
               }
           },
           null,
           TimeSpan.FromSeconds(30),
           TimeSpan.FromSeconds(Config.BuffCheckInterval)
       );
    }

    /// <summary>
    /// 插件加载事件
    /// </summary>
    /// <returns></returns>
    public Task OnLoaded()
    {
        StringBuilder message = new("\n");
        message.AppendLine(Static.Line);
        message.AppendLine(Static.Logo);
        message.AppendLine(string.Format(Langs.PluginVer, nameof(ASFBuffBot), Utils.MyVersion.ToString()));
        message.AppendLine(Langs.PluginContact);
        message.AppendLine(Langs.PluginInfo);
        message.AppendLine(Static.Line);

        string pluginFolder = Path.GetDirectoryName(Utils.MyLocation) ?? ".";
        string backupPath = Path.Combine(pluginFolder, $"{nameof(ASFBuffBot)}.bak");
        bool existsBackup = File.Exists(backupPath);
        if (existsBackup)
        {
            try
            {
                File.Delete(backupPath);
                message.AppendLine(Langs.CleanUpOldBackup);
            }
            catch (Exception e)
            {
                Utils.Logger.LogGenericException(e);
                message.AppendLine(Langs.CleanUpOldBackupFailed);
            }
        }
        else
        {
            message.AppendLine(Langs.ASFEVersionTips);
            message.AppendLine(Langs.ASFEUpdateTips);
        }

        message.AppendLine(Static.Line);

        Utils.Logger.LogGenericInfo(message.ToString());

        return Task.CompletedTask;
    }

    /// <summary>
    /// 处理命令
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="access"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <param name="steamId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static async Task<string?> ResponseCommand(Bot bot, EAccess access, string message, string[] args)
    {
        string cmd = args[0].ToUpperInvariant();

        if (cmd.StartsWith("ABB."))
        {
            cmd = cmd[4..];
        }
        else
        {
            //跳过禁用命令
            if (Config.DisabledCmds?.Contains(cmd) == true)
            {
                Utils.Logger.LogGenericInfo("Command {0} is disabled!");
                return null;
            }
        }

        int argLength = args.Length;
        switch (argLength)
        {
            case 0:
                throw new InvalidOperationException(nameof(args));
            case 1: //不带参数
                switch (cmd)
                {
                    //Core
                    case "VALIDCOOKIES" when access >= EAccess.Master:
                    case "VC" when access >= EAccess.Master:
                        return await Core.Command.ResponseValidCoolies(bot).ConfigureAwait(false);

                    case "DELETECOOKIES" when access >= EAccess.Master:
                    case "DC" when access >= EAccess.Master:
                        return await Core.Command.ResponseDeleteCoolies(bot).ConfigureAwait(false);

                    case "COOKIESSTATUS" when access >= EAccess.Master:
                    case "CS" when access >= EAccess.Master:
                        return await Core.Command.ResponseBotStatus(bot).ConfigureAwait(false);

                    //Update
                    case "ASFBUFFBOT" when access >= EAccess.FamilySharing:
                    case "ABB" when access >= EAccess.FamilySharing:
                        return Update.Command.ResponseASFBuffBotVersion();

                    case "ABBVERSION" when access >= EAccess.Operator:
                    case "ABBV" when access >= EAccess.Operator:
                        return await Update.Command.ResponseCheckLatestVersion().ConfigureAwait(false);

                    case "ABBUPDATE" when access >= EAccess.Owner:
                    case "ABBU" when access >= EAccess.Owner:
                        return await Update.Command.ResponseUpdatePlugin().ConfigureAwait(false);

                    default:
                        return null;
                }
            default: //带参数
                switch (cmd)
                {
                    //Core
                    case "VALIDCOOKIES" when access >= EAccess.Master:
                    case "VC" when access >= EAccess.Master:
                        return await Core.Command.ResponseValidCoolies(Utilities.GetArgsAsText(args, 1, ",")).ConfigureAwait(false);

                    case "DELETECOOKIES" when access >= EAccess.Master:
                    case "DC" when access >= EAccess.Master:
                        return await Core.Command.ResponseDeleteCoolies(Utilities.GetArgsAsText(args, 1, ",")).ConfigureAwait(false);

                    case "COOKIESSTATUS" when access >= EAccess.Master:
                    case "CS" when access >= EAccess.Master:
                        return await Core.Command.ResponseBotStatus(Utilities.GetArgsAsText(args, 1, ",")).ConfigureAwait(false);

                    case "UPDATECOOKIES" when access >= EAccess.Master:
                    case "UC" when access >= EAccess.Master:
                        return await Core.Command.ResponseUpdateCoolies(Utilities.GetArgsAsText(message, 1)).ConfigureAwait(false);

                    case "UPDATECOOKIESBOT" when argLength >= 3 && access >= EAccess.Master:
                    case "UCB" when argLength >= 3 && access >= EAccess.Master:
                        return await Core.Command.ResponseUpdateCoolies(args[1], Utilities.GetArgsAsText(message, 2)).ConfigureAwait(false);

                    default:
                        return null;
                }
        }
    }

    /// <summary>
    /// 处理命令事件
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="access"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <param name="steamId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<string?> OnBotCommand(Bot bot, EAccess access, string message, string[] args, ulong steamId = 0)
    {
        if (!Enum.IsDefined(access))
        {
            throw new InvalidEnumArgumentException(nameof(access), (int)access, typeof(EAccess));
        }

        try
        {
            return await ResponseCommand(bot, access, message, args).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            string version = await bot.Commands.Response(EAccess.Owner, "VERSION").ConfigureAwait(false) ?? "Unknown";
            var i = version.LastIndexOf('V');
            if (i >= 0)
            {
                version = version[++i..];
            }
            string cfg = JsonConvert.SerializeObject(Config, Formatting.Indented);

            StringBuilder sb = new();
            sb.AppendLine(Langs.ErrorLogTitle);
            sb.AppendLine(Static.Line);
            sb.AppendLine(string.Format(Langs.ErrorLogOriginMessage, message));
            sb.AppendLine(string.Format(Langs.ErrorLogAccess, access.ToString()));
            sb.AppendLine(string.Format(Langs.ErrorLogASFVersion, version));
            sb.AppendLine(string.Format(Langs.ErrorLogPluginVersion, Utils.MyVersion));
            sb.AppendLine(Static.Line);
            sb.AppendLine(cfg);
            sb.AppendLine(Static.Line);
            sb.AppendLine(string.Format(Langs.ErrorLogErrorName, ex.GetType()));
            sb.AppendLine(string.Format(Langs.ErrorLogErrorMessage, ex.Message));
            sb.AppendLine(ex.StackTrace);

            _ = Task.Run(async () =>
            {
                await Task.Delay(500).ConfigureAwait(false);
                sb.Insert(0, '\n');
                Utils.Logger.LogGenericError(sb.ToString());
            }).ConfigureAwait(false);

            return sb.ToString();
        }
    }

    /// <summary>
    /// 收到报价
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="tradeOffer"></param>
    /// <returns></returns>
    public Task<bool> OnBotTradeOffer(Bot bot, TradeOffer tradeOffer)
    {
        if (Utils.BuffCookies.ContainsKey(bot.BotName))
        {
            Core.Handler.AddTradeCache(bot, tradeOffer);
        }
        return Task.FromResult(false);
    }

    /// <summary>
    /// 报价完成
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="tradeResults"></param>
    /// <returns></returns>
    public Task OnBotTradeOfferResults(Bot bot, IReadOnlyCollection<ParseTradeResult> tradeResults)
    {
        return Task.CompletedTask;
    }

    public Task OnBotDisconnected(Bot bot, SteamKit2.EResult reason)
    {
        if (Utils.BuffCookies.ContainsKey(bot.BotName))
        {
            Core.Handler.ClearTradeCache(bot);
        }
        return Task.CompletedTask;
    }

    public Task OnBotLoggedOn(Bot bot)
    {
        if (Utils.BuffCookies.ContainsKey(bot.BotName))
        {
            Core.Handler.InitTradeCache(bot);
        }
        return Task.CompletedTask;
    }
}

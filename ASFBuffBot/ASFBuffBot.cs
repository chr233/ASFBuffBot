using ArchiSteamFarm.Core;
using ArchiSteamFarm.Plugins.Interfaces;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Data;
using ASFBuffBot.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamKit2;
using System.ComponentModel;
using System.Composition;
using System.Reflection;
using System.Text;

namespace ASFBuffBot;

[Export(typeof(IPlugin))]
internal sealed class ASFBuffBot : IASF, IBotCommand2, IBotConnection, IBotTradeOffer
{
    public string Name => "ASF Buff Bot";
    public Version Version => MyVersion;

    private AdapterBtidge? ASFEBridge = null;

    [JsonProperty]
    public static PluginConfig Config => Utils.Config;

    private Timer? BuffTimer;

    private Timer? StatisticTimer;

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
                        Utils.ASFLogger.LogGenericException(ex);
                    }
                }
            }
        }

        Utils.Config = config ?? new();

        var warning = new StringBuilder();

        //统计
        if (Config.Statistic)
        {
            var request = new Uri("https://asfe.chrxw.com/asfbuffbot");
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

        //每轮检测间隔
        if (Config.BuffCheckInterval < 30)
        {
            Config.BuffCheckInterval = 30;
            warning.AppendLine(Static.Line);
            warning.AppendLine(Langs.BuffCheckIntervalWarn);
            warning.AppendLine(Static.Line);
        }

        //每个机器人检测间隔
        if (Config.BotInterval < 5)
        {
            Config.BotInterval = 5;
            warning.AppendLine(Static.Line);
            warning.AppendLine(Langs.BotIntervalWarn);
            warning.AppendLine(Static.Line);
        }

        var succ = await Utils.LoadFile().ConfigureAwait(false);
        if (!succ || Utils.BuffBotStorage.Count == 0)
        {
            warning.AppendLine(Static.Line);
            warning.AppendLine(Langs.BuffBotNotSetWarn1);
            warning.AppendLine(Langs.BuffBotNotSetWarn2);
            warning.AppendLine(Static.Line);
        }

        if (string.IsNullOrEmpty(Config.CustomUserAgent))
        {
            Config.CustomUserAgent = Static.DefaultUserAgent;
        }

        if (warning.Length > 0)
        {
            warning.Insert(0, Environment.NewLine);
            Utils.ASFLogger.LogGenericWarning(warning.ToString());
        }

        BuffTimer = new Timer(
           Core.Handler.OnBuffTimer,
           null,
           TimeSpan.FromSeconds(30),
           TimeSpan.FromSeconds(Config.BuffCheckInterval)
        );

        Utils.ASFLogger.LogGenericInfo(Langs.BuffCheckWillStartIn30);

        //注册Buff Service
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(1000).ConfigureAwait(false);
                await Core.ReflectionHelper.AddBuffService().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Utils.ASFLogger.LogGenericException(ex, Langs.RegisterBuffServiceFailed);
            }
        });
    }

    /// <summary>
    /// 插件加载事件
    /// </summary>
    /// <returns></returns>
    public Task OnLoaded()
    {
        try
        {
            var flag = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var handler = typeof(ASFBuffBot).GetMethod(nameof(ResponseCommand), flag);

            const string pluginName = nameof(ASFBuffBot);
            const string cmdPrefix = "ABB";
            const string repoName = "ASFBuffBot";

            ASFEBridge = AdapterBtidge.InitAdapter(pluginName, cmdPrefix, repoName, handler);
            ASF.ArchiLogger.LogGenericDebug(ASFEBridge != null ? "ASFEBridge 注册成功" : "ASFEBridge 注册失败");
        }
        catch (Exception ex)
        {
            ASF.ArchiLogger.LogGenericDebug("ASFEBridge 注册出错");
            ASF.ArchiLogger.LogGenericException(ex);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// 处理命令
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="access"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static Task<string?>? ResponseCommand(Bot bot, EAccess access, string cmd, string message, string[] args)
    {
        int argLength = args.Length;
        return argLength switch
        {
            0 => throw new InvalidOperationException(nameof(args)),
            1 => cmd switch  //不带参数
            {
                //Core
                "ENABLEBUFF" or
                "EB" when access >= EAccess.Master =>
                    Core.Command.ResponseEnableBuffBot(bot),

                "DISABLEBUFF" or
                "DB" when access >= EAccess.Master =>
                    Core.Command.ResponseDisableBuffBot(bot),

                "BUFFSTATUS" or
                "BS" when access >= EAccess.Master =>
                    Core.Command.ResponseBotStatus(bot),

                //Update
                "ASFBUFFBOT" or
                "ABB" when access >= EAccess.FamilySharing =>
                    Task.FromResult(Update.Command.ResponseASFBuffBotVersion()),

                _ => null,
            },
            _ => cmd switch //带参数
            {
                //Core
                "ENABLEBUFF" or
                "EB" when access >= EAccess.Master =>
                     Core.Command.ResponseEnableBuffBot(Utilities.GetArgsAsText(args, 1, ",")),

                "DISABLEBUFF" or
                "DB" when access >= EAccess.Master =>
                     Core.Command.ResponseDisableBuffBot(Utilities.GetArgsAsText(args, 1, ",")),

                "BUFFSTATUS" or
                "BS" when access >= EAccess.Master =>
                     Core.Command.ResponseBotStatus(Utilities.GetArgsAsText(args, 1, ",")),

                "UPDATECOOKIESBOT" or
                "UCB" when argLength >= 3 && access >= EAccess.Master =>
                     Core.Command.ResponseUpdateCoolies(args[1], Utilities.GetArgsAsText(message, 2)),

                "VERIFYCODE" or
                "VC" when argLength >= 3 && access >= EAccess.Master =>
                     Core.Command.ResponseVerifyCode(args[1], args[2]),

                _ => null,
            }
        };
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
        if (ASFEBridge != null)
        {
            return null;
        }

        if (!Enum.IsDefined(access))
        {
            throw new InvalidEnumArgumentException(nameof(access), (int)access, typeof(EAccess));
        }

        try
        {
            var cmd = args[0].ToUpperInvariant();

            if (cmd.StartsWith("DEMO."))
            {
                cmd = cmd[5..];
            }

            var task = ResponseCommand(bot, access, cmd, message, args);
            if (task != null)
            {
                return await task.ConfigureAwait(false);
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(500).ConfigureAwait(false);
                Utils.ASFLogger.LogGenericException(ex);
            }).ConfigureAwait(false);

            return ex.StackTrace;
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
        if (Utils.BuffBotStorage.ContainsKey(bot.BotName))
        {
            Core.Handler.AddTradeCache(bot, tradeOffer);
        }
        return Task.FromResult(false);
    }

    /// <summary>
    /// Steam上线
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    public Task OnBotLoggedOn(Bot bot)
    {
        if (Utils.BuffBotStorage.TryGetValue(bot.BotName, out var storage))
        {
            Core.Handler.InitTradeCache(bot);

            var currentCookies = bot.ArchiWebHandler.WebBrowser.GetBuffCookies();
            if (string.IsNullOrEmpty(currentCookies) && !string.IsNullOrEmpty(storage.Cookies))
            {
                bot.ArchiWebHandler.WebBrowser.SetBuffCookies(storage.Cookies);
            }
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Steam离线
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="reason"></param>
    /// <returns></returns>
    public async Task OnBotDisconnected(Bot bot, EResult reason)
    {
        if (Utils.BuffBotStorage.TryGetValue(bot.BotName, out var storage))
        {
            Core.Handler.ClearTradeCache(bot);
            storage.Cookies = bot.ArchiWebHandler.WebBrowser.GetBuffCookies();
            await Utils.SaveFile().ConfigureAwait(false);
        }
    }
}

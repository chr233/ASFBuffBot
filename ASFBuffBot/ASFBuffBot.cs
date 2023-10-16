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

    private bool ASFEBridge;

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
                if (configProperty == "ASFEnhance" && configValue.Type == JTokenType.Object)
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
                        ASFLogger.LogGenericException(ex);
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

        //每轮检测间隔
        if (Config.BuffCheckInterval < 30)
        {
            Config.BuffCheckInterval = 30;
            warning.AppendLine(Langs.Line);
            warning.AppendLine(Langs.BuffCheckIntervalWarn);
            warning.AppendLine(Langs.Line);
        }

        //每个机器人检测间隔
        if (Config.BotInterval < 5)
        {
            Config.BotInterval = 5;
            warning.AppendLine(Langs.Line);
            warning.AppendLine(Langs.BotIntervalWarn);
            warning.AppendLine(Langs.Line);
        }

        var succ = await LoadFile().ConfigureAwait(false);
        if (!succ || BuffBotStorage.Count == 0)
        {
            warning.AppendLine(Langs.Line);
            warning.AppendLine(Langs.BuffBotNotSetWarn1);
            warning.AppendLine(Langs.BuffBotNotSetWarn2);
            warning.AppendLine(Langs.Line);
        }

        if (string.IsNullOrEmpty(Config.CustomUserAgent))
        {
            Config.CustomUserAgent = null;
        }

        if (warning.Length > 0)
        {
            warning.Insert(0, Environment.NewLine);
            ASFLogger.LogGenericWarning(warning.ToString());
        }

        BuffTimer = new Timer(
           Core.Handler.OnBuffTimer,
           null,
           TimeSpan.FromSeconds(30),
           TimeSpan.FromSeconds(Config.BuffCheckInterval)
        );

        ASFLogger.LogGenericInfo(Langs.BuffCheckWillStartIn30);

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
                ASFLogger.LogGenericException(ex, Langs.RegisterBuffServiceFailed);
            }
        });
    }

    /// <summary>
    /// 插件加载事件
    /// </summary>
    /// <returns></returns>
    public Task OnLoaded()
    {
        ASFLogger.LogGenericInfo(Langs.PluginContact);
        ASFLogger.LogGenericInfo(Langs.PluginInfo);

        var flag = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        var handler = typeof(ASFBuffBot).GetMethod(nameof(ResponseCommand), flag);

        const string pluginName = nameof(ASFBuffBot);
        const string cmdPrefix = "ABB";
        const string repoName = "ASFBuffBot";

        ASFEBridge = AdapterBtidge.InitAdapter(pluginName, cmdPrefix, repoName, handler);

        if (ASFEBridge)
        {
            ASFLogger.LogGenericDebug(Langs.ASFEnhanceRegisterSuccess);
        }
        else
        {
            ASFLogger.LogGenericInfo(Langs.ASFEnhanceRegisterFailed);
            ASFLogger.LogGenericWarning(Langs.PluginStandalongMode);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取插件信息
    /// </summary>
    private static string? PluginInfo => string.Format("{0} {1}", nameof(ASFBuffBot), MyVersion);

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
                //Plugin Info
                "ASFBUFFBOT" or
                "ABB" when access >= EAccess.FamilySharing =>
                    Task.FromResult(PluginInfo),

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
        if (ASFEBridge)
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

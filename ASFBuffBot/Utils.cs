using ArchiSteamFarm.Core;
using ArchiSteamFarm.NLog;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Integration;
using ArchiSteamFarm.Web;
using ASFBuffBot.Data;
using System.Text.Json.Serialization;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace ASFBuffBot;

internal static class Utils
{
    /// <summary>
    /// 插件配置
    /// </summary>
    internal static PluginConfig Config { get; set; } = new();

    /// <summary>
    /// BuffCookies
    /// </summary>
    internal static Dictionary<string, BotStorage> BuffBotStorage { get; private set; } = [];

    internal static HashSet<string> PaddingBots { get; set; } = [];

    /// <summary>
    /// 更新已就绪
    /// </summary>
    internal static bool UpdatePadding { get; set; }

    /// <summary>
    /// 更新标记
    /// </summary>
    /// <returns></returns>
    private static string UpdateFlag()
    {
        if (UpdatePadding)
        {
            return "*";
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// 格式化返回文本
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    internal static string FormatStaticResponse(string message)
    {
        string flag = UpdateFlag();

        return $"<ABB{flag}> {message}";
    }

    /// <summary>
    /// 格式化返回文本
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    internal static string FormatBotResponse(this Bot bot, string message)
    {
        string flag = UpdateFlag();

        return $"<{bot.BotName}{flag}> {message}";
    }

    /// <summary>
    /// 获取Cookies文件路径
    /// </summary>
    /// <returns></returns>
    internal static string GetFilePath()
    {
        string pluginFolder = Path.GetDirectoryName(MyLocation) ?? ".";
        string cookieFilePath = Path.Combine(pluginFolder, "BuffBots.json");
        return cookieFilePath;
    }

    /// <summary>
    /// 读取Cookies
    /// </summary>
    /// <returns></returns>
    internal static async Task<bool> LoadFile()
    {
        try
        {
            string cookieFilePath = GetFilePath();
            using var fs = File.Open(cookieFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);
            string? raw = await sr.ReadLineAsync().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(raw))
            {
                var encStorage = JsonSerializer.Deserialize<Dictionary<string, BotStorage>>(raw);

                BuffBotStorage = [];
                if (encStorage != null)
                {
                    foreach (var (botName, storage) in encStorage)
                    {
                        var cookies = storage.Cookies;
                        if (!string.IsNullOrEmpty(cookies))
                        {
                            try
                            {
                                cookies = Encoding.UTF8.GetString(Convert.FromBase64String(cookies));
                            }
                            catch (Exception ex)
                            {
                                ASFLogger.LogGenericException(ex);
                            }
                        }
                        else
                        {
                            cookies = null;
                        }
                        storage.Cookies = cookies;

                        BuffBotStorage.Add(botName, storage);
                    }

                    return true;
                }
            }
            await SaveFile().ConfigureAwait(false);
            return false;
        }
        catch (Exception)
        {
            ASFLogger.LogGenericError(Langs.ReadCookiesFailed);
            await SaveFile().ConfigureAwait(false);
            return false;
        }
    }

    /// <summary>
    /// 写入Cookies
    /// </summary>
    /// <returns></returns>
    internal static async Task<bool> SaveFile()
    {
        try
        {
            var encStorage = new Dictionary<string, BotStorage>();
            foreach (var (botName, storage) in BuffBotStorage)
            {
                var cookies = storage.Cookies;
                if (!string.IsNullOrEmpty(cookies))
                {
                    cookies = Convert.ToBase64String(Encoding.UTF8.GetBytes(cookies));
                }
                else
                {
                    cookies = null;
                }

                var enc = new BotStorage
                {
                    //Enabled = storage.Enabled,
                    Cookies = cookies,
                };
                encStorage.Add(botName, enc);
            }

            string cookieFilePath = GetFilePath();
            using var fs = File.Open(cookieFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            using var sw = new StreamWriter(fs);
            string json = JsonSerializer.Serialize(encStorage);
            await sw.WriteAsync(json).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            ASFLogger.LogGenericException(ex, Langs.WriteCookiesFailed);
            return false;
        }
    }

    /// <summary>
    /// 获取版本号
    /// </summary>
    internal static Version MyVersion => Assembly.GetExecutingAssembly().GetName().Version ?? new Version("0");

    /// <summary>
    /// 获取插件所在路径
    /// </summary>
    internal static string MyLocation => Assembly.GetExecutingAssembly().Location;

    /// <summary>
    /// Steam商店链接
    /// </summary>
    internal static Uri SteamStoreURL => ArchiWebHandler.SteamStoreURL;

    /// <summary>
    /// Steam社区链接
    /// </summary>
    internal static Uri SteamCommunityURL => ArchiWebHandler.SteamCommunityURL;

    /// <summary>
    /// Buff链接
    /// </summary>
    internal static Uri BuffUrl => new("https://buff.163.com/");


    /// <summary>
    /// 日志
    /// </summary>
    internal static ArchiLogger ASFLogger => ASF.ArchiLogger;

    internal static string GetBuffCookies(this WebBrowser webBrowser)
    {
        var cookiesCollection = webBrowser.CookieContainer.GetCookies(BuffUrl);
        var sb = new StringBuilder();
        foreach (var cookies in cookiesCollection.ToList())
        {
            sb.Append(string.Format("{0}={1}; ", cookies.Name, cookies.Value));
        }
        return sb.ToString();
    }

    internal static void SetBuffCookies(this WebBrowser webBrowser, string cookies)
    {
        webBrowser.CookieContainer.SetCookies(BuffUrl, cookies);
    }
}

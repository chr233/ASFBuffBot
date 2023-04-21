using ASFBuffBot.Data;
using System.IO.Compression;
using System.Text;

namespace ASFBuffBot.Update;

internal static class Command
{
    /// <summary>
    /// 查看插件版本
    /// </summary>
    /// <returns></returns>
    internal static string ResponseASFBuffBotVersion()
    {
        return Utils.FormatStaticResponse(string.Format(Langs.PluginVer, nameof(ASFBuffBot), Utils.MyVersion.ToString()));
    }

    /// <summary>
    /// 获取插件最新版本
    /// </summary>
    /// <returns></returns>
    internal static async Task<string?> ResponseCheckLatestVersion()
    {
        GitHubReleaseResponse? response = await WebRequest.GetLatestRelease(true).ConfigureAwait(false);

        if (response == null)
        {
            return Utils.FormatStaticResponse(Langs.GetReleaseInfoFailed);
        }

        StringBuilder sb = new();
        sb.AppendLine(Utils.FormatStaticResponse(Langs.MultipleLineResult));

        sb.AppendLine(string.Format(Langs.ASFECurrentVersion, Utils.MyVersion.ToString()));
        sb.AppendLine(string.Format(Langs.ASFEOnlineVersion, response.TagName));
        sb.AppendLine(string.Format(Langs.Detail, response.Body));
        sb.AppendLine(Langs.Assert);

        foreach (var asset in response.Assets)
        {
            sb.AppendLine(string.Format(Langs.SubName, asset.Name));
        }

        sb.AppendLine(Langs.UpdateTips);

        return sb.ToString();
    }

    /// <summary>
    /// 自动更新插件
    /// </summary>
    /// <returns></returns>
    internal static async Task<string?> ResponseUpdatePlugin()
    {
        var releaseResponse = await WebRequest.GetLatestRelease(true).ConfigureAwait(false);

        if (releaseResponse == null)
        {
            return Utils.FormatStaticResponse(Langs.GetReleaseInfoFailed);
        }

        if (Utils.MyVersion.ToString() == releaseResponse.TagName)
        {
            return Utils.FormatStaticResponse(Langs.AlreadyLatest);
        }

        string langVersion = Langs.CurrentLanguage;
        string? downloadUrl = null;

        foreach (var asset in releaseResponse.Assets)
        {
            if (asset.Name.Contains(langVersion))
            {
                downloadUrl = asset.DownloadUrl;
                break;
            }
        }

        if (string.IsNullOrEmpty(downloadUrl) && releaseResponse.Assets.Any())
        {
            downloadUrl = releaseResponse.Assets?.First().DownloadUrl;
        }

        var binResponse = await WebRequest.DownloadRelease(downloadUrl).ConfigureAwait(false);

        if (binResponse == null)
        {
            return Utils.FormatStaticResponse(Langs.DownloadFailed);
        }

        var zipBytes = binResponse?.Content as byte[] ?? binResponse?.Content?.ToArray();

        if (zipBytes == null)
        {
            return Utils.FormatStaticResponse(Langs.DownloadFailed);
        }

        MemoryStream ms = new(zipBytes);

        try
        {
            await using (ms.ConfigureAwait(false))
            {
                using ZipArchive zipArchive = new(ms);

                string currentPath = Utils.MyLocation ?? ".";
                string pluginFolder = Path.GetDirectoryName(currentPath) ?? ".";
                string backupPath = Path.Combine(pluginFolder, $"{nameof(ASFBuffBot)}.bak");

                File.Move(currentPath, backupPath, true);

                foreach (var entry in zipArchive.Entries)
                {
                    if (entry.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        entry.ExtractToFile(currentPath, true);
                        Utils.UpdatePadding = true;

                        StringBuilder sb = new();
                        sb.AppendLine(Langs.UpdateSuccess);

                        sb.AppendLine();
                        sb.AppendLine(string.Format(Langs.ASFECurrentVersion, Utils.MyVersion.ToString()));
                        sb.AppendLine(string.Format(Langs.ASFEOnlineVersion, releaseResponse.TagName));
                        sb.AppendLine(string.Format(Langs.Detail, releaseResponse.Body));

                        return sb.ToString();
                    }
                }
                File.Move(backupPath, currentPath);
                return Langs.UpdateFiledWithZip;
            }
        }
        catch (Exception e)
        {
            Utils.Logger.LogGenericException(e);
            return Utils.FormatStaticResponse(Langs.UpdateFiledWithZip);
        }
    }
}

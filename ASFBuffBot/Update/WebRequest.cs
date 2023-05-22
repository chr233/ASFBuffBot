using ArchiSteamFarm.Core;
using ArchiSteamFarm.Web.Responses;
using ASFBuffBot.Data;

namespace ASFBuffBot.Update;

internal static class WebRequest
{
    /// <summary>
    /// 获取最新的发行版
    /// </summary>
    /// <returns></returns>
    internal static async Task<GitHubReleaseResponse?> GetLatestRelease(bool useMirror = true)
    {
        Uri request = new(
            useMirror ? "https://hub.chrxw.com/ASFBuffBot/releases/latest" : "https://api.github.com/repos/chr233/ASFBuffBot/releases/latest"
        );
        var response = await ASF.WebBrowser!.UrlGetToJsonObject<GitHubReleaseResponse>(request).ConfigureAwait(false);

        if (response == null && useMirror)
        {
            return await GetLatestRelease(false).ConfigureAwait(false);
        }

        return response?.Content;
    }

    /// <summary>
    /// 下载发行版
    /// </summary>
    /// <param name="downloadUrl"></param>
    /// <returns></returns>
    internal static async Task<BinaryResponse?> DownloadRelease(string? downloadUrl)
    {
        if (string.IsNullOrEmpty(downloadUrl))
        {
            return null;
        }

        Uri request = new(downloadUrl);
        BinaryResponse? response = await ASF.WebBrowser!.UrlGetToBinary(request).ConfigureAwait(false);
        return response;
    }

    /// <summary>
    /// 统计使用率
    /// </summary>
    /// <param name="_"></param>
    internal static async void OnStatisticTimer(object? _ = null)
    {
        var version = Utils.MyVersion.ToString();
        var request = new Uri($"https://asfe.chrxw.com/asfbuffbot/{version}");
        await ASF.WebBrowser!.UrlGetToHtmlDocument(request).ConfigureAwait(false);
    }
}

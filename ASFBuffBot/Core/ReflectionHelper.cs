#pragma warning disable CS8600
#pragma warning disable CS8602

using ArchiSteamFarm.Core;
using ArchiSteamFarm.Helpers;
using ArchiSteamFarm.Plugins;
using ArchiSteamFarm.Steam.Integration;
using System.Collections.Immutable;
using System.Reflection;

namespace ASFBuffBot.Core;

internal static class ReflectionHelper
{
    internal static async Task SetBuffService()
    {
        var type = Type.GetType("ArchiSteamFarm.Core.ASF,ArchiSteamFarm");
        var filedInfo = type.GetField("WebLimitingSemaphores", BindingFlags.Static);

        if (filedInfo != null)
        {
            var newValue = new Dictionary<Uri, (ICrossProcessSemaphore RateLimitingSemaphore, SemaphoreSlim OpenConnectionsSemaphore)>(5);

            var oldValue = (ImmutableDictionary<Uri, (ICrossProcessSemaphore RateLimitingSemaphore, SemaphoreSlim OpenConnectionsSemaphore)>)filedInfo.GetValue(null);


            foreach (var (k, v) in oldValue)
            {
                newValue.Add(k, v);
            }

            newValue.Add(Utils.BuffUrl, (await PluginsCore.GetCrossProcessSemaphore($"{nameof(ArchiWebHandler)}-{nameof(Utils.BuffUrl)}").ConfigureAwait(false), new SemaphoreSlim(5, 5)));

            filedInfo.SetValue(null, newValue);
        }
        else
        {
            type = typeof(ASF);
            filedInfo = type.GetField("WebLimitingSemaphores", BindingFlags.Static);
            Utils.Logger.LogGenericWarning("fieldinfo is null");
        }
    }
}

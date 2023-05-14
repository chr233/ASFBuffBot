#pragma warning disable CS8600
using ArchiSteamFarm.Core;
using ArchiSteamFarm.Helpers;
using ArchiSteamFarm.Plugins;
using ArchiSteamFarm.Steam.Integration;
using System.Collections.Immutable;
using System.Reflection;

namespace ASFBuffBot.Core;

internal static class ReflectionHelper
{
    internal static async Task AddBuffService()
    {
        var type = typeof(ASF);
        var filedInfo = type.GetProperty("WebLimitingSemaphores", BindingFlags.NonPublic | BindingFlags.Static);

        if (filedInfo != null)
        {
            var newValue = new Dictionary<Uri, (ICrossProcessSemaphore RateLimitingSemaphore, SemaphoreSlim OpenConnectionsSemaphore)>(5);
            var oldValue = (ImmutableDictionary<Uri, (ICrossProcessSemaphore RateLimitingSemaphore, SemaphoreSlim OpenConnectionsSemaphore)>)filedInfo.GetValue(null);

            if (oldValue != null)
            {
                foreach (var (k, v) in oldValue)
                {
                    newValue.Add(k, v);
                }
            }

            newValue.Add(Utils.BuffUrl, (await PluginsCore.GetCrossProcessSemaphore($"{nameof(ArchiWebHandler)}-{nameof(Utils.BuffUrl)}").ConfigureAwait(false), new SemaphoreSlim(5, 5)));

            filedInfo.SetValue(null, newValue.ToImmutableDictionary());
        }
        else
        {
            Utils.Logger.LogGenericWarning("Fieldinfo is null");
        }
    }
}

using System.Reflection;

namespace ASFBuffBot;
internal static class AdapterBtidge
{
    public static bool InitAdapter(string pluginName, string? cmdPrefix, string? repoName, MethodInfo? cmdHandler)
    {
        try
        {
            var adapterEndpoint = Assembly.Load("ASFEnhance").GetType("ASFEnhance._Adapter_.Endpoint");
            var registerModule = adapterEndpoint?.GetMethod("RegisterModule", BindingFlags.Static | BindingFlags.Public);
            var pluinVersion = Assembly.GetExecutingAssembly().GetName().Version;

            if (registerModule != null && adapterEndpoint != null)
            {
                var result = registerModule?.Invoke(null, new object?[] { pluginName, cmdPrefix, repoName, pluinVersion, cmdHandler });

                if (result is string str)
                {
                    if (str == pluginName)
                    {
                        return true;
                    }
                    else
                    {
                        ASFLogger.LogGenericWarning(str);
                    }
                }
            }
        }
#if DEBUG
        catch (Exception ex)
        {
            ASFLogger.LogGenericException(ex, "Community with ASFEnhance failed");
        }
#else
        catch (Exception)
        {
            ASFLogger.LogGenericDebug("Community with ASFEnhance failed");
        }
#endif
        return false;
    }
}

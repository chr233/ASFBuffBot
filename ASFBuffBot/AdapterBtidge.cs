using System.Reflection;

namespace ASFBuffBot;
internal sealed class AdapterBtidge
{
    private Type Endpoint { get; set; } = null!;

    private AdapterBtidge(Type endpoint)
    {
        Endpoint = endpoint;
    }

    public static AdapterBtidge? InitAdapter(string pluginName, string? cmdPrefix, string? repoName, MethodInfo? cmdHandler)
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
                        return new AdapterBtidge(adapterEndpoint);
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
        return null;
    }
}

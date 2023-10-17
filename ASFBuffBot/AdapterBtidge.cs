using System.Reflection;

namespace ASFBuffBot;
internal static class AdapterBtidge
{
    /// <summary>
    /// 注册子模块
    /// </summary>
    /// <param name="pluginName">插件名称</param>
    /// <param name="pluginIdentity">插件唯一标识符</param>
    /// <param name="cmdPrefix">命令前缀</param>
    /// <param name="repoName">自动更新仓库</param>
    /// <param name="cmdHandler">命令处理函数</param>
    /// <returns></returns>
    public static bool InitAdapter(string pluginName, string pluginIdentity, string? cmdPrefix, string? repoName, MethodInfo? cmdHandler)
    {
        try
        {
            var adapterEndpoint = Assembly.Load("ASFEnhance").GetType("ASFEnhance._Adapter_.Endpoint");
            var registerModule = adapterEndpoint?.GetMethod("RegisterModule", BindingFlags.Static | BindingFlags.Public);
            var pluinVersion = Assembly.GetExecutingAssembly().GetName().Version;

            if (registerModule != null && adapterEndpoint != null)
            {
                var result = registerModule?.Invoke(null, new object?[] { pluginName, pluginIdentity, cmdPrefix, repoName, pluinVersion, cmdHandler });

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

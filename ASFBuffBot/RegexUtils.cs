using System.Text.RegularExpressions;

namespace ASFBuffBot;

internal static partial class RegexUtils
{
    [GeneratedRegex(@"(?:https?:\/\/steamcommunity\.com\/tradeoffer\/new\/\?)?partner=(\d+)&token=(\S+)")]
    public static partial Regex MatchTradeLink();
}

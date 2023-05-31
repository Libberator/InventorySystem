using System.Collections.Generic;

namespace TooltipSystem
{
    public static class LinkLookup
    {
        // the string Key used for a lookup should be the same as the linkID you use for Rich Text linking
        private static readonly Dictionary<string, IHaveTooltip> _tooltipProviders = new();

        public static void AddAsTooltipProvider(IHaveTooltip tooltipSource, string linkID) =>
            _tooltipProviders[linkID] = tooltipSource;

        public static IHaveTooltip GetProviderForLink(string linkID)
        {
            if (_tooltipProviders.TryGetValue(linkID, out var provider))
                return provider;
            return null;
        }

        public static string GetTooltipFor(string linkID)
        {
            if (_tooltipProviders.TryGetValue(linkID, out var provider))
                return provider.GetTooltipText();
            return string.Empty;
        }
    }
}

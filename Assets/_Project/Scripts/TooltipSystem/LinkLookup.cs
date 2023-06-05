using System.Collections.Generic;

namespace TooltipSystem
{
    public static class LinkLookup
    {
        // the string Key used for a lookup should be the same as the linkID you use for Rich Text linking
        private static readonly Dictionary<string, IHaveTooltip> _linkSources = new();

        public static void AddAsLinkSource(IHaveTooltip tooltipSource, string linkID) =>
            _linkSources[linkID] = tooltipSource;

        public static void RemoveLinkSource(IHaveTooltip tooltipSource, string linkID)
        {
            if (_linkSources.ContainsKey(linkID) && _linkSources[linkID] == tooltipSource)
                _linkSources.Remove(linkID);
        }

        public static IHaveTooltip GetProviderForLink(string linkID)
        {
            if (_linkSources.TryGetValue(linkID, out var provider))
                return provider;
            return null;
        }
    }
}

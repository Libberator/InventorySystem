using UnityEngine;

namespace Utilities
{
    public static class ColorUtils
    {
        public static string ToHex(this Color color, bool includeAlpha = false) =>
           $"#{(includeAlpha ? ColorUtility.ToHtmlStringRGBA(color) : ColorUtility.ToHtmlStringRGB(color))}";

        public static Color WithAlphaAt(this Color color, float a) => new(color.r, color.g, color.b, a);
    }
}

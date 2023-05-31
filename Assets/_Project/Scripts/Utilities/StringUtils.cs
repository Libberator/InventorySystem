using UnityEngine;

namespace Utilities
{
    public static class StringUtils
    {
        public const string Alphanumeric = "0123456789abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ";
        public const string alphanumeric = "0123456789abcdefghijklmnopqrstuvxywz";
        public const string AlphabetUpper = "ABCDEFGHIJKLMNOPQRSTUVXYWZ";
        public const string AlphabetLower = "abcdefghijklmnopqrstuvxywz";

        public static string WithColor(this string text, Color color, bool includeAlpha = false) =>
            $"<color={color.ToHex(includeAlpha)}>{text}</color>";

        public static string WithLink(this string text, string link) =>
            $"<link=\"{link}\">{text}</link>";

        public static string ToHex(this Color color, bool includeAlpha = false) =>
           $"#{(includeAlpha ? ColorUtility.ToHtmlStringRGBA(color) : ColorUtility.ToHtmlStringRGB(color))}";
    }
}
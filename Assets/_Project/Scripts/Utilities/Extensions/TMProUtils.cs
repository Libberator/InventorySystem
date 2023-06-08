using TMPro;
using UnityEngine.EventSystems;

namespace Utilities
{
    public static class TMProUtils
    {
        /// <summary>
        /// This detects if the cursor is over any links (unrelated to Website Links), a rich-text tagging feature: "&lt;link="<paramref name="linkID"/>"&gt;like this&lt;/link&gt;"
        /// </summary>
        public static bool IsOverLinkText(this PointerEventData eventData, TMP_Text text, out string linkID)
        {
            linkID = string.Empty;
            if (text.textInfo.linkCount == 0) return false;
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, null);

            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
                linkID = linkInfo.GetLinkID();
                return true;
            }
            return false;
        }

    }
}

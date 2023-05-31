using InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TooltipSystem
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class RichTextLinkParser : MonoBehaviour, IPointerMoveHandler, IPointerClickHandler
    {
        private TextMeshProUGUI _text;
        private void Awake() => _text = GetComponent<TextMeshProUGUI>();

        private bool PointerOverLinkText(PointerEventData eventData, out string linkID)
        {
            linkID = string.Empty;
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, eventData.position, null);

            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = _text.textInfo.linkInfo[linkIndex];
                linkID = linkInfo.GetLinkID();
                return true;
            }
            return false;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (_text.textInfo.linkCount == 0) return;

            if (PointerOverLinkText(eventData, out string linkTag))
            {
                // TODO: remove this hard-coded dependency that this TooltipSystem class has with InventorySystem
                Tooltip.ShowTooltip(ItemEntryController.Instance.DraggedItem);
                
                //Debug.Log($"Parsed Link Tag: {linkTag}");
            }
            else
            {
                Tooltip.HideTooltip();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (PointerOverLinkText(eventData, out string linkTag))
                Debug.Log($"Clicked link: {linkTag}");
        }

    }
}

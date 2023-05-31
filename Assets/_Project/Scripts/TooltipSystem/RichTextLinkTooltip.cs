using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace TooltipSystem
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class RichTextLinkTooltip : MonoBehaviour, IPointerMoveHandler, IPointerClickHandler
    {
        private TextMeshProUGUI _text;
        private bool _isHoveringOverLinkedText;

        private void Awake() => _text = GetComponent<TextMeshProUGUI>();

        public void OnPointerMove(PointerEventData eventData)
        {
            if (eventData.IsOverLinkText(_text, out string linkID))
            {
                if (!_isHoveringOverLinkedText)
                {
                    _isHoveringOverLinkedText = true;
                    Tooltip.ShowTooltip(LinkLookup.GetProviderForLink(linkID));
                }
            }
            else if (_isHoveringOverLinkedText)
            {
                _isHoveringOverLinkedText = false;
                Tooltip.HideTooltip();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.IsOverLinkText(_text, out string linkTag))
                Debug.Log($"Clicked link: {linkTag}");
        }
    }
}

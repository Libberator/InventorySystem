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
        private Tooltip _tooltip;

        private void Awake() => _text = GetComponent<TextMeshProUGUI>();

        private void Start() => _tooltip = ServiceLocator.Get<Tooltip>();

        public void OnPointerMove(PointerEventData eventData)
        {
            if (eventData.IsOverLinkText(_text, out string linkID))
            {
                if (!_isHoveringOverLinkedText)
                {
                    _isHoveringOverLinkedText = true;
                    _tooltip.ShowTooltip(LinkLookup.GetProviderForLink(linkID));
                }
            }
            else if (_isHoveringOverLinkedText)
            {
                _isHoveringOverLinkedText = false;
                _tooltip.HideTooltip();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.IsOverLinkText(_text, out string linkTag))
            {
                Debug.Log($"Clicked link: {linkTag}"); // nothing implemented lol
            }
        }
    }
}

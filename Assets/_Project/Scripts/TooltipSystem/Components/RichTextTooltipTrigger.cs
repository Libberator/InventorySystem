using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace TooltipSystem
{
    /// <summary>
    /// This uses the <link="linkID">rich text</link> feature from TMPro to have dynamic tooltips.
    /// You will need to assign a LinkProvider elsewhere to source from the LinkLookup.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class RichTextTooltipTrigger : UITooltipTrigger
    {
        private TextMeshProUGUI _text;
        private bool _isHoveringOverLink;

        protected override void Awake() => _text = GetComponent<TextMeshProUGUI>();

        // removing searching locally for a source. Source is provided in OnPointerMove
        protected override void FindSource() { }

        public override void OnPointerMove(PointerEventData eventData)
        {
            if (eventData.IsOverLinkText(_text, out string linkID))
            {
                if (!_isHoveringOverLink)
                {
                    _isHoveringOverLink = true;

                    _tooltipSource = LinkLookup.GetProviderForLink(linkID);
                    HandleTooltipAction(_onPointerMove); // Recommend "Show" for _onPointerMove
                }
            }
            else if (_isHoveringOverLink)
            {
                _isHoveringOverLink = false;
                HandleTooltipAction(_onPointerExit);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.IsOverLinkText(_text, out string linkID))
            {
                _tooltipSource = LinkLookup.GetProviderForLink(linkID);
                HandleTooltipAction(_onPointerClick);
            }
        }
    }
}

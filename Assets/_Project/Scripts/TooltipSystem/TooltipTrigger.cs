using UnityEngine;
using Utilities;

namespace TooltipSystem
{
    public enum TooltipAction
    {
        DoNothing = 0,
        Show = 1,
        Hide = 2,
    }

    public abstract class TooltipTrigger : MonoBehaviour
    {
        protected IHaveTooltip _tooltipSource;
        protected TooltipView _tooltip;

        protected virtual void Awake() => FindSource();

        protected virtual void Start() => _tooltip = ServiceLocator.Get<TooltipView>();

        protected virtual void FindSource()
        {
            if (_tooltipSource == null && !this.TryGetComponentInHierarchy(out _tooltipSource))
                Debug.LogWarning("Could not find a Source for IHaveTooltips in the hierarchy.", this);
        }

        protected void ShowTooltip() => _tooltip.ShowTooltip(_tooltipSource);

        protected void HideTooltip() => _tooltip.HideTooltip();
    }
}

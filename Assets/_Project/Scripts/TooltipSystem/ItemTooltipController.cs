using InventorySystem;
using UnityEngine;

namespace TooltipSystem
{
    /// <summary>
    /// This class handles the connections between the TooltipSystem and the InventorySystem
    /// </summary>
    public static class ItemTooltipController
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void SubscribeToEvents()
        {
            ItemEntryView.PointerEnter += ShowTooltip;
            ItemEntryView.PointerExit += HideTooltip;
            ItemEntryView.LeftClicked += HideTooltip;
            ItemEntryView.RightClicked += HideTooltip;
        }

        private static void ShowTooltip(ItemEntryView slot)
        {
            if (slot.Item != null)
                Tooltip.Instance.ShowTooltip(slot.Item);
        }

        private static void HideTooltip(ItemEntryView slot)
        {
            Tooltip.Instance.HideTooltip();
        }
    }
}

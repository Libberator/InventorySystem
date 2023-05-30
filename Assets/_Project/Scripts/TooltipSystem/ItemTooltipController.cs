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
            ItemSlot.PointerEnter += ShowTooltip;
            ItemSlot.PointerExit += HideTooltip;
            ItemSlot.LeftClicked += HideTooltip;
            ItemSlot.RightClicked += HideTooltip;
        }

        private static void ShowTooltip(ItemSlot slot)
        {
            if (slot.Item != null)
                Tooltip.Instance.ShowTooltip(slot.Item);
        }

        private static void HideTooltip(ItemSlot slot)
        {
            Tooltip.Instance.HideTooltip();
        }
    }
}

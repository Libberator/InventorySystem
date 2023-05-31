using TooltipSystem;
using UnityEngine;

namespace InventorySystem
{
    /// <summary>
    /// This class handles the connections between the TooltipSystem and the InventorySystem.
    /// The Item class also has an IHaveTooltip interface, so these are the only cross-dependencies
    /// </summary>
    public static class ItemTooltipBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapInitialization()
        {
            ItemEntryView.PointerEnter += ShowTooltip;
            ItemEntryView.PointerExit += HideTooltip;
            ItemEntryView.LeftClicked += HideTooltip;
            ItemEntryView.RightClicked += HideTooltip;
            LinkLookup.AddAsTooltipProvider(new ItemTooltipProvider(), "Item");
        }

        private static void ShowTooltip(ItemEntryView slot)
        {
            if (slot.Item != null)
                Tooltip.ShowTooltip(slot.Item);
        }

        private static void HideTooltip(ItemEntryView slot)
        {
            Tooltip.HideTooltip();
        }
    }

    // Relies on the ItemEntryController Singleton to provide the Item Tooltip info
    public class ItemTooltipProvider : IHaveTooltip
    {
        public string GetTooltipText()
        {
            return ItemEntryController.Instance.DraggedItem.GetTooltipText();
        }
    }
}

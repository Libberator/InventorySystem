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
        private static Tooltip _tooltip;
        private static IHaveTooltip _tooltipProvider;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapInitialization()
        {
            _tooltip = ServiceLocator.Get<Tooltip>();
            _tooltipProvider = new ItemTooltipProvider(ServiceLocator.Get<ItemEntryController>());
            LinkLookup.AddAsTooltipProvider(_tooltipProvider, "Item");
            
            ItemEntryView.PointerEnter += ShowTooltip;
            ItemEntryView.PointerExit += HideTooltip;
            ItemEntryView.LeftClicked += HideTooltip;
            ItemEntryView.RightClicked += HideTooltip;
        }

        private static void ShowTooltip(ItemEntryView slot)
        {
            if (slot.Item != null)
                _tooltip.ShowTooltip(slot.Item);
        }

        private static void HideTooltip(ItemEntryView slot)
        {
            _tooltip.HideTooltip();
        }
    }

    // Relies on the ItemEntryController Singleton to provide the Item Tooltip info
    public class ItemTooltipProvider : IHaveTooltip
    {
        private readonly ItemEntryController _dragger;

        public ItemTooltipProvider(ItemEntryController dragger) => _dragger = dragger;

        public string GetTooltipText() => _dragger.DraggedItem.GetTooltipText();
    }
}

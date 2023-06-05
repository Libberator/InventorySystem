using TooltipSystem;
using UnityEngine;

namespace InventorySystem
{
    [RequireComponent(typeof(ItemEntryView))]
    public class ItemEntryViewTooltipTrigger : UITooltipTrigger<ItemEntryView>
    {
        public override Tooltip GetTooltip() =>
            _target.Item != null ? _target.Item.GetTooltip() : Tooltip.Empty;
    }
}

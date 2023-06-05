using TooltipSystem;

namespace InventorySystem
{
    public class ItemEntryDraggerLinkProvider : LinkProvider<ItemEntryDragger>
    {
        public override Tooltip GetTooltip()
        {
            return _provider.Entry.Item != null ? _provider.Entry.Item.GetTooltip() : Tooltip.Empty;
        }
    }
}

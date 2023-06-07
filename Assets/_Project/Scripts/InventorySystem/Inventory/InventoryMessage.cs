namespace InventorySystem
{
    public enum InventoryEvent
    {
        None,
        ItemAddSuccess,
        ItemAddFail,
        ItemRemoveSuccess,
        ItemRemoveFail,
        ItemMoveSuccess,
        ItemMoveFail,
        ItemDiscardSuccess,
        ItemDiscardCancelled,
        ItemDiscardFail,
        
    }

    public struct InventoryMessage
    {
        public Item Item;
        public int Quantity;
        public InventoryEvent Event;

        public InventoryMessage(Item item, int quantity = -1, InventoryEvent invEvent = InventoryEvent.None)
        {
            Item = item;
            Quantity = quantity;
            Event = invEvent;
        }
    }
}

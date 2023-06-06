namespace InventorySystem
{
    public enum InventoryEvent
    {
        None,
        ItemAddSuccess,
        ItemAddFail,
        ItemMoveSuccess,
        ItemMoveFail,
        ItemDiscardSuccess,
        ItemDiscardFail, // or "Cancel" instead?
        
    }

    public struct InventoryMessage
    {
        public string Message;
        public InventoryEvent Event;

        public InventoryMessage(string message, InventoryEvent invEvent = InventoryEvent.None)
        {
            Message = message;
            Event = invEvent;
        }
    }
}

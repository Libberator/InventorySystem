using System;

namespace InventorySystem
{
    [Serializable]
    public class SlotEntry
    {
        public Item Item;
        public int Quantity = 1;

        public SlotEntry(Item item, int quantity = 1)
        {
            Item = item;
            Quantity = quantity;
        }
    }
}
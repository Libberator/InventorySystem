using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace InventorySystem
{
    [Serializable]
    [InlineProperty]
    public class ItemEntry
    {
        public event Action<Item> ItemChanged;
        public event Action<int> QuantityChanged;

        [HorizontalGroup, HideLabel]
        [SerializeField] private Item _item;
        public Item Item
        {
            get => _item;
            set
            {
                if (_item != value)
                {
                    _item = value;
                    ItemChanged?.Invoke(_item);
                }
            }
        }

        [HorizontalGroup(width: 40), HideLabel]
        [SerializeField] private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    QuantityChanged?.Invoke(_quantity);
                    if (_quantity == 0)
                        Item = null;
                }
            }
        }

        public ItemEntry(Item item, int quantity = 1)
        {
            Item = item;
            Quantity = quantity;
        }



    }
}
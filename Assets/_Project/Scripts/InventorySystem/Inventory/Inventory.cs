using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace InventorySystem
{
    [Serializable]
    public class Inventory
    {
        [SerializeField, ReadOnly] private bool _isPlayerInventory = false;
        [OnValueChanged(nameof(AdjustSize))]
        [SerializeField, ReadOnly] private int _inventorySize = 15;
        [SerializeField, ReadOnly, ListDrawerSettings(NumberOfItemsPerPage = 5)] private ItemEntry[] _items;

        public int Size => _inventorySize;
        public ItemEntry[] Items => _items;

        public int AmountOf(Item item) => _items.Where(s => s.Item == item).Sum(s => s.Quantity);
        public bool Contains(ItemEntry entry) => _items.Contains(entry);

        public Inventory(List<ItemEntry> startingItems, int inventorySize = 15, bool isPlayerInventory = false) : this(inventorySize, isPlayerInventory)
        {
            InitializeWith(startingItems);
        }

        public Inventory(int inventorySize = 15, bool isPlayerInventory = false)
        {
            AdjustSize(inventorySize);
            _isPlayerInventory = isPlayerInventory;
            if (_isPlayerInventory) ServiceLocator.Register(this);
        }

        #region Initializing Inventory

        [Button]
        public void InitializeWith(List<ItemEntry> startingEntries)
        {
            for (int i = 0; i < _inventorySize; i++)
            {
                _items[i] = new ItemEntry();
                if (startingEntries != null && i < startingEntries.Count)
                    _items[i].Set(startingEntries[i]);
            }
        }

        protected void AdjustSize(int size)
        {
            _inventorySize = size;
            if (_items == null)
            {
                _items = new ItemEntry[size];
                return;
            }
            Array.Resize(ref _items, size);
        }

        #endregion

        #region Adding & Removing Items

        public void CollectAll()
        {
            if (_isPlayerInventory) return;
            var playerInventory = ServiceLocator.Get<Inventory>();

            foreach (var slot in _items)
            {
                if (slot.Item == null) continue;
                playerInventory.TryAddItem(slot, out int remainder);
                slot.RemoveQuantity(slot.Quantity - remainder);
            }
        }

        public bool TryAddItem(ItemEntry entry, out int remainder) => TryAddItem(entry.Item, entry.Quantity, out remainder);

        public bool TryAddItem(Item item, int qty, out int remainder)
        {
            remainder = qty;

            // first try stacking
            if (item.IsStackable)
            {
                foreach (var slot in _items.Where(s => s.Item == item && s.Quantity < item.MaxStack).OrderByDescending(s => s.Quantity))
                {
                    remainder = slot.AddQuantity(qty);
                    if (remainder == 0)
                        return true;
                    qty = remainder;
                }
            }

            // find empty slots to add to
            foreach (var emptySlot in _items.Where(s => s.Item == null))
            {
                var toAdd = Math.Min(remainder, item.MaxStack);
                emptySlot.Set(item, toAdd);
                remainder -= toAdd;
                if (remainder == 0)
                    return true;
            }

            // inventory is full. no room left
            return false;
        }

        public bool TryRemoveItem(Item item, int qty, out int remainder)
        {
            remainder = qty;

            foreach (var slot in _items.Where(s => s.Item == item).OrderBy(s => s.Quantity))
            {
                remainder = slot.RemoveQuantity(qty);
                if (remainder == 0)
                    return true;
                qty = remainder;
            }

            return false;
        }

        #endregion

        #region Stacking & Sorting

        // sorts to the upper-left in the grid
        public void CombineLikeItems(Item item)
        {
            if (!item.IsStackable) return;

            var likeItemSlots = _items.Where(s => s.Item == item).Reverse().ToArray();

            for (var i = 0; i < likeItemSlots.Length - 1; i++)
            {
                // start with the bottom-right-most slot
                var current = likeItemSlots[i];

                // fill from the top-left on
                for (int j = likeItemSlots.Length - 1; j > i; j--)
                {
                    var target = likeItemSlots[j];
                    if (target.Quantity == item.MaxStack) continue;

                    current.TransferTo(target);
                    if (current.Quantity == 0) break;
                }
            }
        }

        public void AutoSort() // Equipment first, then Rarity, then by something else other than Name, then naturally descending quantity
        {
            // SortedSet of just Items,
            // combine like items (will only apply to non-stackable ones ofc)
            // now for the important part: how do we re-arrange without losing anything in the process?
            // do we just have a dictionary (SortedList?) with Item & Qty, then just refresh everything to it?
            // it should ignore whatever we're dragging, if anything, by default

            //items = items
            //.OrderBy(s => s.Item == null)
            //.ThenBy(s => s.Item == null ? "" : s.Item.DisplayName)
            //.ThenByDescending(s => s.Count)
            //.ToArray();
            //InventoryUpdated?.Invoke();
        }

        #endregion
    }
}
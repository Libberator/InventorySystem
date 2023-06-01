using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.UI;

namespace InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        [Header("Settings"), OnValueChanged(nameof(MatchSize))]
        [SerializeField, Min(0)] private int _inventorySize = 12;
        [SerializeField] private List<ItemEntry> _startingItems = new();

        [Header("References")]
        [SerializeField] private PanelAnimator _panelAnimator;
        [SerializeField] private ItemEntryView _slotPrefab;
        [SerializeField] private RectTransform _itemSlotsParent;
        [SerializeField] private ItemEntryView[] _itemSlots;

        public static event Action<Inventory> Closed;
        private bool _isOpen = false;
        public bool IsOpen => _isOpen;

        protected virtual void Awake() => InitializeInventory(_inventorySize, _startingItems);

        [Button]
        public void ToggleInventory()
        {
            if (_isOpen)
                CloseInventory();
            else
                OpenInventory();
        }

        [Button]
        public void OpenInventory()
        {
            _panelAnimator.Show();
            _isOpen = true;
        }

        [Button]
        public void CloseInventory()
        {
            _panelAnimator.Hide();
            _isOpen = false;
            Closed?.Invoke(this);
        }

        public int AmountOf(Item item) => _itemSlots.Where(s => s.Item == item).Sum(s => s.Quantity);
        public bool Contains(ItemEntryView slot) => _itemSlots.Contains(slot);

        #region Adding & Removing Items

        // move to a Chest/Loot child class?
        public void CollectAll()
        {
            var playerInventory = ServiceLocator.Get<Inventory>();
            if (playerInventory == this) return;

            foreach (var slot in _itemSlots)
            {
                if (slot.Item == null) continue;
                playerInventory.TryAddItem(slot.Entry, out int remainder);
                slot.Entry.RemoveQuantity(slot.Quantity - remainder);
            }
        }

        public bool TryAddItem(ItemEntry entry, out int remainder) => TryAddItem(entry.Item, entry.Quantity, out remainder);

        public bool TryAddItem(Item item, int qty, out int remainder)
        {
            remainder = qty;

            // first try stacking
            if (item.IsStackable)
            {
                foreach (var slot in _itemSlots.Where(s => s.Item == item && s.Quantity < item.MaxStack).OrderByDescending(s => s.Quantity))
                {
                    remainder = slot.Entry.AddQuantity(qty);
                    if (remainder == 0)
                        return true;
                    qty = remainder;
                }
            }

            // find empty slots to add to
            foreach (var emptySlot in _itemSlots.Where(s => s.Item == null))
            {
                var toAdd = Math.Min(remainder, item.MaxStack);
                emptySlot.SetEntry(item, toAdd);
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

            foreach (var slot in _itemSlots.Where(s => s.Item == item).OrderBy(s => s.Quantity))
            {
                remainder = slot.Entry.RemoveQuantity(qty);
                if (remainder == 0)
                    return true;
                qty = remainder;
            }

            return false;
        }

        #endregion

        #region Sorting

        public void CombineLikeItems(Item item)
        {
            if (!item.IsStackable) return;

            // for my implementation, it's important to sort from small to large
            var likeItemSlots = _itemSlots.Where(s => s.Item == item && s.Quantity != item.MaxStack).OrderBy(s => s.Quantity).ToArray();

            for (var i = 0; i < likeItemSlots.Length - 1; i++)
            {
                // start with the smallest slots
                var current = likeItemSlots[i];
                if (current.Quantity == item.MaxStack) continue;
                
                // fill from the largest to the smallest that's available
                for (int j = likeItemSlots.Length - 1; j > i; j--)
                {
                    var target = likeItemSlots[j];
                    if (target.Quantity == item.MaxStack) continue;

                    current.Entry.TransferTo(target.Entry);
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
        }

        #endregion

        #region Adding & Removing ItemSlots

        protected void InitializeInventory(int size, List<ItemEntry> startingEntries = null)
        {
            MatchSize(size);

            if (startingEntries != null && startingEntries.Count <= size)
            {
                for (int i = 0; i < startingEntries.Count; i++)
                {
                    _itemSlots[i].SetEntry(startingEntries[i]);
                }
            }
        }

        protected void MatchSize(int size)
        {
            var currentCount = _itemSlotsParent.childCount;
            if (currentCount < size)
                AddItemSlots(size - currentCount);
            else if (currentCount > size)
                RemoveItemSlots(currentCount - size);

            RefreshItemSlots();
        }

        protected void AddItemSlots(int qty = 1)
        {
            for (int i = 0; i < qty; i++)
            {
#if UNITY_EDITOR
                UnityEditor.PrefabUtility.InstantiatePrefab(_slotPrefab, _itemSlotsParent);
#else
                Instantiate(_slotPrefab, _itemSlotsParent);
#endif
            }
        }

        protected void RemoveItemSlots(int qty = 1)
        {
            int lowestIndex = Mathf.Max(0, _itemSlotsParent.childCount - qty);
            for (int i = _itemSlotsParent.childCount - 1; i >= lowestIndex; i--)
            {
                var childObject = _itemSlotsParent.GetChild(i).gameObject;
#if UNITY_EDITOR
                if (Application.isPlaying)
                    Destroy(childObject);
                else
                    DestroyImmediate(childObject);
#else
                Destroy(childObject);
#endif
            }
        }

        private void RefreshItemSlots() => _itemSlots = _itemSlotsParent.GetComponentsInChildren<ItemEntryView>(includeInactive: true);

        #endregion

        protected virtual void OnValidate()
        {
            if (_itemSlotsParent == null)
            {
                Debug.LogWarning("Please assign a Transform to hold ItemSlots", _itemSlotsParent);
                return;
            }

            if (_slotPrefab == null)
            {
                Debug.LogWarning("Please assign an ItemSlot prefab", _slotPrefab);
                return;
            }

            //if (_inventorySize != _itemSlotsParent.childCount)
            //{
            //    MatchSize(_inventorySize);
            //    RefreshItemSlots();
            //}
        }
    }
}
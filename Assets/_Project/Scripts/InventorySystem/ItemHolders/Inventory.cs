using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _inventorySize = 12;
        [SerializeField] private List<SlotEntry> _startingItems = new();
        
        [Header("References")]
        [SerializeField] private PanelAnimator _panelAnimator;
        [SerializeField] private ItemSlot _slotPrefab;
        [SerializeField] private RectTransform ItemSlotsParent;
        [SerializeField] private ItemSlot[] _itemSlots;
        
        public static event Action<Inventory> Closed;

        private void Awake() => InitializeInventory(_inventorySize, _startingItems);

        [Button]
        public void OpenInventory() => _panelAnimator.Show();

        [Button]
        public void CloseInventory()
        {
            _panelAnimator.Hide();
            Closed?.Invoke(this);
        }

        public int AmountOf(Item item) => _itemSlots.Where(s => s.Item == item).Sum(s => s.Quantity);
        public bool Contains(ItemSlot slot) => _itemSlots.Contains(slot);

        #region Adding & Removing ItemSlots

        public void InitializeInventory(int size, List<SlotEntry> startingEntries = null)
        {
            MatchSize(size);

            RefreshItemSlots();

            if (startingEntries != null && startingEntries.Count <= size)
            {
                for (int i = 0; i < startingEntries.Count; i++)
                {
                    _itemSlots[i].SetEntry(startingEntries[i]);
                }
            }
        }

        private void MatchSize(int size)
        {
            var currentCount = ItemSlotsParent.childCount;
            if (currentCount < size)
                AddItemSlots(size - currentCount);
            else if (currentCount > size)
                RemoveItemSlots(currentCount - size);
        }

        private void AddItemSlots(int qty = 1)
        {
            for (int i = 0; i < qty; i++)
                Instantiate(_slotPrefab, ItemSlotsParent);
        }

        private void RemoveItemSlots(int qty = 1)
        {
            int lowestIndex = Mathf.Max(0, ItemSlotsParent.childCount - qty);
            for (int i = ItemSlotsParent.childCount - 1; i >= lowestIndex; i--)
            {
                if (Application.isPlaying)
                    Destroy(ItemSlotsParent.GetChild(i).gameObject);
                else
                    DestroyImmediate(ItemSlotsParent.GetChild(i).gameObject);
            }
        }

        private void RefreshItemSlots() => _itemSlots = ItemSlotsParent.GetComponentsInChildren<ItemSlot>(includeInactive: true);

        #endregion

        #region Adding & Removing Items

        public bool TryAddItem(Item item, int qty, out int remainder)
        {
            remainder = qty;

            // first try stacking
            if (item.IsStackable)
            {
                foreach (var slot in _itemSlots.Where(s => s.CanStackItem(item)).OrderByDescending(s => s.Quantity))
                {
                    if (slot.TryAddItem(item, qty, out remainder))
                        return true;
                    qty = remainder;
                }
            }

            // find empty slot
            var emptySlot = _itemSlots.FirstOrDefault(s => s.Item == null);
            if (emptySlot != null && emptySlot.TryAddItem(item, qty, out remainder))
                return true;

            // inventory is full. no room left
            return false;
        }

        public bool TryRemoveItem(Item item, int qty, out int remainder)
        {
            remainder = qty;

            foreach (var slot in _itemSlots.Where(s => s.Item == item).OrderBy(s => s.Quantity))
            {
                if (slot.TryRemoveItem(qty, out remainder))
                    return true;
                qty = remainder;
            }

            return false;
        }

        #endregion

        private void OnValidate()
        {
            if (ItemSlotsParent == null)
            {
                Debug.LogWarning("Please assign a Transform to hold ItemSlots", ItemSlotsParent);
                return;
            }

            if (_slotPrefab == null)
            {
                Debug.LogWarning("Please assign an ItemSlot prefab", _slotPrefab);
                return;
            }

            if (_inventorySize != ItemSlotsParent.childCount)
            {
                MatchSize(_inventorySize);
                RefreshItemSlots();
            }
        }
    }
}
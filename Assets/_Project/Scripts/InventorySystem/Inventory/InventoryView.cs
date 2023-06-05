using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities.UI;

namespace InventorySystem
{
    public class InventoryView : MonoBehaviour
    {
        public static event Action<InventoryView> Opened;
        public static event Action<InventoryView> Closed;

        [Header("Settings")]
        [SerializeField, ReadOnly] private Inventory _inventory;
        public bool IsPlayerInventory => _inventory.IsPlayerInventory;

        [Header("References")]
        [SerializeField] private PanelAnimator _panelAnimator;
        [SerializeField] private ItemEntryView _slotPrefab;
        [SerializeField] private RectTransform _itemSlotsParent;
        [SerializeField, HideInInspector] private ItemEntryView[] _itemSlots;
        private ConfirmationDialog _confirmationDialog;

        private static readonly List<InventoryView> _openInventoryViews = new();

        public static Inventory GetOtherOpenInventory(ItemEntryView slot)
        {
            var openView = _openInventoryViews.Find(i => !i.Contains(slot));
            if (openView == null) return null;
            return openView._inventory;
        }

        public static Inventory GetInventoryFromItemEntry(ItemEntryView entry) =>
            _openInventoryViews.Find(v => v.Contains(entry))._inventory;

        public bool Contains(ItemEntryView slot) => Array.Exists(_itemSlots, s => s == slot);

        private bool _isOpen = false;
        public bool IsOpen
        {
            get => _isOpen;
            protected set
            {
                if (_isOpen == value) return;
                if (value)
                    Opened?.Invoke(this);
                else
                    Closed?.Invoke(this);
                _isOpen = value;
            }
        }

        protected virtual void Start()
        {
            _confirmationDialog = ServiceLocator.Get<ConfirmationDialog>();
            if (_inventory != null)
                BindTo(_inventory);
        }

        public void BindTo(Inventory inventory)
        {
            AdjustSize(inventory.Size);

            _inventory = inventory;

            SyncItems();
            //for (int i = 0; i < inventory.Size; i++)
            //    _itemSlots[i].BindTo(inventory.Items[i]);
        }

        #region Adding & Removing ItemSlots

        [Button]
        protected void AdjustSize(int size)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && UnityEditor.PrefabUtility.IsPartOfPrefabInstance(_itemSlotsParent))
            {
                Debug.LogWarning("Can't adjust item count if we're both part of a Prefab Instance and not in Play Mode.", this);
                return;
            }
#endif
            var currentCount = _itemSlotsParent.childCount;
            if (size > currentCount)
                AddItemSlots(size - currentCount);
            else if (size < currentCount)
                RemoveItemSlots(currentCount - size);

            GetItemSlots();
        }

        [Button]
        private void SyncItems()
        {
            var count = Mathf.Min(_inventory.Size, _itemSlots.Length);
            for (int i = 0; i < count; i++)
                _itemSlots[i].BindTo(_inventory.Items[i]);
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
                DestroyImmediate(childObject);
#else
                Destroy(childObject);
#endif
            }
        }

        private void GetItemSlots() => _itemSlots = _itemSlotsParent.GetComponentsInChildren<ItemEntryView>(includeInactive: true);

#endregion

        #region Toggling View

        public void ToggleInventory()
        {
            if (_isOpen)
                CloseInventory();
            else
                OpenInventory();
        }

        [ButtonGroup]
        public void OpenInventory()
        {
            if (_confirmationDialog.IsActive) return;
            if (!IsPlayerInventory)
                CloseAllNonPlayerInventories();
            _openInventoryViews.Add(this);
            _panelAnimator.Show();
            IsOpen = true;
        }

        [ButtonGroup]
        public void CloseInventory()
        {
            if (_confirmationDialog.IsActive) return;
            _openInventoryViews.Remove(this);
            _panelAnimator.Hide();
            IsOpen = false;
        }

        private void CloseAllNonPlayerInventories()
        {
            for (int i = _openInventoryViews.Count - 1; i >= 0; i--)
            {
                var view = _openInventoryViews[i];
                if (!view.IsPlayerInventory)
                    view.CloseInventory();
            }
        }

        #endregion

        #region Assigned to Buttons

        public void CollectAllClicked() => _inventory.CollectAll();

        #endregion

        protected virtual void OnValidate()
        {
            if (_panelAnimator == null)
            {
                _panelAnimator = GetComponentInChildren<PanelAnimator>();
                if (_panelAnimator == null) Debug.LogWarning("Please assign a Panel Animator for the Inventory View", _panelAnimator);
            }

            if (_itemSlotsParent == null)
            {
                var parent = GetComponentInChildren<GridLayoutGroup>(true);
                if (parent != null) _itemSlotsParent = parent.GetComponent<RectTransform>();
                else Debug.LogWarning("Please assign a Transform to hold ItemSlots", _itemSlotsParent);
            }

            if (_slotPrefab == null)
                Debug.LogWarning("Please assign an ItemSlot prefab", _slotPrefab);
        }
    }
}
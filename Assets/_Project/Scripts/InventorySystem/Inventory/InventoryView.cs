using Sirenix.OdinInspector;
using System;
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

        [Header("References")]
        [SerializeField] private PanelAnimator _panelAnimator;
        [SerializeField] private ItemEntryView _slotPrefab;
        [SerializeField] private RectTransform _itemSlotsParent;
        [SerializeField, HideInInspector] private ItemEntryView[] _itemSlots;

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
            if (_inventory != null)
                BindTo(_inventory);
        }

        [Button]
        public void BindTo(Inventory inventory)
        {
            AdjustSize(inventory.Size);

            for (int i = 0; i < inventory.Size; i++)
                _itemSlots[i].BindTo(inventory.Items[i]);

            _inventory = inventory;
        }

        #region Adding & Removing ItemSlots

        [Button]
        protected void AdjustSize(int size)
        {
            var currentCount = _itemSlotsParent.childCount;
            if (size > currentCount)
                AddItemSlots(size - currentCount);
            else if (size < currentCount)
                RemoveItemSlots(currentCount - size);

            GetItemSlots();
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

        [Button]
        public void OpenInventory()
        {
            _panelAnimator.Show();
            IsOpen = true;
        }

        [Button]
        public void CloseInventory()
        {
            _panelAnimator.Hide();
            IsOpen = false;
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

            //if (_inventorySize != _itemSlotsParent.childCount)
            //{
            //    MatchSize(_inventorySize);
            //    RefreshItemSlots();
            //}
        }
    }
}
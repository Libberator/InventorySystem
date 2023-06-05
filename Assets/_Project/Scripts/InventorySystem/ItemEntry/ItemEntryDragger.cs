using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.UI;

namespace InventorySystem
{
    /// <summary>
    /// Acts as the Hand/Liaison to manage the moving, stacking, swapping, disposing, etc.
    /// Depends on ItemEntryView events for left-clicks and dragging, ItemEntryMenu for right-clicking, and
    /// Inventory for when they close and shift-click bulk-adding (inventory won't need to be open)
    /// </summary>
    public class ItemEntryDragger : MonoBehaviour
    {
        // garbage can is only thing listening to these events
        public static event Action<bool> IsDraggingChanged;
        public static event EventHandler<ItemEntry> ItemDisposed;

        [Header("Dragging References")]
        [SerializeField, Required] private Image _icon;
        [SerializeField, Required] private TextMeshProUGUI _qtyText;
        private Transform _draggedTransform;

        [Header("Juice")]
        [SerializeField] private float _punchStrength = 0.75f;
        [SerializeField] private float _punchDuration = 0.25f;
        private Tween _pickupTween;

        // Dependencies retrieved via ServiceLocator
        private Inventory _playerInventory;
        private ConfirmationDialog _confirmationDialog;
        
        [Header("What's In Hand")]
        [SerializeField, ReadOnly] private ItemEntry _entry = new();
        public ItemEntry Entry => _entry;

        private ItemEntryView _returnSlot;
        private bool _isPartialDrag = false;
        
        private bool _isDragging = false;
        public bool IsDragging
        {
            get => _isDragging;
            private set
            {
                if (_isDragging != value)
                {
                    _isDragging = value;
                    IsDraggingChanged?.Invoke(_isDragging);
                }
            }
        }

        private void Awake()
        {
            ServiceLocator.Register(this);
            _draggedTransform = _icon.transform;
            _entry.ItemChanged += OnItemChanged;
            _entry.QuantityChanged += OnQuantityChanged;
        }

        private void OnEnable()
        {
            ItemEntryView.BeginDrag += OnStartDragging;
            ItemEntryView.LeftClicked += OnLeftClicked;
            ItemEntryView.LeftShiftClicked += OnLeftShiftClicked;
            ItemEntryView.DoubleClicked += OnDoubleClicked;
            ItemEntryView.DroppedOn += OnDropped;
            ItemEntryView.EndDrag += OnEndDragging;
            
            InventoryView.Closed += OnInventoryClosed;

            ItemEntryMenu.BeginPartialDrag += OnStartPartialDragging;
        }

        private void OnDisable()
        {
            ItemEntryView.BeginDrag -= OnStartDragging;
            ItemEntryView.LeftClicked -= OnLeftClicked;
            ItemEntryView.LeftShiftClicked -= OnLeftShiftClicked;
            ItemEntryView.DoubleClicked -= OnDoubleClicked;
            ItemEntryView.DroppedOn -= OnDropped;
            ItemEntryView.EndDrag -= OnEndDragging;
            
            InventoryView.Closed -= OnInventoryClosed;

            ItemEntryMenu.BeginPartialDrag -= OnStartPartialDragging;
        }

        private void Start()
        {
            _playerInventory = ServiceLocator.Get<Inventory>();
            _confirmationDialog = ServiceLocator.Get<ConfirmationDialog>();
        }

        private void Update()
        {
            if (!_isDragging || _confirmationDialog.IsActive) return;
            // TODO: consider if the dragged icon needs to be offset. Update when we add custom Cursor
            _draggedTransform.position = Input.mousePosition;
        }

        #region Dragging, Dropping, Stacking, Swapping

        private void OnStartDragging(ItemEntryView slot)
        {
            if (_isPartialDrag)
                ReturnItemsToStart();

            _entry.SwapWith(slot.Entry);
            if (_returnSlot == null)
                _returnSlot = slot;
            IsDragging = true;
            _isPartialDrag = false;
        }

        private void OnStartPartialDragging(ItemEntryView slot, int partialQty)
        {
            // if this "partial" is actually a "full" drag
            if (partialQty == slot.Entry.Quantity)
            {
                OnStartDragging(slot);
                return;
            }

            slot.Entry.TransferTo(_entry, partialQty);
            if (_returnSlot == null)
                _returnSlot = slot;
            IsDragging = true;
            _isPartialDrag = true;
        }

        // pick up, drop, stack, drop
        private void OnLeftClicked(ItemEntryView slot)
        {
            if (!_isDragging)
            {
                if (slot.Item != null)
                    OnStartDragging(slot);
                return;
            }
            // below here, _isDragging is true

            // moving to empty slot or stacking on similar one
            if (_entry.CanTransferTo(slot.Entry))
            {
                _entry.TransferTo(slot.Entry);
                if (_entry.Quantity == 0)
                    StopDragging();
            }
            // swapping
            else if (slot.Item != _entry.Item)
            {
                // carrying a partial quantity - return to home
                if (_isPartialDrag)
                {
                    ReturnItemsToStart();
                    OnStartDragging(slot);
                }
                // swap with what's in hand
                else
                {
                    _entry.SwapWith(slot.Entry);
                }
            }
        }

        // swap to other open inventory (or "quick-collect" if from non-Player inventory)
        private void OnLeftShiftClicked(ItemEntryView slot)
        {
            // nothing implemented yet for shift-click while dragging
            if (_isDragging || slot.Item == null)
                return;
            // below here, _isDragging is false and we double clicked a valid item

            var source = InventoryView.GetInventoryFromItemEntry(slot);

            Inventory target = source != _playerInventory ? _playerInventory : InventoryView.GetOtherOpenInventory(slot);

            // TODO: add a Notification System for any Errors
            if (target == null)
            {
                Debug.Log("Double-clicked on Item in Player Inventory with no other open Inventory");
                return;
            }

            if (!target.TryAddItem(slot.Entry, out int remainder))
            {
                Debug.Log($"Inventory is too full to add {slot.Item} ({remainder})");
            }
            var qtyAdded = slot.Quantity - remainder;
            if (qtyAdded > 0)
                Debug.Log($"Added {qtyAdded} {slot.Item}");
            slot.Entry.RemoveQuantity(qtyAdded);
        }

        private void OnDoubleClicked(ItemEntryView slot)
        {
            // stack like items - only within the Player Inventory (I guess. Don't ask me why)
            if (_playerInventory.Contains(slot.Entry))
                _playerInventory.CombineLikeItems(slot.Item);
        }

        // called in conjunction with OnEndDragging
        private void OnDropped(ItemEntryView slot)
        {
            if (_isDragging)
            {
                OnLeftClicked(slot);
                ReturnItemsToStart();
                StopDragging();
            }
        }

        // After refactoring, I guess I don't need this method
        private void OnEndDragging(ItemEntryView slot)
        {
            // maybe if we drag out into the void (non-UI world), like for dropping items in minecraft
        }

        private void OnInventoryClosed(InventoryView panel)
        {
            if (_isDragging && panel.Contains(_returnSlot))
            {
                ReturnItemsToStart();
                StopDragging();
            }
        }

        private void ReturnItemsToStart()
        {
            if (_returnSlot != null)
                _entry.TransferTo(_returnSlot.Entry);
            _returnSlot = null;
        }

        private void StopDragging()
        {
            _returnSlot = null;
            _isPartialDrag = false;
            IsDragging = false;
        }

        #endregion

        #region Disposal

        public void DisposeEntry()
        {
            ItemDisposed?.Invoke(_draggedTransform, Entry);
            _entry.Set(null, 0);
            StopDragging();
        }

        #endregion

        #region Updating UI

        private void OnItemChanged(Item item)
        {
            if (item != null)
            {
                _icon.sprite = item.Icon;
                _icon.enabled = true;
                _pickupTween?.Complete();
                _pickupTween = _draggedTransform.DOPunchScale(_punchStrength * Vector3.one, _punchDuration);
            }
            else
            {
                _icon.enabled = false;
            }
        }

        private void OnQuantityChanged(int qty)
        {
            if (qty > 1)
            {
                _qtyText.SetText(qty.ToString());
                _qtyText.enabled = true;
            }
            else
                _qtyText.enabled = false;
        }

        #endregion
    }
}
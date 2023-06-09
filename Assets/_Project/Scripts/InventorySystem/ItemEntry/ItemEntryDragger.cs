using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.MessageSystem;
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
        public static event Action<bool> IsCarryingChanged;
        public static event EventHandler<ItemEntry> ItemDisposed;
        public ItemEntry Entry => _entry;

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
        private ItemEntryView _returnSlot;
        private bool _isPartialCarry = false;
        private bool _isCarrying = false;
        private bool CanDropOnto(ItemEntry slot) => slot.Item == null || slot.Item == _entry.Item && slot.Quantity < _entry.Item.MaxStack;

        public bool IsCarrying
        {
            get => _isCarrying;
            private set
            {
                if (_isCarrying != value)
                {
                    _isCarrying = value;
                    IsCarryingChanged?.Invoke(_isCarrying);
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
            ItemEntryView.HoverEntered += OnEntered;
            ItemEntryView.LeftClicked += OnLeftClicked;
            ItemEntryView.RightClicked += OnRightClicked;
            ItemEntryView.LeftShiftClicked += OnLeftShiftClicked;
            ItemEntryView.DoubleClicked += OnDoubleClicked;
            ItemEntryView.DroppedOn += OnDropped;
            ItemEntryView.BeginDrag += OnBeginCarry;
            ItemEntryView.RightBeginDrag += OnBeginRightDragging;
            ItemEntryView.RightEndDrag += OnEndRightDragging;
            ItemEntryView.RightDroppedOn += OnEndRightDragging;

            InventoryView.Closed += OnInventoryClosed;

            ItemEntryMenu.BeginPartialCarry += OnStartPartialCarrying;
        }

        private void OnDisable()
        {
            ItemEntryView.HoverEntered -= OnEntered;
            ItemEntryView.LeftClicked -= OnLeftClicked;
            ItemEntryView.RightClicked -= OnRightClicked;
            ItemEntryView.LeftShiftClicked -= OnLeftShiftClicked;
            ItemEntryView.DoubleClicked -= OnDoubleClicked;
            ItemEntryView.DroppedOn -= OnDropped;
            ItemEntryView.BeginDrag -= OnBeginCarry;
            ItemEntryView.RightBeginDrag -= OnBeginRightDragging;
            ItemEntryView.RightEndDrag -= OnEndRightDragging;
            ItemEntryView.RightDroppedOn -= OnEndRightDragging;

            InventoryView.Closed -= OnInventoryClosed;

            ItemEntryMenu.BeginPartialCarry -= OnStartPartialCarrying;
        }

        private void Start()
        {
            _playerInventory = ServiceLocator.Get<Inventory>();
            _confirmationDialog = ServiceLocator.Get<ConfirmationDialog>();
        }

        private void Update()
        {
            if (!_isCarrying || _confirmationDialog.IsActive) return;
            // TODO: consider if the dragged icon needs to be offset. Update when we add custom Cursor
            _draggedTransform.position = Input.mousePosition;
        }

        #region Carrying, Dragging, Dropping, Stacking, Swapping

        private void OnBeginCarry(ItemEntryView slot)
        {
            if (_isSplitDragging) return;

            if (_isPartialCarry)
                ReturnItemsToStart();

            _entry.SwapWith(slot.Entry);
            if (_returnSlot == null)
                _returnSlot = slot;
            IsCarrying = true;
            _isPartialCarry = false;
        }

        private void OnStartPartialCarrying(ItemEntryView slot, int partialQty)
        {
            // if this "partial" is actually a "full" drag
            if (partialQty == slot.Entry.Quantity)
            {
                OnBeginCarry(slot);
                return;
            }

            slot.Entry.TransferTo(_entry, partialQty);
            if (_returnSlot == null)
                _returnSlot = slot;
            IsCarrying = true;
            _isPartialCarry = true;
        }

        // pick up, drop, stack, drop
        private void OnLeftClicked(ItemEntryView slot)
        {
            if (_isSplitDragging) return;

            if (!_isCarrying)
            {
                if (slot.Item != null)
                    OnBeginCarry(slot);
                return;
            }

            var source = InventoryView.GetInventoryFromItemEntry(_returnSlot);
            var target = InventoryView.GetInventoryFromItemEntry(slot);
            var qty = _entry.Quantity;

            // moving to empty slot or stacking on similar one
            if (_entry.CanTransferTo(slot.Entry))
            {
                _entry.TransferTo(slot.Entry);
                if (_entry.Quantity == 0)
                    EndCarrying();

                if (source != target)
                {
                    if (source == _playerInventory)
                        Messenger.SendMessage(new InventoryMessage(slot.Item, qty - _entry.Quantity, InventoryEvent.ItemRemoveSuccess));
                    else
                        Messenger.SendMessage(new InventoryMessage(slot.Item, qty - _entry.Quantity, InventoryEvent.ItemAddSuccess));
                }
            }
            // swapping
            else if (slot.Item != _entry.Item)
            {
                // return partial carry to home before doing a swap
                if (_isPartialCarry)
                {
                    ReturnItemsToStart();
                    OnBeginCarry(slot);
                }
                // swap with what's in hand
                else
                {
                    _entry.SwapWith(slot.Entry);

                    if (source != target)
                    {
                        if (source == _playerInventory)
                        {
                            Messenger.SendMessage(new InventoryMessage(slot.Item, qty, InventoryEvent.ItemRemoveSuccess));
                            Messenger.SendMessage(new InventoryMessage(_entry.Item, _entry.Quantity, InventoryEvent.ItemAddSuccess));
                        }
                        if (target == _playerInventory)
                        {
                            Messenger.SendMessage(new InventoryMessage(_entry.Item, _entry.Quantity, InventoryEvent.ItemRemoveSuccess));
                            Messenger.SendMessage(new InventoryMessage(slot.Item, qty, InventoryEvent.ItemAddSuccess));
                        }
                    }
                }
            }
        }

        private void OnRightClicked(ItemEntryView view)
        {
            if (!_isCarrying || _isSplitDragging) return;
            if (!CanDropOnto(view.Entry)) return;

            _entry.TransferTo(view.Entry, 1);
            if (_entry.Quantity == 0)
                EndCarrying();
        }

        private void OnDropped(ItemEntryView slot)
        {
            if (_isSplitDragging) return;

            if (_isCarrying)
            {
                OnLeftClicked(slot);
                ReturnItemsToStart();
                EndCarrying();
            }
        }

        private void ReturnItemsToStart()
        {
            if (_returnSlot != null)
                _entry.TransferTo(_returnSlot.Entry);
            _returnSlot = null;
        }

        private void EndCarrying()
        {
            _returnSlot = null;
            _isPartialCarry = false;
            IsCarrying = false;
        }

        private void OnInventoryClosed(InventoryView panel)
        {
            if (_isCarrying && panel.Contains(_returnSlot))
            {
                ReturnItemsToStart();
                EndCarrying();
            }
        }

        #endregion

        #region Auto-Transferring & Auto-Stacking

        // swap to other open inventory
        private void OnLeftShiftClicked(ItemEntryView slot)
        {
            // nothing implemented yet for shift-click while dragging
            if (_isCarrying || slot.Item == null)
                return;
            // below here, _isDragging is false and we double clicked a valid item

            var target = InventoryView.GetInventoryFromItemEntry(slot) != _playerInventory ?
                _playerInventory : InventoryView.GetOtherOpenInventory(slot);

            if (target == null)
            {
                Messenger.SendMessage(new InventoryMessage(slot.Item, slot.Quantity, InventoryEvent.ItemMoveFail));
                return;
            }

            target.TryAddItem(slot.Entry, out int remainder);
            slot.Entry.RemoveQuantity(slot.Quantity - remainder);
        }

        private void OnDoubleClicked(ItemEntryView slot)
        {
            // stack like items - only within the Player Inventory (I guess. Don't ask me why)
            if (_playerInventory.Contains(slot.Entry))
                _playerInventory.CombineLikeItems(slot.Item);
        }

        #endregion

        #region Split Dragging

        private readonly HashSet<ItemEntryView> _splitDraggedTargets = new();
        private bool _isSplitDragging = false; // RMB + drag while carrying
        private bool CanSplitDrag => _isCarrying && _entry.Quantity > 1;

        private void OnBeginRightDragging(ItemEntryView slot)
        {
            if (!CanSplitDrag) return;
            if (!CanDropOnto(slot.Entry)) return;
            _isSplitDragging = true;

            AddTarget(slot);
        }

        private void OnEntered(ItemEntryView slot)
        {
            if (!_isSplitDragging) return;
            if (!CanDropOnto(slot.Entry)) return;

            AddTarget(slot);
        }

        private void OnEndRightDragging(ItemEntryView slot)
        {
            if (!_isSplitDragging) return;
            _isSplitDragging = false;

            SplitBetweenTargets();
            if (_entry.Quantity == 0)
                EndCarrying();
            else
                _isPartialCarry = true;
        }

        private void AddTarget(ItemEntryView slot)
        {
            slot.ShowHighlight();
            _splitDraggedTargets.Add(slot);
        }

        private void RemoveTargets()
        {
            foreach (var target in _splitDraggedTargets)
                target.HideHighlight();
            _splitDraggedTargets.Clear();
        }

        private void SplitBetweenTargets()
        {
            var targetAmount = _splitDraggedTargets.Count;
            var excess = _entry.Quantity % _splitDraggedTargets.Count;

            foreach (var target in _splitDraggedTargets)
            {
                var amountToAdd = _entry.Quantity / targetAmount;
                if (excess > 0)
                {
                    amountToAdd++;
                    excess--;
                }
                _entry.TransferTo(target.Entry, amountToAdd);
                targetAmount--;
            }

            RemoveTargets();
        }

        #endregion

        #region Disposal

        public void DisposeEntry()
        {
            ItemDisposed?.Invoke(_draggedTransform, _entry);
            _entry.Set(null, 0);
            EndCarrying();
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
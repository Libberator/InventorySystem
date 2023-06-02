using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.UI;

namespace InventorySystem
{
    /// <summary>
    /// Acts as the Hand/Liaison to manage the moving, stacking, swapping, disposing, etc.
    /// Depends on ItemEntryView events for left-clicks and dragging, ItemEntryMenu for right-clicking, and
    /// Inventory for when they close and shift-click bulk-adding (inventory won't need to be open)
    /// </summary>
    public class ItemEntryController : MonoBehaviour
    {
        public static event Action<bool> IsDraggingChanged;
        public static event Action<ItemEntry> DisposedEntry;

        [Header("Dragging References")]
        [SerializeField, Required] private Image _icon;
        [SerializeField, Required] private TextMeshProUGUI _qtyText;
        private Transform _draggedTransform;

        [Header("Right-Click Menu References")]
        [SerializeField] private ItemEntryMenu _rightClickMenu;

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

        private void Start()
        {
            _playerInventory = ServiceLocator.Get<Inventory>();
            _confirmationDialog = ServiceLocator.Get<ConfirmationDialog>();
        }

        private void OnEnable()
        {
            ItemEntryView.BeginDrag += OnStartDragging;
            ItemEntryView.LeftClicked += OnLeftClicked;
            ItemEntryView.LeftShiftClicked += OnLeftShiftClicked;
            ItemEntryView.RightClicked += OnRightClicked;
            ItemEntryView.DroppedOn += OnDropped;
            ItemEntryView.EndDrag += OnEndDragging;
            Inventory.Closed += OnInventoryClosed;
            _rightClickMenu.BeginPartialDrag += OnStartPartialDragging;
        }

        private void OnDisable()
        {
            ItemEntryView.BeginDrag -= OnStartDragging;
            ItemEntryView.LeftClicked -= OnLeftClicked;
            ItemEntryView.LeftShiftClicked -= OnLeftShiftClicked;
            ItemEntryView.RightClicked -= OnRightClicked;
            ItemEntryView.DroppedOn -= OnDropped;
            ItemEntryView.EndDrag -= OnEndDragging;
            Inventory.Closed -= OnInventoryClosed;
            _rightClickMenu.BeginPartialDrag -= OnStartPartialDragging;
        }

        private void Update()
        {
            if (!_isDragging) return;
            // TODO: consider if the dragged icon needs to be offset. Update when we add custom Cursor
            _draggedTransform.position = Input.mousePosition;
        }

        #region Dragging, Dropping, Stacking, Swapping

        private void OnStartDragging(ItemEntryView slot)
        {
            if (_rightClickMenu.MenuShown)
                _rightClickMenu.HideMenu();

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

        private void OnLeftShiftClicked(ItemEntryView slot)
        {
            if (_rightClickMenu.MenuShown)
                _rightClickMenu.HideMenu();

            // nothing implemented yet for shift-click while dragging
            if (_isDragging)
                return;
            // below here, _isDragging is false
            
            if (slot.Item != null)
            {
                // shift-clicked in the player inventory - try to do stacking
                if (_playerInventory.Contains(slot))
                {
                    _playerInventory.CombineLikeItems(slot.Item);
                }
                // shift-clicked in an external inventory - collect into player inventory
                else
                {
                    // TODO: add a way to display messages on picking items up, and errors notifications too
                    if (!_playerInventory.TryAddItem(slot.Entry, out int remainder))
                    {
                        Debug.Log($"Inventory is too full to add {slot.Item} ({remainder})");
                    }
                    var qtyAdded = slot.Quantity - remainder;
                    if (qtyAdded > 0)
                        Debug.Log($"Added {qtyAdded} {slot.Item}");
                    slot.Entry.RemoveQuantity(qtyAdded);
                }
            }
        }

        private void OnLeftClicked(ItemEntryView slot)
        {
            if (_rightClickMenu.MenuShown)
                _rightClickMenu.HideMenu();

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

        private void OnRightClicked(ItemEntryView slot)
        {
            if (_isDragging) return;

            if (slot.Item == null || _rightClickMenu.MenuShown && _rightClickMenu.FocusedSlot == slot)
                _rightClickMenu.HideMenu();
            else
                _rightClickMenu.ShowMenu(slot);
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

        private void OnInventoryClosed(Inventory inventory)
        {
            if (_isDragging && inventory.Contains(_returnSlot))
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

        public void Dispose()
        {
            if (!_isDragging) return;
            _isDragging = false;
            var msg = $"Dispose of\n{_entry.Item.ColoredName.WithLink("Item")} ({_entry.Quantity})?";
            _confirmationDialog.AskWithBypass("Dispose Item", msg, ConfirmDisposal, CancelDisposal);
        }

        private void ConfirmDisposal()
        {
            DisposedEntry?.Invoke(_entry);
            _entry.Set(null, 0);
            _isDragging = true;
            StopDragging();
        }

        private void CancelDisposal() => _isDragging = true;

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
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    /// <summary>
    /// Acts as the Hand/Liaison to manage the moving, stacking, swapping, etc.
    /// Depends on ItemEntryView events for left-clicks and dragging, ItemEntryMenu for right-clicking, and
    /// Inventory for when they close and shift-click bulk-adding (inventory won't need to be open)
    /// </summary>
    public class ItemEntryController : Singleton<ItemEntryController>
    {
        [Header("Dragging References")]
        [SerializeField, Required] private Image _icon;
        [SerializeField, Required] private TMP_Text _qtyText;
        private Transform _draggedTransform;

        [Header("Right-Click Menu References")]
        [SerializeField] private ItemEntryMenu _rightClickMenu;

        [Header("Juice")]
        [SerializeField] private float _punchStrength = 0.75f;
        [SerializeField] private float _punchDuration = 0.25f;
        private Tween _pickupTween;

        private bool _isDragging = false;
        private bool _isPartialDrag = false;
        private ItemEntryView _returnSlot;

        [Header("What's In Hand")]
        [SerializeField, ReadOnly]
        private ItemEntry _entry = new();
        private Item DraggedItem => _entry.Item;
        private int DraggedQuantity => _entry.Quantity;

        protected override void Awake()
        {
            base.Awake();
            _draggedTransform = _icon.transform;
            _entry.ItemChanged += OnItemChanged;
            _entry.QuantityChanged += OnQuantityChanged;
        }

        private void OnEnable()
        {
            ItemEntryView.BeginDrag += OnStartDragging;
            ItemEntryView.LeftClicked += OnLeftClicked;
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
            _entry.SwapWith(slot.Entry);
            _returnSlot = slot;
            _isDragging = true;
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
            _returnSlot = slot;
            _isDragging = true;
            _isPartialDrag = true;
        }

        private void OnLeftClicked(ItemEntryView slot)
        {
            if (!_isDragging)
            {
                // check if Shift is being Held
                // check if source of slot is NOT main inventory
                // Then try to transfer as many things over via inventory.T
                // on second though: make OnShiftLeftClicked a separate method

                OnStartDragging(slot);
            }
            else if (_isDragging)
            {
                // moving to empty slot or stacking on similar one
                if (_entry.CanTransferTo(slot.Entry))
                {
                    _entry.TransferTo(slot.Entry);
                    if (DraggedQuantity == 0)
                        StopDragging();
                }
                // swapping
                else if (slot.Item != DraggedItem)
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
                        //_returnSlot = slot;
                    }
                }
            }
            if (_rightClickMenu.MenuShown)
                _rightClickMenu.HideMenu();
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
        }

        private void StopDragging()
        {
            _returnSlot = null;
            _isDragging = false;
            _isPartialDrag = false;
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
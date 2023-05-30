using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    /// <summary>
    /// Acts as the Hand/Liaison to manage the moving, stacking, swapping, etc.
    /// Depends on ItemSlot events for left-clicks and dragging, ItemSlotMenu for right-clicking, and
    /// Inventory for when they close and shift-click bulk-adding (inventory won't need to be open)
    /// </summary>
    public class ItemSlotInteractor : Singleton<ItemSlotInteractor>
    {
        [Header("Dragging References")]
        [SerializeField, Required] private Image _icon;
        [SerializeField, Required] private TMP_Text _qtyText;
        private Transform _draggedTransform;

        [Header("Right-Click Menu References")]
        [SerializeField] private ItemSlotMenu _rightClickMenu;

        [Header("Juice")]
        [SerializeField] private float _punchStrength = 0.75f;
        [SerializeField] private float _punchDuration = 0.25f;
        private Tween _pickupTween;

        private bool _isDragging = false;
        private ItemSlot _source;
        private Item _draggedItem;
        private int _draggedQuantity;

        protected override void Awake()
        {
            base.Awake();
            _draggedTransform = _icon.transform;
        }

        private void OnEnable()
        {
            ItemSlot.BeginDrag += OnStartDragging;
            ItemSlot.LeftClicked += OnLeftClicked;
            ItemSlot.RightClicked += OnRightClicked;
            ItemSlot.DroppedOn += OnDropped;
            ItemSlot.EndDrag += OnEndDragging;
            Inventory.Closed += OnInventoryClosed;
            _rightClickMenu.SplitItems += OnStartDragging;
        }

        private void OnDisable()
        {
            ItemSlot.BeginDrag -= OnStartDragging;
            ItemSlot.LeftClicked -= OnLeftClicked;
            ItemSlot.RightClicked -= OnRightClicked;
            ItemSlot.DroppedOn -= OnDropped;
            ItemSlot.EndDrag -= OnEndDragging;
            Inventory.Closed -= OnInventoryClosed;
            _rightClickMenu.SplitItems -= OnStartDragging;
        }

        private void Update()
        {
            if (!_isDragging) return;
            // TODO: consider if the dragged icon needs to be offset. Update when we add custom Cursor
            _draggedTransform.position = Input.mousePosition;
        }

        #region Dragging, Dropping, Stacking, Swapping

        private void OnStartDragging(ItemSlot slot)
        {
            if (slot.Item == null) return;

            _source = slot;
            _draggedItem = slot.Item;
            _draggedQuantity = slot.Quantity;
            slot.TryRemoveItem(slot.Quantity, out _);
            
            RefreshUI();

            _isDragging = true;
        }

        private void OnStartDragging(ItemSlot slot, int partialQty)
        {
            if (slot.Item == null) return;

            _source = slot;
            _draggedItem = slot.Item;
            _draggedQuantity = partialQty;
            _source.TryRemoveItem(partialQty, out _);
            RefreshUI();

            _isDragging = true;
        }

        private void OnLeftClicked(ItemSlot slot)
        {
            if (!_isDragging)
            {
                // check if Shift is being Held
                // check if source of slot is NOT main inventory
                // Then try to transfer as many things over via inventory.T


                OnStartDragging(slot);
            }
            else if (_isDragging)
            {
                // moving and/or stacking
                if (slot.Item == null || slot.Item == _draggedItem && slot.Item.IsStackable)
                {
                    if (slot.TryAddItem(_draggedItem, _draggedQuantity, out _draggedQuantity))
                        StopDragging();
                    else
                        UpdateQuantity(_draggedQuantity);
                }
                // swapping
                else if (slot.Item != _draggedItem)
                {
                    // carrying a partial quantity - return to home
                    if (_source.Quantity > 0)
                    {
                        _source.TryAddItem(_draggedItem, _draggedQuantity, out _);
                        OnStartDragging(slot);
                    }
                    // do proper swap with what's in hand
                    else
                    {
                        var safeSource = _source;
                        var itemToDropOff = _draggedItem;
                        var qtyToDropOff = _draggedQuantity;
                        OnStartDragging(slot);
                        _source = safeSource;
                        slot.SetEntry(itemToDropOff, qtyToDropOff);
                    }
                }
            }
            if (_rightClickMenu.MenuShown)
                _rightClickMenu.HideMenu();
        }

        private void OnRightClicked(ItemSlot slot)
        {
            if (_isDragging) return;

            if (slot.Item == null || _rightClickMenu.MenuShown && _rightClickMenu.FocusedSlot == slot)
                _rightClickMenu.HideMenu();
            else
                _rightClickMenu.ShowMenu(slot);
        }

        private void OnDropped(ItemSlot slot)
        {
            if (_isDragging)
                OnLeftClicked(slot);
        }

        private void OnEndDragging(ItemSlot slot)
        {
            if (_isDragging)
                StopDragging();
        }

        private void OnInventoryClosed(Inventory inventory)
        {
            if (_isDragging && inventory.Contains(_source))
                StopDragging();
        }

        private void StopDragging()
        {
            // return any items back home
            _source.TryAddItem(_draggedItem, _draggedQuantity, out _);
            _source = null;
            _draggedItem = null;
            _draggedQuantity = 0;
            
            HideUI();
            _isDragging = false;
        }

        #endregion

        #region Updating UI

        private void RefreshUI()
        {
            ShowItem(_draggedItem);
            UpdateQuantity(_draggedQuantity);
        }

        private void ShowItem(Item item)
        {
            _icon.sprite = item.Icon;
            _icon.enabled = true;
            _pickupTween?.Complete();
            _pickupTween = _draggedTransform.DOPunchScale(_punchStrength * Vector3.one, _punchDuration);
        }

        private void UpdateQuantity(int qty)
        {
            if (qty > 1)
            {
                _qtyText.SetText(qty.ToString());
                _qtyText.enabled = true;
            }
            else
                _qtyText.enabled = false;
        }

        private void HideUI()
        {
            _icon.enabled = false;
            _qtyText.enabled = false;
        }

        #endregion
    }
}
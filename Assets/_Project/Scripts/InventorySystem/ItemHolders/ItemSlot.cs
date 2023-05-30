using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
    {
        public static event Action<ItemSlot> PointerEnter;
        public static event Action<ItemSlot> PointerExit;
        public static event Action<ItemSlot> LeftClicked;
        public static event Action<ItemSlot> RightClicked;
        public static event Action<ItemSlot> BeginDrag;
        public static event Action<ItemSlot> EndDrag;
        public static event Action<ItemSlot> DroppedOn;

        [Header("UI References")]
        [SerializeField] private Image _background;
        [SerializeField] private Image _frame;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _qtyText;

        [Header("Settings")]
        [SerializeField] private Rarity _default;
        private readonly float _punchStrength = -0.6f;
        private readonly float _punchDuration = 0.25f;
        private Tween _placeItemTween;

        [Header("Internal Data")]
        [SerializeField, ReadOnly] private Item _item;
        public Item Item
        {
            get => _item;
            protected set
            {
                _item = value;
                ShowItem(_item);
            }
        }

        [SerializeField, ReadOnly] private int _quantity;
        public int Quantity
        {
            get => _quantity;
            protected set
            {
                _quantity = value;
                if (_quantity == 0)
                    Item = null;
                ShowQuantity(_quantity);
            }
        }

        protected virtual void Start() => RefreshUI();

        public virtual void SetEntry(SlotEntry entry) => SetEntry(entry.Item, entry.Quantity);

        public virtual void SetEntry(Item item, int qty)
        {
            Item = item;
            Quantity = qty;
        }

        public virtual bool CanStackItem(Item item) => Item == item && Quantity < Item.MaxStack;

        protected virtual bool CanSwapWith(ItemSlot other) => true;
        // TODO: make sure we don't lose important quest items or non-destructables

        #region Adding & Removing

        public virtual bool TryAddItem(Item item, int qty, out int remainder)
        {
            remainder = 0;
            if (Item == null)
            {
                Item = item;
                Quantity = qty;
                return true;
            }

            if (Item == item)
            {
                var toAdd = Math.Min(item.MaxStack - Quantity, qty);
                remainder = qty - toAdd;
                Quantity += toAdd;
            }

            return remainder == 0; // was able to add ALL of it
        }

        /// <summary>
        /// The out remainder parameter is the remainder of the requested quantity, not ItemSlot qty
        /// </summary>
        public virtual bool TryRemoveItem(int qty, out int remainder)
        {
            var toRemove = Math.Min(Quantity, qty);
            remainder = qty - toRemove;
            Quantity -= toRemove;

            return remainder == 0; // was able to remove ALL of requested qty
        }

        //public virtual void StackOnto(ItemSlot other)
        //{
        //    if (!other.CanStackItem(Item)) return;

        //    var toAdd = Math.Min(Item.MaxStack - other.Quantity, Quantity);
        //    other.Quantity += toAdd;
        //    Quantity -= toAdd;
        //}

        //public virtual void SwapWith(ItemSlot other)
        //{
        //    (_item, other.Item) = (other.Item, _item);
        //    (_quantity, other.Quantity) = (other.Quantity, _quantity);
        //}

        #endregion

        #region Updating UI

        public virtual void RefreshUI()
        {
            ShowItem(_item);
            ShowQuantity(_quantity);
        }

        public virtual void ShowQuantity(int qty)
        {
            if (qty > 1)
            {
                _qtyText.SetText(qty.ToString());
                _qtyText.enabled = true;
            }
            else
            {
                _qtyText.enabled = false;
            }
        }

        public virtual void ShowItem(Item item)
        {
            if (item != null)
            {
                _background.color = item.Rarity.PrimaryColor;
                _frame.color = item.Rarity.SecondaryColor;
                _icon.sprite = item.Icon;
                _icon.enabled = true;
                _placeItemTween?.Complete();
                _placeItemTween = _icon.transform.DOPunchScale(_punchStrength * Vector3.one, _punchDuration);
            }
            else
            {
                _background.color = _default.PrimaryColor;
                _frame.color = _default.SecondaryColor;
                _icon.enabled = false;
            }
        }

        #endregion

        #region Interface Methods

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                LeftClicked?.Invoke(this);
            else if (eventData.button == PointerEventData.InputButton.Right)
                RightClicked?.Invoke(this);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            _background.DOFade(1f, 0.2f);
            PointerEnter?.Invoke(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            _background.DOFade(0.5f, 0.2f);
            PointerExit?.Invoke(this);
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                BeginDrag?.Invoke(this);
        }

        public virtual void OnEndDrag(PointerEventData eventData) => EndDrag?.Invoke(this);

        public virtual void OnDrag(PointerEventData eventData) { } // unusued, but required for other things to work

        public virtual void OnDrop(PointerEventData eventData) => DroppedOn?.Invoke(this);

        #endregion
    }
}

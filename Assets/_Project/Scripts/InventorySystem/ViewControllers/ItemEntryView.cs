﻿using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    public class ItemEntryView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
    {
        public static event Action<ItemEntryView> PointerEnter;
        public static event Action<ItemEntryView> PointerExit;
        public static event Action<ItemEntryView> LeftClicked;
        public static event Action<ItemEntryView> RightClicked;
        public static event Action<ItemEntryView> BeginDrag;
        public static event Action<ItemEntryView> EndDrag;
        public static event Action<ItemEntryView> DroppedOn;

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
        [SerializeField, ReadOnly] private ItemEntry _entry = new();
        public ItemEntry Entry => _entry;
        public Item Item => _entry.Item;
        public int Quantity => _entry.Quantity;

        private void OnEnable()
        {
            _entry.ItemChanged += OnItemChanged;
            _entry.QuantityChanged += OnQuantityChanged;
            OnItemChanged(_entry.Item);
            OnQuantityChanged(_entry.Quantity);
        }

        private void OnDisable()
        {
            _entry.ItemChanged -= OnItemChanged;
            _entry.QuantityChanged -= OnQuantityChanged;
        }

        public virtual void SetEntry(ItemEntry entry) => _entry.Set(entry);
        public virtual void SetEntry(Item item, int qty) => _entry.Set(item, qty);

        #region Updating UI

        private void OnItemChanged(Item item)
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

        private void OnQuantityChanged(int qty)
        {
            if (qty > 1)
            {
                _qtyText.SetText(qty.ToString());
                _qtyText.enabled = true;
                _placeItemTween?.Complete();
                _placeItemTween = _icon.transform.DOPunchScale(_punchStrength * Vector3.one, _punchDuration);
            }
            else
            {
                _qtyText.enabled = false;
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
            if (eventData.button == PointerEventData.InputButton.Left && _entry.Item != null)
                BeginDrag?.Invoke(this);
        }

        public virtual void OnDrag(PointerEventData eventData) { } // unusued, but required for other things to work

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                DroppedOn?.Invoke(this);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                EndDrag?.Invoke(this);
        }

        #endregion
    }
}
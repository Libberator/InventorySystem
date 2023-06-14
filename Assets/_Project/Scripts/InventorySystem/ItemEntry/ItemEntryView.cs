using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.Cooldown;
using Utilities.UI;

namespace InventorySystem
{
    // Relies on the legacy Input system for holding down Left Shift
    public class ItemEntryView : PointerInteractor<ItemEntryView>
    {
        [Header("UI References")]
        [SerializeField] private Image _background;
        [SerializeField] private Image _frame;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _highlight;
        [SerializeField] private TMP_Text _qtyText;
        [SerializeField] private CooldownView _cooldown;

        [Header("Settings")]
        [SerializeField] private Rarity _default;
        private readonly float _punchStrength = -0.6f;
        private readonly float _punchDuration = 0.25f;
        private Tween _placeItemTween;

        [Header("Internal Data")]
        [SerializeField, ReadOnly] private ItemEntry _entry;

        public ItemEntry Entry => _entry;
        public Item Item => _entry.Item;
        public int Quantity => _entry.Quantity;

        private void OnDestroy()
        {
            if (_entry != null) UnbindFrom(_entry);
        }

        #region Bindings

        public virtual void BindTo(ItemEntry entry)
        {
            if (_entry == entry) return;
            if (_entry != null) UnbindFrom(_entry);
            if (entry == null) return;

            _entry = entry;
            _entry.ItemChanged += OnItemChanged;
            _entry.QuantityChanged += OnQuantityChanged;

            OnItemChanged(_entry.Item);
            OnQuantityChanged(_entry.Quantity);
        }

        private void UnbindFrom(ItemEntry entry)
        {
            entry.ItemChanged -= OnItemChanged;
            entry.QuantityChanged -= OnQuantityChanged;

            OnItemChanged(null);
            OnQuantityChanged(0);
            _entry = null;
        }

        #endregion

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

            if (item is IHaveCooldown cooldown)
                _cooldown.BindTo(cooldown);
            else
                _cooldown.UnbindFromCurrent();
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

        public void ShowHighlight() => _highlight.enabled = true;

        public void HideHighlight() => _highlight.enabled = false;

        #region Inherited Methods

        protected override void OnHoverEntered(ItemEntryView target)
        {
            _background.DOFade(1f, 0.2f);
            base.OnHoverEntered(target);
        }

        protected override void OnHoverExited(ItemEntryView target)
        {
            _background.DOFade(0.5f, 0.2f);
            base.OnHoverExited(target);
        }
        
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (_entry.Item != null)
                    OnBeginDrag(this);
                else
                    eventData.pointerDrag = null; // prevents OnDrag & OnDrop from being called
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
                OnRightBeginDrag(this);
        }

        #endregion
    }
}

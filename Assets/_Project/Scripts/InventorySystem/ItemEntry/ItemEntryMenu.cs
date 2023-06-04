using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.UI;

namespace InventorySystem
{
    // "Right Click Menu". Relies on the legacy Input system for the mouse scrolling
    public class ItemEntryMenu : MonoBehaviour
    {
        public static event Action<ItemEntryView, int> BeginPartialDrag;
        public static event Action<ItemEntryView> UseClicked;
        public static event Action<ItemEntryView> EquipClicked;

        [Header("Quantity Splitter")]
        [SerializeField] private Image _splittingSelector;
        [SerializeField] private TMP_Text _qtyText;
        [SerializeField] private float _fillDuration = 0.5f;
        [SerializeField] private Ease _fillEase = Ease.OutQuint;
        private int _partialQuantity;
        private Tween _splitterTween;

        [Header("Button Animators")]
        [SerializeField] private PanelAnimator _use;
        [SerializeField] private PanelAnimator _equip;
        [SerializeField] private PanelAnimator _sell;
        [SerializeField] private PanelAnimator _toss;

        private ItemEntryDragger _dragger;
        private ItemEntryView _focusedSlot;
        private bool _isShown;
        
        private ItemEntry Entry => _focusedSlot.Entry;

        private void Awake()
        {
            ItemEntryView.BeginDrag += Hide;
            ItemEntryView.LeftClicked += Hide;
            ItemEntryView.LeftShiftClicked += Hide;
            ItemEntryView.RightClicked += OnRightClicked;
        }

        private void OnDestroy()
        {
            ItemEntryView.BeginDrag -= Hide;
            ItemEntryView.LeftClicked -= Hide;
            ItemEntryView.LeftShiftClicked -= Hide;
            ItemEntryView.RightClicked -= OnRightClicked;
        }

        private void Start()
        {
            HideQtySplitter();
            _dragger = ServiceLocator.Get<ItemEntryDragger>();
        }

        private void Update()
        {
            if (!_isShown || _focusedSlot == null || Entry.Item == null || !Entry.Item.IsStackable) return;
            if (Input.mouseScrollDelta.y != 0)
            {
                _splitterTween?.Kill();
                UpdateQuantity(_partialQuantity + (int)Input.mouseScrollDelta.y);
            }
        }

        private void OnRightClicked(ItemEntryView slot)
        {
            if (_dragger.IsDragging) return;

            if (slot.Item == null || _isShown && _focusedSlot == slot)
                HideMenu();
            else
                ShowMenu(slot);
        }

        #region Show & Hide Menu

        public void ShowMenu(ItemEntryView slot)
        {
            _focusedSlot = slot;
            transform.position = slot.transform.position;

            if (Entry.Quantity > 1)
                ShowQtySplitter();
            else
                HideQtySplitter();

            // consider interfaces
            if (Entry.Item is Consumable)
                _use.Show(restart: true);
            else if (_use.isActiveAndEnabled)
                _use.Hide(instant: true);
            
            // consider interfaces
            if (Entry.Item is Equipment)
                _equip.Show(restart: true);
            else if (_equip.isActiveAndEnabled)
                _equip.Hide(instant: true);

            _isShown = true;
        }

        private void Hide(ItemEntryView _)
        {
            if (_isShown) HideMenu();
        }

        [Button]
        public void HideMenu()
        {
            _use.Hide();
            _equip.Hide();
            _sell.Hide();
            _toss.Hide();
            HideQtySplitter();
            _isShown = false;
        }

        #endregion

        #region Quantity Splitter

        // assigned to the Radial Fill as an Event Trigger
        public void OnSplitterClick(BaseEventData baseEventData)
        {
            var eventData = baseEventData as PointerEventData;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                BeginPartialDrag?.Invoke(_focusedSlot, _partialQuantity);
                HideMenu();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                HideMenu();
            }
        }

        private string QtyText(int selected, int max) => $"{selected}/{max}";

        private void ShowQtySplitter()
        {
            _splitterTween?.Kill();
            _splittingSelector.gameObject.SetActive(true);

            var max = Entry.Quantity;

            _splitterTween = DOVirtual.Int(0, max / 2, _fillDuration, UpdateQuantity).SetEase(_fillEase);
        }

        private void UpdateQuantity(int qty)
        {
            int max = Entry.Quantity;
            if (max == 0)
            {
                HideMenu();
                return;
            }
            if (max == 1)
            {
                HideQtySplitter();
                return;
            }

            qty = Mathf.Clamp(qty, 1, max);
            _splittingSelector.fillAmount = (float)qty / max;
            _qtyText.SetText(QtyText(qty, max));

            _partialQuantity = qty;
        }

        private void HideQtySplitter()
        {
            _splitterTween?.Kill();
            _splittingSelector.gameObject.SetActive(false);
        }

        #endregion

        #region Button Methods

        public void UseButtonPressed()
        {
            var consumable = Entry.Item as Consumable;

            UseClicked?.Invoke(_focusedSlot);
            
            if (Entry.RemoveQuantity(1) == 0)
            {
                UpdateQuantity(_partialQuantity);
                consumable.Use();
            }
        }

        public void EquipButtonPressed()
        {
            Debug.Log("Equip Button Pressed");
            EquipClicked?.Invoke(_focusedSlot);
            HideMenu();
        }

        public void SellButtonPressed()
        {
            Debug.Log("Sell Button Pressed");
        }

        public void TossButtonPressed()
        {
            Debug.Log("Toss Button Pressed");
        }

        #endregion
    }
}
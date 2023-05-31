using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public class ItemEntryMenu : MonoBehaviour
    {
        public event Action<ItemEntryView, int> BeginPartialDrag;
        //public event Action<ItemSlot> EquippedItem;

        [Header("Quantity Splitter")]
        [SerializeField] private Image _splittingSelector;
        [SerializeField] private TMP_Text _qtyText;
        [SerializeField] private float _fillDuration = 0.75f;
        private int _partialQuantity;
        private Tween _splitterTween;

        [Header("Button Animators")]
        [SerializeField] private PanelAnimator _use;
        [SerializeField] private PanelAnimator _equip;
        [SerializeField] private PanelAnimator _sell;
        [SerializeField] private PanelAnimator _toss;

        public bool MenuShown { get; private set; }
        public ItemEntryView FocusedSlot { get; private set; }
        private ItemEntry Entry => FocusedSlot.Entry;

        private void Start()
        {
            HideQtySelector();
        }

        private void Update()
        {
            if (!MenuShown || FocusedSlot == null || Entry.Item == null || !Entry.Item.IsStackable) return;
            if (Input.mouseScrollDelta.y != 0)
            {
                _splitterTween?.Kill();
                UpdateQuantity(_partialQuantity + (int)Input.mouseScrollDelta.y);
            }
        }

        #region Show & Hide Menu

        public void ShowMenu(ItemEntryView slot)
        {
            FocusedSlot = slot;
            transform.position = slot.transform.position;

            if (Entry.Quantity > 1)
                ShowQtySelector();
            else
                HideQtySelector();

            // consider interfaces
            if (Entry.Item is Equipment)
                _equip.Show(restart: true);
            else if (_equip.isActiveAndEnabled)
                _equip.Hide(instant: true);

            // consider interfaces
            if (Entry.Item is Consumable)
                _use.Show(restart: true);
            else if (_use.isActiveAndEnabled)
                _use.Hide(instant: true);

            MenuShown = true;
        }

        [Button]
        public void HideMenu()
        {
            _use.Hide();
            _sell.Hide();
            _toss.Hide();
            _equip.Hide();
            HideQtySelector();
            MenuShown = false;
        }

        #endregion

        #region Quantity Splitter

        private string QtyText(int selected, int max) => $"{selected}/{max}";

        private void ShowQtySelector()
        {
            _splitterTween?.Kill();
            _splittingSelector.gameObject.SetActive(true);

            var max = Entry.Quantity;

            _splitterTween = DOVirtual.Int(0, max / 2, _fillDuration, UpdateQuantity).SetEase(Ease.OutBack);
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
                HideQtySelector();
                return;
            }

            qty = Mathf.Clamp(qty, 1, max);
            _splittingSelector.fillAmount = (float)qty / max;
            _qtyText.SetText(QtyText(qty, max));

            _partialQuantity = qty;
        }

        private void HideQtySelector()
        {
            _splitterTween?.Kill();
            _splittingSelector.gameObject.SetActive(false);
        }

        #endregion

        #region Button Methods

        public void SplitItemPressed()
        {
            BeginPartialDrag?.Invoke(FocusedSlot, _partialQuantity);
            HideMenu();
        }

        public void UseButtonPressed()
        {
            var consumable = Entry.Item as Consumable;

            if (Entry.RemoveQuantity(1) == 0)
            {
                UpdateQuantity(_partialQuantity);
                consumable.Use();
            }
        }

        public void EquipButtonPressed()
        {
            Debug.Log("Equip Button Pressed");
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

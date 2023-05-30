using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;

namespace InventorySystem
{
    public class ItemSlotMenu : MonoBehaviour
    {
        public event Action<ItemSlot, int> SplitItems;
        //public event Action<ItemSlot> EquippedItem;

        [Header("Quantity Splitter")]
        [SerializeField] private Image _splittingSelector;
        [SerializeField] private TMP_Text _qtyText;
        [SerializeField] private int _quantity;
        [SerializeField] private float _fillDuration = 0.75f;
        private Tween _splitterTween;

        [Header("Button Animators")]
        [SerializeField] private PanelAnimator _use;
        [SerializeField] private PanelAnimator _equip;
        [SerializeField] private PanelAnimator _sell;
        [SerializeField] private PanelAnimator _toss;

        public bool MenuShown { get; private set; }
        public ItemSlot FocusedSlot { get; private set; }

        private void Start()
        {
            HideQtySelector();
        }

        private void Update()
        {
            if (!MenuShown || FocusedSlot == null || FocusedSlot.Item == null || !FocusedSlot.Item.IsStackable) return;
            if (Input.mouseScrollDelta.y != 0)
            {
                _splitterTween?.Kill();
                UpdateQuantity(_quantity + (int)Input.mouseScrollDelta.y);
            }
        }

        #region Show & Hide Menu

        public void ShowMenu(ItemSlot slot)
        {
            FocusedSlot = slot;
            transform.position = slot.transform.position;

            if (slot.Quantity > 1)
                ShowQtySelector();
            else
                HideQtySelector();
            
            // consider interfaces
            if (slot.Item is Equipment)
                _equip.Show(restart: true);
            else if (_equip.isActiveAndEnabled)
                _equip.Hide(instant: true);

            // consider interfaces
            if (slot.Item is Consumable)
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

            var max = FocusedSlot.Quantity;

            _splitterTween = DOVirtual.Int(0, max / 2, _fillDuration, UpdateQuantity).SetEase(Ease.OutBack);
        }
        
        private void UpdateQuantity(int qty)
        {
            int max = FocusedSlot.Quantity;
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

            _quantity = qty;
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
            SplitItems?.Invoke(FocusedSlot, _quantity);
            //Debug.Log($"Split pressed. Chose {_quantity} / {_source.Quantity}");
            HideMenu();
        }

        public void UseButtonPressed()
        {
            var consumable = FocusedSlot.Item as Consumable;

            if (FocusedSlot.TryRemoveItem(1, out _))
            {
                UpdateQuantity(_quantity);
                consumable.Use();
            }
            //HideMenu();
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

using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;
using Utilities.MessageSystem;
using Utilities.UI;

namespace InventorySystem
{
    public class GarbageCan : MonoBehaviour, IPointerClickHandler, IDropHandler
    {
        [SerializeField] private PanelSlider _slider;
        [SerializeField] private string _richTextLinkID = "Item";
        private ItemEntryDragger _dragger;
        private ConfirmationDialog _confirmationDialog;

        private void OnEnable() => ItemEntryDragger.IsCarryingChanged += OnCarryingChanged;

        private void OnDisable() => ItemEntryDragger.IsCarryingChanged -= OnCarryingChanged;

        private void Start()
        {
            _dragger = ServiceLocator.Get<ItemEntryDragger>();
            _confirmationDialog = ServiceLocator.Get<ConfirmationDialog>();
        }

        private void OnCarryingChanged(bool isDragging)
        {
            if (isDragging)
                _slider.Show(); // TODO: Add safety checks to not show if the item is not disposable.
            else
                _slider.Hide();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                StartDisposal(_dragger.Entry);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                StartDisposal(_dragger.Entry);
        }

        private void StartDisposal(ItemEntry entry)
        {
            // TODO: Add safety checks to automatically cancel if the item is not disposable.
            // It would classify as an ItemDiscardFailed InventoryEvent
            var msg = $"Dispose of\n{entry.Item.ColoredName.WithLink(_richTextLinkID)} ({entry.Quantity})?";
            _confirmationDialog.AskWithBypass("Dispose Item", msg, ConfirmDisposal, CancelDisposal);
        }

        private void ConfirmDisposal()
        {
            Messenger.SendMessage(new InventoryMessage(_dragger.Entry.Item, _dragger.Entry.Quantity, InventoryEvent.ItemDiscardSuccess));
            _dragger.DisposeEntry();
        }

        private void CancelDisposal() =>
            Messenger.SendMessage(new InventoryMessage(_dragger.Entry.Item, _dragger.Entry.Quantity, InventoryEvent.ItemDiscardCancelled));
    }
}

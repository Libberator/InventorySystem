using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;
using Utilities.UI;

namespace InventorySystem
{
    public class GarbageCan : MonoBehaviour, IPointerClickHandler, IDropHandler
    {
        [SerializeField] private PanelAnimator _animator;
        [SerializeField] private ParticleSystem _particle;
        private ItemEntryDragger _dragger;
        private ConfirmationDialog _confirmationDialog;

        private void Start()
        {
            _dragger = ServiceLocator.Get<ItemEntryDragger>();
            _confirmationDialog = ServiceLocator.Get<ConfirmationDialog>();
            ItemEntryDragger.IsDraggingChanged += OnDraggingChanged;
        }

        private void OnDraggingChanged(bool isDragging)
        {
            if (isDragging)
                _animator.Show();
            else
                _animator.Hide();
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
            var msg = $"Dispose of\n{entry.Item.ColoredName.WithLink("Item")} ({entry.Quantity})?";
            _confirmationDialog.AskWithBypass("Dispose Item", msg, ConfirmDisposal, CancelDisposal);
        }

        private void ConfirmDisposal()
        {
            _dragger.DisposeEntry();
            _particle.Play();
        }

        private void CancelDisposal() => Debug.Log("Disposal Cancelled"); // do nothing
    }
}

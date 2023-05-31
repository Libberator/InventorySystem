using UnityEngine;
using UnityEngine.EventSystems;
using Utilities.UI;

namespace InventorySystem
{
    public class GarbageCan : MonoBehaviour, IPointerClickHandler, IDropHandler
    {
        [SerializeField] private PanelAnimator _animator;
        [SerializeField] private ParticleSystem _particle;
        private ItemEntryController _dragger;

        private void Awake()
        {
            ItemEntryController.IsDraggingChanged += OnDraggingChanged;
            ItemEntryController.DisposedEntry += OnDisposed;
        }

        private void Start() => _dragger = ItemEntryController.Instance;

        private void OnDraggingChanged(bool isDragging)
        {
            if (isDragging)
                _animator.Show();
            else
                _animator.Hide();
        }

        private void OnDisposed(ItemEntry entry) => _particle.Play();

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                _dragger.Dispose();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                _dragger.Dispose();
        }
    }
}

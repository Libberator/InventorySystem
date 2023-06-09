using UnityEngine;
using UnityEngine.EventSystems;

namespace CursorSystem
{
    public class CursorHoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private CursorType _hoverCursor = CursorType.Hand;

        private CursorController _cursorController;

        private void Start() => _cursorController = ServiceLocator.Get<CursorController>();

        public void OnPointerEnter(PointerEventData eventData) => _cursorController.ChangeCursor(_hoverCursor);

        public void OnPointerExit(PointerEventData eventData) => _cursorController.ResetCursor();

        private void OnMouseEnter() => _cursorController.ChangeCursor(_hoverCursor);

        private void OnMouseExit() => _cursorController.ResetCursor();
    }
}

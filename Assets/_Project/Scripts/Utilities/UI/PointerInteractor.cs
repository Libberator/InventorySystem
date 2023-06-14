using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utilities.UI
{
    // Relies on the legacy Input system for holding down Left/Right Shift
    public abstract class PointerInteractor<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
        where T : PointerInteractor<T>
    {
        protected T _this;
        private void Awake() => _this = this as T;

        #region Hovering

        public static event Action<T> HoverEntered;
        public static event Action<T> HoverExited;

        protected virtual void OnHoverEntered(T target) => HoverEntered?.Invoke(target);
        protected virtual void OnHoverExited(T target) => HoverExited?.Invoke(target);

        public virtual void OnPointerEnter(PointerEventData eventData) => OnHoverEntered(_this);
        public virtual void OnPointerExit(PointerEventData eventData) => OnHoverExited(_this);

        #endregion

        #region Clicking

        public static event Action<T> LeftShiftClicked;
        public static event Action<T> DoubleClicked;
        public static event Action<T> LeftClicked;
        public static event Action<T> RightClicked;

        protected virtual void OnLeftShiftClicked(T target) => LeftShiftClicked?.Invoke(target);
        protected virtual void OnDoubleClicked(T target) => DoubleClicked?.Invoke(target);
        protected virtual void OnLeftClicked(T target) => LeftClicked?.Invoke(target);
        protected virtual void OnRightClicked(T target) => RightClicked?.Invoke(target);

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    OnLeftShiftClicked(_this);
                else
                    OnLeftClicked(_this);

                if (eventData.clickCount == 2)
                    OnDoubleClicked(_this);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
                OnRightClicked(_this);
        }

        #endregion

        #region Dragging

        public static event Action<T> BeginDrag;
        public static event Action<T> Drag;
        public static event Action<T> EndDrag;
        public static event Action<T> RightBeginDrag;
        public static event Action<T> RightDrag;
        public static event Action<T> RightEndDrag;

        protected virtual void OnBeginDrag(T target) => BeginDrag?.Invoke(target);
        protected virtual void OnDrag(T target) => Drag?.Invoke(target);
        protected virtual void OnEndDrag(T target) => EndDrag?.Invoke(target);
        protected virtual void OnRightBeginDrag(T target) => RightBeginDrag?.Invoke(target);
        protected virtual void OnRightDrag(T target) => RightDrag?.Invoke(target);
        protected virtual void OnRightEndDrag(T target) => RightEndDrag?.Invoke(target);

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                OnBeginDrag(_this);
            else if (eventData.button == PointerEventData.InputButton.Right)
                OnRightBeginDrag(_this);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                OnDrag(_this);
            if (eventData.button == PointerEventData.InputButton.Right)
                OnRightDrag(_this);
        }
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                OnEndDrag(_this);
            else if (eventData.button == PointerEventData.InputButton.Right)
                OnRightEndDrag(_this);
        }

        #endregion

        #region Dropping

        public static event Action<T> DroppedOn;
        public static event Action<T> RightDroppedOn;

        protected virtual void OnDropped(T target) => DroppedOn?.Invoke(target);
        protected virtual void OnRightDropped(T target) => RightDroppedOn?.Invoke(target);

        // Note: OnDrop gets called *before* OnEndDrag, and on different targets
        public virtual void OnDrop(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                OnDropped(_this);
            if (eventData.button == PointerEventData.InputButton.Right)
                OnRightDropped(_this);
        }

        #endregion
    }
}

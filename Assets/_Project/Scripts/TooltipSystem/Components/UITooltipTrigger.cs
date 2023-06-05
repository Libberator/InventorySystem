using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace TooltipSystem
{
    public class UITooltipTrigger : TooltipTrigger, IPointerEnterHandler, IPointerMoveHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField, PropertyOrder(1)] protected TooltipAction _onPointerEnter = TooltipAction.Show;
        [SerializeField, PropertyOrder(1)] protected TooltipAction _onPointerMove = TooltipAction.Show;
        [SerializeField, PropertyOrder(1)] protected TooltipAction _onPointerExit = TooltipAction.Hide;
        [SerializeField, PropertyOrder(1)] protected TooltipAction _onPointerClick = TooltipAction.Hide;

        public virtual void OnPointerEnter(PointerEventData eventData) =>
            HandleTooltipAction(_onPointerEnter);

        public virtual void OnPointerMove(PointerEventData eventData) =>
            HandleTooltipAction(_onPointerMove);

        public virtual void OnPointerClick(PointerEventData eventData) =>
            HandleTooltipAction(_onPointerClick);

        public virtual void OnPointerExit(PointerEventData eventData) =>
            HandleTooltipAction(_onPointerExit);

        protected virtual void HandleTooltipAction(TooltipAction action)
        {
            switch (action)
            {
                case TooltipAction.Show:
                    ShowTooltip();
                    break;
                case TooltipAction.Hide:
                    HideTooltip();
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// If you don't want another class to implement IHaveTooltip, you can inherit from this
    /// to have access to a "_target", which you can use in order to provide a Tooltip.
    /// </summary>
    public abstract class UITooltipTrigger<T> : UITooltipTrigger, IHaveTooltip where T : Component
    {
        [SerializeField] protected T _target;

        protected override void Awake()
        {
            base.Awake();
            FindTarget();
        }

        protected virtual void FindTarget()
        {
            if (_target == null && !this.TryGetComponentInHierarchy(out _target))
                Debug.LogWarning($"Could not find a Target for [{typeof(T)}] in the hierarchy. Please assign one.", this);
        }

        public abstract Tooltip GetTooltip();

        protected virtual void OnValidate() => FindTarget();
    }
}

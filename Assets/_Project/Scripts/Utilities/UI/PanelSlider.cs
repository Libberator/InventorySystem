using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.UI
{
    public class PanelSlider : MonoBehaviour, IDisplayable
    {
        [SerializeField, HideInInspector] private RectTransform _rectTransform;
        private RectTransform RectProperty => _rectTransform != null ? _rectTransform : _rectTransform = GetComponent<RectTransform>();

        [SerializeField] protected bool _startShown = false;

        [Tooltip("This should match the RectTranform's local/anchored Position in Inspector when shown")]
        [InlineButton(nameof(SetShownPosition), "Set")]
        [InlineButton(nameof(GetShownPosition), "Get")]
        [SerializeField, FoldoutGroup("Show Settings")] protected Vector2 _shownPosition;
        [SerializeField, FoldoutGroup("Show Settings")] protected float _showDuration = 0.5f;
        [SerializeField, FoldoutGroup("Show Settings")] protected Ease _showEase = Ease.OutBack;
        [SerializeField, FoldoutGroup("Show Settings")] protected UnityEvent _onStartShowing;
        [SerializeField, FoldoutGroup("Show Settings")] protected UnityEvent _onShowComplete;

        [Tooltip("This should match the RectTranform's local/anchored Position in Inspector when hidden")]
        [InlineButton(nameof(SetHiddenPosition), "Set")]
        [InlineButton(nameof(GetHiddenPosition), "Get")]
        [SerializeField, FoldoutGroup("Hide Settings")] protected Vector2 _hiddenPosition;
        [SerializeField, FoldoutGroup("Hide Settings")] protected float _hideDuration = 0.5f;
        [SerializeField, FoldoutGroup("Hide Settings")] protected Ease _hideEase = Ease.OutQuint;
        [SerializeField, FoldoutGroup("Hide Settings")] protected bool _setInactiveWhenHidden = true;
        [SerializeField, FoldoutGroup("Hide Settings")] protected UnityEvent _onStartHiding;
        [SerializeField, FoldoutGroup("Hide Settings")] protected UnityEvent _onHideComplete;

        private Tween _tween;

        private IEnumerator Start()
        {
            yield return null;
            // why wait a frame? Because the things that are using this have conflicting LayoutGroups w/ Content Size Fitter on children
            // For more info: https://docs.unity3d.com/ScriptReference/Canvas.ForceUpdateCanvases.html
            if (_startShown)
                Show();
            else
                Hide(instant: true);
        }

        [Button]
        public void Show() => Show(false);
        public void Show(bool restart)
        {
            _onStartShowing.Invoke();
            if (restart || !gameObject.activeInHierarchy)
            {
                SetHiddenPosition();
                gameObject.SetActive(true);
            }

            _tween?.Kill();
            _tween = RectProperty.DOAnchorPos(_shownPosition, _showDuration).SetEase(_showEase).OnComplete(_onShowComplete.Invoke);
        }

        [Button]
        public void Hide() => Hide(false);
        public void Hide(bool instant)
        {
            _onStartHiding.Invoke();
            if (instant)
            {
                SetHiddenPosition();
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
                _onHideComplete.Invoke();
                return;
            }

            _tween?.Kill();
            _tween = RectProperty.DOAnchorPos(_hiddenPosition, _hideDuration).SetEase(_hideEase).OnComplete(() =>
            {
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
                _onHideComplete.Invoke();
            });
        }

        private void GetShownPosition() => _shownPosition = RectProperty.anchoredPosition;
        private void SetShownPosition() => RectProperty.anchoredPosition = _shownPosition;
        private void GetHiddenPosition() => _hiddenPosition = RectProperty.anchoredPosition;
        private void SetHiddenPosition() => RectProperty.anchoredPosition = _hiddenPosition;

        private void OnDrawGizmosSelected()
        {
            var hidden = _hiddenPosition - RectProperty.anchoredPosition;
            var shown = _shownPosition - RectProperty.anchoredPosition;

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(hidden, shown);
        }
    }
}
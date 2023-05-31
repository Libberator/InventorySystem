using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class PanelAnimator : MonoBehaviour
    {
        [HideInInspector] private RectTransform _rectTransform;
        private RectTransform RectProperty => _rectTransform != null ? _rectTransform : _rectTransform = GetComponent<RectTransform>();

        [Tooltip("This should match the RectTranform's local/anchored Position in Inspector when shown")]
        [InlineButton(nameof(MoveToShownPosition), "Move To")]
        [InlineButton(nameof(SetShownPositionToCurrent), "Set")]
        [SerializeField, FoldoutGroup("Show Settings")] protected Vector2 _shownPosition;
        [SerializeField, FoldoutGroup("Show Settings")] protected Ease _showEase = Ease.OutBack;
        [SerializeField, FoldoutGroup("Show Settings")] protected float _showDuration = 0.5f;
        [SerializeField, FoldoutGroup("Show Settings")] protected bool _startShown = false;
        [SerializeField, FoldoutGroup("Show Settings")] protected UnityEvent _onStartShowing;
        [SerializeField, FoldoutGroup("Show Settings")] protected UnityEvent _onShowComplete;

        [Tooltip("This should match the RectTranform's local/anchored Position in Inspector when hidden")]
        [InlineButton(nameof(MoveToHiddenPosition), "Move To")]
        [InlineButton(nameof(SetHiddenPositionToCurrent), "Set")]
        [SerializeField, FoldoutGroup("Hide Settings")] protected Vector2 _hiddenPosition;
        [SerializeField, FoldoutGroup("Hide Settings")] protected Ease _hideEase = Ease.InBack;
        [SerializeField, FoldoutGroup("Hide Settings")] protected float _hideDuration = 0.5f;
        [SerializeField, FoldoutGroup("Hide Settings")] protected bool _setInactiveWhenHidden = true;
        [SerializeField, FoldoutGroup("Hide Settings")] protected UnityEvent _onStartHiding;
        [SerializeField, FoldoutGroup("Hide Settings")] protected UnityEvent _onHideComplete;

        private Tween _tween;

        private IEnumerator Start()
        {
            yield return null;
            // why wait a frame? because https://docs.unity3d.com/ScriptReference/Canvas.ForceUpdateCanvases.html
            gameObject.SetActive(_startShown);
        }

        [Button]
        public void Show(bool restart = false)
        {
            if (restart || !gameObject.activeInHierarchy)
            {
                MoveToHiddenPosition();
                gameObject.SetActive(true);
            }
            _onStartShowing?.Invoke();

            _tween?.Kill();
            _tween = RectProperty.DOAnchorPos(_shownPosition, _showDuration).SetEase(_showEase).OnComplete(() =>
            {
                _onShowComplete?.Invoke();
            });
        }

        [Button]
        public void Hide(bool instant = false)
        {
            if (instant)
            {
                MoveToHiddenPosition();
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
                _onHideComplete?.Invoke(); // decide later if this is preferred
                return;
            }

            _onStartHiding?.Invoke();

            _tween?.Kill();
            _tween = RectProperty.DOAnchorPos(_hiddenPosition, _hideDuration).SetEase(_hideEase).OnComplete(() =>
            {
                _onHideComplete?.Invoke();
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
            });
        }

        private void SetShownPositionToCurrent() => _shownPosition = RectProperty.anchoredPosition;
        private void MoveToShownPosition() => RectProperty.anchoredPosition = _shownPosition;
        private void SetHiddenPositionToCurrent() => _hiddenPosition = RectProperty.anchoredPosition;
        private void MoveToHiddenPosition() => RectProperty.anchoredPosition = _hiddenPosition;

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
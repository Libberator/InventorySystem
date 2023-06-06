using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.UI
{
    public class PanelScaler : MonoBehaviour, IDisplayable
    {
        [SerializeField, HideInInspector] private RectTransform _rectTransform;
        private RectTransform RectTransform => _rectTransform != null ? _rectTransform : _rectTransform = GetComponent<RectTransform>();

        [SerializeField, LabelWidth(200)] protected bool _startShown = false;

        [InlineButton(nameof(SetShownScale), "Set")]
        [InlineButton(nameof(GetShownScale), "Get")]
        [SerializeField, FoldoutGroup("Show Settings")] protected Vector3 _shownScale = Vector3.one;
        [SerializeField, FoldoutGroup("Show Settings")] protected float _showDuration = 0.5f;
        [SerializeField, FoldoutGroup("Show Settings")] protected Ease _showEase = Ease.OutBack;
        [SerializeField, FoldoutGroup("Show Settings")] protected UnityEvent _onStartShowing;
        [SerializeField, FoldoutGroup("Show Settings")] protected UnityEvent _onShowComplete;

        [InlineButton(nameof(SetHiddenScale), "Set")]
        [InlineButton(nameof(GetHiddenScale), "Get")]
        [SerializeField, FoldoutGroup("Hide Settings")] protected Vector3 _hiddenScale = Vector3.zero;
        [SerializeField, FoldoutGroup("Hide Settings")] protected float _hideDuration = 0.5f;
        [SerializeField, FoldoutGroup("Hide Settings")] protected Ease _hideEase = Ease.OutBack;
        [SerializeField, FoldoutGroup("Hide Settings"), LabelWidth(200)] protected bool _setInactiveWhenHidden = false;
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
                SetHiddenScale();
                gameObject.SetActive(true);
            }

            _tween?.Kill();
            _tween = RectTransform.DOScale(_shownScale, _showDuration).SetEase(_showEase).OnComplete(_onShowComplete.Invoke);
        }

        [Button]
        public void Hide() => Hide(false);
        public void Hide(bool instant)
        {
            _onStartHiding.Invoke();
            if (instant)
            {
                SetHiddenScale();
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
                _onHideComplete.Invoke();
                return;
            }

            _tween?.Kill();
            _tween = RectTransform.DOScale(_hiddenScale, _hideDuration).SetEase(_showEase).OnComplete(() =>
            {
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
                _onHideComplete.Invoke();
            });
        }

        private void GetShownScale() => _shownScale = RectTransform.localScale;
        private void SetShownScale() => RectTransform.localScale = _shownScale;
        private void GetHiddenScale() => _hiddenScale = RectTransform.localScale;
        private void SetHiddenScale() => RectTransform.localScale = _hiddenScale;
    }
}

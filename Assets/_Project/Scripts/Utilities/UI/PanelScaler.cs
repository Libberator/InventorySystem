using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.UI
{
    public class PanelScaler : MonoBehaviour
    {
        [SerializeField, HideInInspector] private RectTransform _rectTransform;
        private RectTransform RectTransform => _rectTransform != null ? _rectTransform : _rectTransform = GetComponent<RectTransform>();

        [SerializeField, LabelWidth(200)] protected bool _hideOnStart = false;

        [InlineButton(nameof(SetShownScale), "Set")]
        [InlineButton(nameof(GetShownScale), "Get")]
        [SerializeField, FoldoutGroup("Show Settings")] protected Vector3 _shownScale = Vector3.one;
        [SerializeField, FoldoutGroup("Show Settings")] protected float _showDuration = 0.5f;
        [SerializeField, FoldoutGroup("Show Settings")] protected Ease _showEase = Ease.OutBack;
        [FoldoutGroup("Show Settings")] public UnityEvent OnStartShowing;
        [FoldoutGroup("Show Settings")] public UnityEvent OnShowComplete;

        [InlineButton(nameof(SetHiddenScale), "Set")]
        [InlineButton(nameof(GetHiddenScale), "Get")]
        [SerializeField, FoldoutGroup("Hide Settings")] protected Vector3 _hiddenScale = Vector3.zero;
        [SerializeField, FoldoutGroup("Hide Settings")] protected float _hideDuration = 0.5f;
        [SerializeField, FoldoutGroup("Hide Settings")] protected Ease _hideEase = Ease.OutBack;
        [SerializeField, FoldoutGroup("Hide Settings"), LabelWidth(200)] protected bool _setInactiveWhenHidden = false;
        [FoldoutGroup("Hide Settings")] public UnityEvent OnStartHiding;
        [FoldoutGroup("Hide Settings")] public UnityEvent OnHideComplete;

        private Tween _tween;

        private IEnumerator Start()
        {
            yield return null;
            // why wait a frame? In case the things that are using this have conflicting LayoutGroups w/ Content Size Fitter on children
            // For more info: https://docs.unity3d.com/ScriptReference/Canvas.ForceUpdateCanvases.html
            if (_hideOnStart)
                Hide(instant: true);
        }

        [Button]
        public Tween Show() => Show(false);
        public Tween Show(bool restart)
        {
            OnStartShowing.Invoke();
            if (restart || !gameObject.activeInHierarchy)
            {
                SetHiddenScale();
                gameObject.SetActive(true);
            }

            _tween?.Kill();
            return _tween = RectTransform.DOScale(_shownScale, _showDuration).SetEase(_showEase).OnComplete(OnShowComplete.Invoke);
        }

        [Button]
        public Tween Hide() => Hide(false);
        public Tween Hide(bool instant)
        {
            OnStartHiding.Invoke();
            if (instant)
            {
                SetHiddenScale();
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
                OnHideComplete.Invoke();
                return null;
            }

            _tween?.Kill();
            return _tween = RectTransform.DOScale(_hiddenScale, _hideDuration).SetEase(_showEase).OnComplete(() =>
            {
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
                OnHideComplete.Invoke();
            });
        }

        private void GetShownScale() => _shownScale = RectTransform.localScale;
        private void SetShownScale() => RectTransform.localScale = _shownScale;
        private void GetHiddenScale() => _hiddenScale = RectTransform.localScale;
        private void SetHiddenScale() => RectTransform.localScale = _hiddenScale;
    }
}

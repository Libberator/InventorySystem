using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PanelFader : MonoBehaviour
    {
        [SerializeField, HideInInspector] private CanvasGroup _canvasGroup;
        private CanvasGroup CanvasGroup => _canvasGroup != null ? _canvasGroup : _canvasGroup = GetComponent<CanvasGroup>();

        [SerializeField, LabelWidth(200)] protected bool _hideOnStart = true;

        [InlineButton(nameof(SetShownAlpha), "Set")]
        [InlineButton(nameof(GetShownAlpha), "Get")]
        [FoldoutGroup("Show Settings"), SerializeField, Range(0, 1)] protected float _shownAlpha = 1f;
        [FoldoutGroup("Show Settings"), SerializeField] protected float _showDuration = 0.5f;
        [FoldoutGroup("Show Settings"), SerializeField] protected Ease _showEase = Ease.OutQuint;
        [FoldoutGroup("Show Settings"), SerializeField, LabelWidth(200)] protected bool _interactableWhenShown = true;
        [FoldoutGroup("Show Settings"), SerializeField, LabelWidth(200)] protected bool _blockRaycastsWhenShown = true;
        [FoldoutGroup("Show Settings")] public UnityEvent OnStartShowing;
        [FoldoutGroup("Show Settings")] public UnityEvent OnShowComplete;

        [InlineButton(nameof(SetHiddenAlpha), "Set")]
        [InlineButton(nameof(GetHiddenAlpha), "Get")]
        [FoldoutGroup("Hide Settings"), SerializeField, Range(0, 1)] protected float _hiddenAlpha = 0f;
        [FoldoutGroup("Hide Settings"), SerializeField] protected float _hideDuration = 0.5f;
        [FoldoutGroup("Hide Settings"), SerializeField] protected Ease _hideEase = Ease.OutQuint;
        [FoldoutGroup("Hide Settings"), SerializeField, LabelWidth(200)] protected bool _setInactiveWhenHidden = false;
        [FoldoutGroup("Hide Settings"), SerializeField, LabelWidth(200)] protected bool _interactableWhenHidden = false;
        [FoldoutGroup("Hide Settings"), SerializeField, LabelWidth(200)] protected bool _blockRaycastsWhenHidden = false;
        [FoldoutGroup("Hide Settings")] public UnityEvent OnStartHiding;
        [FoldoutGroup("Hide Settings")] public UnityEvent OnHideComplete;

        private Tween _tween;

        private IEnumerator Start()
        {
            yield return null;
            // why wait a frame? Because the things that are using this have conflicting LayoutGroups w/ Content Size Fitter on children
            // For more info: https://docs.unity3d.com/ScriptReference/Canvas.ForceUpdateCanvases.html
            if (_hideOnStart)
                Hide(instant: true);
        }

        [Button(DisplayParameters = false)]
        public Tween Show() => Show(false);
        public Tween Show(bool restart)
        {
            OnStartShowing.Invoke();
            if (restart || !gameObject.activeInHierarchy)
            {
                SetHiddenState();
                gameObject.SetActive(true);
            }

            SetShownState();

            _tween?.Kill();
            return _tween = CanvasGroup.DOFade(_shownAlpha, _showDuration)
                .SetEase(_showEase)
                .OnComplete(OnShowComplete.Invoke);
        }

        [Button(DisplayParameters = false)]
        public Tween Hide() => Hide(false);
        public Tween Hide(bool instant)
        {
            OnStartHiding.Invoke();
            _tween?.Kill();

            if (instant)
            {
                CanvasGroup.alpha = _hiddenAlpha;
                SetHiddenState();
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
                OnHideComplete.Invoke();
                return null;
            }

            return _tween = CanvasGroup.DOFade(_hiddenAlpha, _hideDuration)
                .SetEase(_hideEase)
                .OnComplete(() =>
                {
                    SetHiddenState();
                    if (_setInactiveWhenHidden)
                        gameObject.SetActive(false);
                    OnHideComplete.Invoke();
                });
        }

        private void GetShownAlpha() => _shownAlpha = CanvasGroup.alpha;
        private void SetShownAlpha() => CanvasGroup.alpha = _shownAlpha;
        private void GetHiddenAlpha() => _hiddenAlpha = CanvasGroup.alpha;
        private void SetHiddenAlpha() => CanvasGroup.alpha = _hiddenAlpha;

        private void SetShownState()
        {
            CanvasGroup.interactable = _interactableWhenShown;
            CanvasGroup.blocksRaycasts = _blockRaycastsWhenShown;
        }

        private void SetHiddenState()
        {
            CanvasGroup.interactable = _interactableWhenHidden;
            CanvasGroup.blocksRaycasts = _blockRaycastsWhenHidden;
        }
    }
}

using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PanelFader : MonoBehaviour, IDisplayable
    {
        [SerializeField, HideInInspector] private CanvasGroup _canvasGroup;
        private CanvasGroup CanvasGroup => _canvasGroup != null ? _canvasGroup : _canvasGroup = GetComponent<CanvasGroup>();
        
        [SerializeField, LabelWidth(200)] protected bool _startShown = false;

        [InlineButton(nameof(SetShownAlpha), "Set")]
        [InlineButton(nameof(GetShownAlpha), "Get")]
        [SerializeField, FoldoutGroup("Show Settings"), Range(0, 1)] protected float _shownAlpha = 1f;
        [SerializeField, FoldoutGroup("Show Settings")] protected float _showDuration = 0.5f;
        [SerializeField, FoldoutGroup("Show Settings")] protected Ease _showEase = Ease.OutQuint;
        [SerializeField, FoldoutGroup("Show Settings"), LabelWidth(200)] protected bool _interactableWhenShown = true;
        [SerializeField, FoldoutGroup("Show Settings"), LabelWidth(200)] protected bool _blockRaycastsWhenShown = true;
        [SerializeField, FoldoutGroup("Show Settings")] protected UnityEvent _onStartShowing;
        [SerializeField, FoldoutGroup("Show Settings")] protected UnityEvent _onShowComplete;

        [InlineButton(nameof(SetHiddenAlpha), "Set")]
        [InlineButton(nameof(GetHiddenAlpha), "Get")]
        [SerializeField, FoldoutGroup("Hide Settings"), Range(0, 1)] protected float _hiddenAlpha = 0f;
        [SerializeField, FoldoutGroup("Hide Settings")] protected float _hideDuration = 0.5f;
        [SerializeField, FoldoutGroup("Hide Settings")] protected Ease _hideEase = Ease.OutQuint;
        [SerializeField, FoldoutGroup("Hide Settings"), LabelWidth(200)] protected bool _setInactiveWhenHidden = false;
        [SerializeField, FoldoutGroup("Hide Settings"), LabelWidth(200)] protected bool _interactableWhenHidden = false;
        [SerializeField, FoldoutGroup("Hide Settings"), LabelWidth(200)] protected bool _blockRaycastsWhenHidden = false;
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
                SetHiddenState();
                gameObject.SetActive(true);
            }

            SetShownState();

            _tween?.Kill();
            _tween = CanvasGroup.DOFade(_shownAlpha, _showDuration).SetEase(_showEase).OnComplete(_onShowComplete.Invoke);
        }

        [Button]
        public void Hide() => Hide(false);
        public void Hide(bool instant)
        {
            _onStartHiding.Invoke();
            if (instant)
            {
                SetHiddenState();
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
                _onHideComplete.Invoke();
                return;
            }

            _tween?.Kill();
            _tween = CanvasGroup.DOFade(_hiddenAlpha, _hideDuration).SetEase(_showEase).OnComplete(() =>
            {
                SetHiddenState();
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
                _onHideComplete.Invoke();
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

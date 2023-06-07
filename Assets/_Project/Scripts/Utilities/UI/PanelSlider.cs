using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.UI
{
    public class PanelSlider : MonoBehaviour
    {
        [SerializeField, HideInInspector] private RectTransform _rectTransform;
        private RectTransform RectProperty => _rectTransform != null ? _rectTransform : _rectTransform = GetComponent<RectTransform>();

        [SerializeField, LabelWidth(200)] protected bool _hideOnStart = true;

        [Tooltip("This should match the RectTranform's local/anchored Position in Inspector when shown")]
        [InlineButton(nameof(SetShownPosition), "Set")]
        [InlineButton(nameof(GetShownPosition), "Get")]
        [FoldoutGroup("Show Settings"), SerializeField] protected Vector2 _shownPosition;
        [FoldoutGroup("Show Settings"), SerializeField] protected Ease _showEase = Ease.OutBack;
        [FoldoutGroup("Show Settings"), SerializeField, LabelWidth(150)] protected bool _showIsSpeedBased = false;
        [FoldoutGroup("Show Settings"), SerializeField, LabelText("@ShowText"), LabelWidth(150), Min(0.01f)] protected float _showDuration = 0.5f;
        [FoldoutGroup("Show Settings")] public UnityEvent OnStartShowing;
        [FoldoutGroup("Show Settings")] public UnityEvent OnShowComplete;
        private string ShowText => _showIsSpeedBased ? "Show Speed (units/sec)" : "Show Duration (sec)";

        [Tooltip("This should match the RectTranform's local/anchored Position in Inspector when hidden")]
        [InlineButton(nameof(SetHiddenPosition), "Set")]
        [InlineButton(nameof(GetHiddenPosition), "Get")]
        [FoldoutGroup("Hide Settings"), SerializeField] protected Vector2 _hiddenPosition;
        [FoldoutGroup("Hide Settings"), SerializeField] protected Ease _hideEase = Ease.OutQuint;
        [FoldoutGroup("Hide Settings"), SerializeField, LabelWidth(150)] protected bool _hideIsSpeedBased = false;
        [FoldoutGroup("Hide Settings"), SerializeField, LabelText("@HideText"), LabelWidth(150), Min(0.01f)] protected float _hideDuration = 0.5f;
        [FoldoutGroup("Hide Settings"), SerializeField, LabelWidth(150)] protected bool _setInactiveWhenHidden = false;
        [FoldoutGroup("Hide Settings")] public UnityEvent OnStartHiding;
        [FoldoutGroup("Hide Settings")] public UnityEvent OnHideComplete;
        private string HideText => _hideIsSpeedBased ? "Hide Speed (units/sec)" : "Hide Duration (sec)";

        private Tween _tween;

        private IEnumerator Start()
        {
            yield return null;
            // why wait a frame? In case the things that are using this have conflicting LayoutGroups w/ Content Size Fitter on children
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
                SetHiddenPosition();
                gameObject.SetActive(true);
            }

            _tween?.Kill();
            return _tween = RectProperty.DOAnchorPos(_shownPosition, _showDuration)
                .SetSpeedBased(_showIsSpeedBased)
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
                SetHiddenPosition();
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
                OnHideComplete.Invoke();
                return null;
            }

            return _tween = RectProperty.DOAnchorPos(_hiddenPosition, _hideDuration)
                .SetSpeedBased(_hideIsSpeedBased)
                .SetEase(_hideEase)
                .OnComplete(() =>
                {
                    if (_setInactiveWhenHidden)
                        gameObject.SetActive(false);
                    OnHideComplete.Invoke();
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
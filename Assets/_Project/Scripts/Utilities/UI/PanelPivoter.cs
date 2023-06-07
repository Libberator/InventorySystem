using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.UI
{
    public class PanelPivoter : MonoBehaviour
    {
        [SerializeField, HideInInspector] private RectTransform _rectTransform;
        private RectTransform RectTransform => _rectTransform != null ? _rectTransform : _rectTransform = GetComponent<RectTransform>();

        [SerializeField, HideInInspector] protected Vector2 _defaultPivotPos = Vector2.up;
        [OnValueChanged(nameof(OnDefaultAdjusted))]
        [SerializeField, FoldoutGroup("Settings")] protected TextAnchor _defaultPivot = TextAnchor.UpperLeft;
        [SerializeField, FoldoutGroup("Settings")] protected Ease _slideEase = Ease.Linear; // Or InOut options
        [SerializeField, FoldoutGroup("Settings")] protected bool _isSpeedBased = false;
        [SerializeField, FoldoutGroup("Settings"), LabelText("@SlideText"), LabelWidth(150), Min(0.01f)] protected float _slideDuration = 0.5f;
        [FoldoutGroup("Callbacks")] public UnityEvent OnDoneSliding;
        [FoldoutGroup("Callbacks")] public UnityEvent OnResetToDefault;

        private string SlideText => _isSpeedBased ? "Slide Speed (units/sec)" : "Slide Duration (sec)";

        private Tween _tween;
        private Vector2 _targetPivot;

        [Button("    Slide Up    ", ButtonAlignment = 0.5f, Stretch = false, Icon = SdfIconType.ArrowUp)]
        private void SlideUp() => SlideUp(1);

        [ButtonGroup, Button(Icon = SdfIconType.ArrowLeft)]
        private void SlideLeft() => SlideLeft(1);

        [ButtonGroup, Button("Reset", Icon = SdfIconType.ArrowCounterclockwise)]
        private void ResetPivot() => ResetToDefault();

        [ButtonGroup, Button(Icon = SdfIconType.ArrowRight, IconAlignment = IconAlignment.RightOfText)]
        private void SlideRight() => SlideRight(1);

        [Button("   Slide Down   ", ButtonAlignment = 0.5f, Stretch = false, Icon = SdfIconType.ArrowDown)]
        private void SlideDown() => SlideDown(1);
        
        public Tween SlideTo(Vector2 pivot, bool isLocal = false, bool instant = false)
        {
            _targetPivot = isLocal ? _targetPivot + pivot : pivot;
            _tween?.Kill();
            
            if (instant)
            {
                RectTransform.pivot = _targetPivot;
                return null;
            }

            return _tween = RectTransform.DOPivot(_targetPivot, _slideDuration)
                .SetSpeedBased(_isSpeedBased)
                .SetEase(_slideEase)
                .OnComplete(OnDoneSliding.Invoke);
        }

        public Tween SlideUp(int amount = 1, bool instant = false) => SlideTo(amount * Vector2.down, isLocal: true, instant);
        public Tween SlideLeft(int amount = 1, bool instant = false) => SlideTo(amount * Vector2.right, isLocal: true, instant);
        public Tween SlideRight(int amount = 1, bool instant = false) => SlideTo(amount * Vector2.left, isLocal: true, instant);
        public Tween SlideDown(int amount = 1, bool instant = false) => SlideTo(amount * Vector2.up, isLocal: true, instant);
        public Tween ResetToDefault(bool instant = true) => SlideTo(_defaultPivotPos, instant: instant).OnComplete(OnResetToDefault.Invoke);

        private void OnDefaultAdjusted(TextAnchor pivot)
        {
            _defaultPivotPos = pivot.ToVector2();
            RectTransform.pivot = _defaultPivotPos;
        }
    }
}
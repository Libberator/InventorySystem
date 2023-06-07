using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

namespace Utilities.UI
{
    public class NotificationWindow : MonoBehaviour
    {
        [Header("References")] // Note: not all of these references are *required*
        [SerializeField] private PanelPivoter _pivoter;
        [SerializeField] private PanelSlider _slider;
        [SerializeField] private PanelFader _fader;
        [SerializeField] private TMP_Text _messageText;

        [Header("Notification Settings")]
        [SerializeField] private float _shownDuration = 4f;
        private Action<NotificationWindow> _onHideCallback;

        public PanelPivoter Pivoter => _pivoter;
        public PanelSlider Slider => _slider;
        public PanelFader Fader => _fader;

        private Coroutine _coroutine;

        public void Init(Action<NotificationWindow> onHideCallback)
        {
            _onHideCallback = onHideCallback;
        }

        public virtual void ShowNotification(string message) => ShowNotification(message, _shownDuration);
        public virtual void ShowNotification(string message, float shownDuration)
        {
            _messageText.SetText(message);
            Show();

            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = this.DelayThenDo(shownDuration, Hide); // custom extension method
        }

        protected virtual void Show()
        {
            if (_slider != null) _slider.Show(restart: true);
            if (_fader != null) _fader.Show(restart: true);
        }

        protected virtual void Hide()
        {
            if (_slider != null) _slider.Hide();
            if (_fader != null) _fader.Hide().OnComplete(() => _onHideCallback?.Invoke(this));
        }
    }
}

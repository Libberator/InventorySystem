using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.UI
{
    public enum NotificationType { General = 0, Warning = 1, Error = 2, }

    public class NotificationWindow : MonoBehaviour
    {
        [Header("Object References")]
        [SerializeField] private PanelSlider _panelAnimator;
        [SerializeField] private TMP_Text _notificationText;
        [SerializeField] Image _notifImage;

        [Header("Notification Settings")]
        [SerializeField] private float _defaultDisappearTime = 4f;

        private readonly Queue<Notification> _notificationQueue = new();
        private bool _notifCurrentlyDisplayed = false;

        public void SendNotification(string message) => SendNotification(message, NotificationType.General, _defaultDisappearTime);
        public void SendNotification(string message, NotificationType type) => SendNotification(message, type, _defaultDisappearTime);
        public void SendNotification(string message, NotificationType type, float disappearTime)
        {
            _notificationQueue.Enqueue(new Notification(message, type, disappearTime));
            CheckForNotificationToDisplay();
        }

        public void OnNotificationHideComplete() // assigned to OnHideComplete in the PanelAnimator
        {
            _notifCurrentlyDisplayed = false;
            CheckForNotificationToDisplay();
        }

        private void CheckForNotificationToDisplay()
        {
            if (_notificationQueue.Count > 0 && !_notifCurrentlyDisplayed)
                Show(_notificationQueue.Dequeue());
        }

        private void Show(Notification notif)
        {
            _notifCurrentlyDisplayed = true;
            _notificationText.SetText(notif.Message);
            _notificationText.color = notif.Type == NotificationType.Warning ? Color.black : Color.white;

            _panelAnimator.Show();
            Invoke(nameof(Hide), notif.DelayTime);
        }

        private void Hide() => _panelAnimator.Hide();

        private readonly struct Notification
        {
            public readonly string Message;
            public readonly NotificationType Type;
            public readonly float DelayTime;

            public Notification(string message, NotificationType type, float delay)
            {
                Message = message;
                Type = type;
                DelayTime = delay;
            }
        }
    }
}

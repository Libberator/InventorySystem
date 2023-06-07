using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Utilities.UI
{
    /// <summary>
    /// Acts like a Vertical Layout Group without the "snappiness" that those have.
    /// </summary>
    public class NotificationQueue : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _maxDisplayed = 8;
        [SerializeField] private bool _spawnBelowPrevious = true;

        [Header("References")]
        [SerializeField] private NotificationWindow _notificationPrefab;

        private readonly List<NotificationWindow> _activeWindows = new();
        private ObjectPool<NotificationWindow> _pool;

        private void Awake() => InitializePool();

        [Button]
        public void SendNotification(string message)
        {
            if (_activeWindows.Count == _maxDisplayed)
                ReturnCallback(_activeWindows.First());
            
            var notif = _pool.Get();
            AdjustStartingPivot(notif);
            
            
            notif.ShowNotification(message);
        }

        private void ReturnCallback(NotificationWindow notification)
        {
            _pool.Release(notification);

            SlideActiveWindows();
        }

        private void AdjustStartingPivot(NotificationWindow notification)
        {
            if (_activeWindows.Count <= 1) return;
            
            if (_spawnBelowPrevious)
                notification.Pivoter.SlideDown(_activeWindows.Count - 1, instant: true);
            else
                notification.Pivoter.SlideUp(_activeWindows.Count - 1, instant: true);
        }

        private void SlideActiveWindows()
        {
            foreach (var notif in _activeWindows)
            {
                if (_spawnBelowPrevious)
                    notif.Pivoter.SlideUp();
                else
                    notif.Pivoter.SlideDown();
            }
        }

        #region Pool Methods

        private void InitializePool()
        {
            _pool = new ObjectPool<NotificationWindow>(
                createFunc: Create,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDispose,
                collectionCheck: false, // ignore calling Release on an item in the pool
                defaultCapacity: _maxDisplayed);
        }

        private NotificationWindow Create()
        {
            var notification = Instantiate(_notificationPrefab, transform);
            notification.Init(ReturnCallback);
            return notification;
        }

        private void OnGet(NotificationWindow notification)
        {
            notification.gameObject.SetActive(true);
            notification.Pivoter.ResetToDefault();
            _activeWindows.Add(notification);
        }

        private void OnRelease(NotificationWindow notification)
        {
            _activeWindows.Remove(notification);
            notification.gameObject.SetActive(false);
        }

        private void OnDispose(NotificationWindow notification) => 
            Destroy(notification.gameObject);

        #endregion
    }
}

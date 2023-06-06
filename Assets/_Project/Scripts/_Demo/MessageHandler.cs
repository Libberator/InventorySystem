using TMPro;
using UnityEngine;
using Utilities.MessageSystem;
using Utilities.UI;

namespace InventorySystem.Demo
{
    public class MessageHandler : MonoBehaviour
    {
        [SerializeField] private PanelAnimator _notification;
        [SerializeField] private TextMeshProUGUI _notificationText;

        private void OnEnable()
        {
            Messenger.AddListener<InventoryMessage>(OnInventoryMessage);
        }

        private void OnDisable()
        {
            Messenger.RemoveListener<InventoryMessage>(OnInventoryMessage);
        }

        private void OnInventoryMessage(InventoryMessage message)
        {
            Debug.Log($"[{message.Event}]: {message.Message}");
        }
    }
}

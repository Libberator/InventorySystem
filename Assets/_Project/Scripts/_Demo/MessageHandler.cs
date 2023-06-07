using UnityEngine;
using Utilities.MessageSystem;
using Utilities.UI;

namespace InventorySystem.Demo
{
    public class MessageHandler : MonoBehaviour
    {
        [SerializeField] private NotificationQueue _queue;

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
            switch (message.Event)
            {
                case InventoryEvent.ItemAddSuccess:
                    _queue.SendNotification($"+{message.Quantity} {message.Item.ColoredName}");
                    break;
                case InventoryEvent.ItemRemoveSuccess:
                    _queue.SendNotification($"-{message.Quantity} {message.Item.ColoredName}");
                    break;
                default:
                    Debug.Log($"[{message.Event}] {message.Item.Name} ({message.Quantity})");
                    break;
            }
        }

        //private void OnAbilityMessages(AbilityMessage message)
        //{

        //}

    }
}

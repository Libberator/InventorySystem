using AbilitySystem;
using UnityEngine;
using Utilities.MessageSystem;
using Utilities.UI;

namespace InventorySystem.Demo
{
    public class MessageHandler : MonoBehaviour
    {
        [SerializeField] private NotificationQueue _inventoryNotifications;

        private void OnEnable()
        {
            Messenger.AddListener<InventoryMessage>(OnInventoryMessage);
            Messenger.AddListener<AbilityMessage>(OnAbilityMessage);
        }

        private void OnDisable()
        {
            Messenger.RemoveListener<InventoryMessage>(OnInventoryMessage);
            Messenger.RemoveListener<AbilityMessage>(OnAbilityMessage);
        }

        private void OnInventoryMessage(InventoryMessage message)
        {
            switch (message.Event)
            {
                case InventoryEvent.ItemAddSuccess:
                    _inventoryNotifications.SendNotification($"+{message.Quantity} {message.Item.ColoredName}");
                    break;
                case InventoryEvent.ItemRemoveSuccess:
                    _inventoryNotifications.SendNotification($"-{message.Quantity} {message.Item.ColoredName}");
                    break;
                default:
                    Debug.Log($"[{message.Event}] {message.Item.Name} ({message.Quantity})");
                    break;
            }
        }

        private void OnAbilityMessage(AbilityMessage message)
        {
            switch (message.Event)
            {
                //case AbilityEvent.None:
                //    break;
                //case AbilityEvent.SuccessfulCast:
                //    break;
                //case AbilityEvent.NotEnoughMana:
                //    break;
                //case AbilityEvent.OnCooldown:
                //    Debug.Log("On Cooldown");
                //    break;
                default:
                    Debug.Log($"[{message.Event}] {message.Ability.Name}");
                    break;
            }
        }
    }
}

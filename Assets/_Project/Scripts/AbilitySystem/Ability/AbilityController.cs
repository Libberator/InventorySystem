using UnityEngine;
using Utilities.MessageSystem;

namespace AbilitySystem
{
    public class AbilityController : MonoBehaviour
    {
        private Mana _playerMana;

        private void OnEnable()
        {
            AbilitySlot.Pressed += OnAbilityPressed;
            AbilitySlot.UpgradePressed += OnUpgradePressed;
        }

        private void OnDisable()
        {
            AbilitySlot.Pressed -= OnAbilityPressed;
            AbilitySlot.UpgradePressed -= OnUpgradePressed;
        }

        private void Start()
        {
            _playerMana = FindObjectOfType<Mana>();
        }

        private void OnAbilityPressed(AbilitySlot slot)
        {
            if (slot.CooldownView.Cooldown.IsActive)
            {
                Messenger.SendMessage(new AbilityMessage(slot.Ability, AbilityEvent.OnCooldown));
                return;
            }

            if (_playerMana.Meter.Value < slot.Ability.Cost)
            {
                Messenger.SendMessage(new AbilityMessage(slot.Ability, AbilityEvent.NotEnoughMana));
                return;
            }

            _playerMana.Meter.Decrease(slot.Ability.Cost);
            slot.CooldownView.Cooldown.Start();
            Messenger.SendMessage(new AbilityMessage(slot.Ability, AbilityEvent.SuccessfulCast));
        }

        private void OnUpgradePressed(AbilitySlot slot)
        {
            if (!slot.Ability.CanBeUpgraded)
            {
                Messenger.SendMessage(new AbilityMessage(slot.Ability, AbilityEvent.UpgradeNotAllowed));
                return;
            }

            // TODO: verify that we can upgrade that slot
            // should actually pre-verify so that slot couldn't be clicked on in the first place

            // apply the new upgrade
            Messenger.SendMessage(new AbilityMessage(slot.Ability, AbilityEvent.AbilityUpgraded));
        }
    }
}

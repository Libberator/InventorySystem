using AbilitySystem;
using CombatSystem;
using InventorySystem;
using UnityEngine;
using Utilities.Cooldown;
using Utilities.MessageSystem;
using static UnityEngine.EventSystems.EventTrigger;

namespace SystemsDemo
{
    public class Player : MonoBehaviour
    {
        public Health Health;
        public Mana Mana;
        public StatSheet StatSheet;
        public InventoryController Inventory;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnEnable()
        {
            ItemEntryMenu.UseClicked += OnConsumableUsed;
            ItemEntryMenu.EquipClicked += OnEquipClicked;
        }

        private void OnDisable()
        {
            ItemEntryMenu.UseClicked -= OnConsumableUsed;
            ItemEntryMenu.EquipClicked -= OnEquipClicked;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Inventory.Toggle();
            }
        }

        private void OnConsumableUsed(ItemEntry entry)
        {
            var consumable = entry.Item as Consumable;

            var cooldown = Cooldown.Get(consumable);
            if (cooldown.IsActive)
            {
                Messenger.SendMessage(new InventoryMessage(entry.Item, entry.Quantity, InventoryEvent.ConsumableOnCooldown));
                return;
            }

            foreach (var statMod in consumable.StatModifiers)
            {
                if (statMod.Stat == Health.CurrentHP)
                    Health.Heal((int)statMod.Value);
                else if (statMod.Stat == Health.MaxHP)
                    Health.IncreaseMaxHP((int)statMod.Value);
                else if (statMod.Stat == Mana.CurrentMana)
                    Mana.RestoreMana((int)statMod.Value);
                else if (statMod.Stat == Mana.MaxMana)
                    Mana.IncreaseMaxMana((int)statMod.Value);
            }

            Messenger.SendMessage(new InventoryMessage(entry.Item, entry.Quantity, InventoryEvent.ConsumableUseSuccess));
            entry.RemoveQuantity(1);
            cooldown.Start();
        }

        private void OnEquipClicked(ItemEntryView view)
        {
            Debug.Log("[Player] Equip Clicked");
        }
    }
}

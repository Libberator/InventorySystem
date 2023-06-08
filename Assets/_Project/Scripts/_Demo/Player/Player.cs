using AbilitySystem;
using CombatSystem;
using InventorySystem;
using UnityEngine;

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Inventory.Toggle();
            }
        }
    }
}

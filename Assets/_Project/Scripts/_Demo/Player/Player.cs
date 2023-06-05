using CombatSystem;
using UnityEngine;

namespace InventorySystem.Demo
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
    }
}

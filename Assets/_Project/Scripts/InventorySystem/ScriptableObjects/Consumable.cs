using UnityEngine;

namespace InventorySystem
{
    // some enum for what type of consumable - permanent, temporary, conditional

    [CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory System/Consumable")]
    public class Consumable : Item
    {
        public float Cooldown = 0.5f;
        // public int Charges = 1;

        public StatModifier[] StatModifiers;


        public void Use()
        {
            Debug.Log($"Used {Name}");
        }
    }
}
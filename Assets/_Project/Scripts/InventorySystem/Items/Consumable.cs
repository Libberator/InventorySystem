using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory System/Consumable")]
    public class Consumable : Item
    {

        public float Cooldown = 0.5f;

        public int Charges = 1;

        public virtual void Use()
        {
            Debug.Log($"Used {Name}");
        }
    }
}
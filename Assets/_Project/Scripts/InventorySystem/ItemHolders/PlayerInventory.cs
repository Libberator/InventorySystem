using UnityEngine;

namespace InventorySystem
{
    public class PlayerInventory : Inventory
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventory();
            }
        }
    }
}

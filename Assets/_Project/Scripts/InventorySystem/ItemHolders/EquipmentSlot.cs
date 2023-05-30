namespace InventorySystem
{
    public class EquipmentSlot : ItemSlot
    {
        public EquipmentType SlotType;

        protected void OnValidate()
        {
            gameObject.name = SlotType.ToString() + " Slot";
        }
    }
}

namespace InventorySystem
{
    public class EquipmentSlot : ItemEntryView
    {
        public EquipmentType SlotType;

        protected void OnValidate()
        {
            gameObject.name = SlotType.ToString() + " Slot";
        }
    }
}

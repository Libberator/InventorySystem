namespace InventorySystem
{
    public class PlayerStatSheet : StatSheet
    {
        private void Awake()
        {
            ServiceLocator.Register<StatSheet>(this);
        }
    }
}

using UnityEngine;
using Utilities.UI;

namespace InventorySystem
{
    public class PlayerInventory : Inventory
    {
        private ConfirmationDialog _confirmationDialog;

        protected override void Awake()
        {
            base.Awake();
            ServiceLocator.Register<Inventory>(this);
        }

        private void Start() => _confirmationDialog = ServiceLocator.Get<ConfirmationDialog>();

        // TODO: Switch to alternative forms of Input instead of relying directly on the legacy Input system
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I) && !_confirmationDialog.IsActive)
                ToggleInventory();
        }
    }
}

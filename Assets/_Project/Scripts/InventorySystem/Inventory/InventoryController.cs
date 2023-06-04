using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Utilities.UI;

namespace InventorySystem
{
    // Chest, Storage, Backpack, Misc Lootdrop, etc.
    public class InventoryController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Inventory _inventory = new();
        [SerializeField] private InventoryView _inventoryView; // prefab, reference, or use a Factory approach

        [Header("Starting Items")]
        [SerializeField] private bool _isPlayerInventory = false;
        [SerializeField] private int _inventorySize = 12;
        [SerializeField] private List<ItemEntry> _startingItems = new();

        private ConfirmationDialog _confirmationDialog;
        private void Start()
        {
            _confirmationDialog = ServiceLocator.Get<ConfirmationDialog>();
            Sync();
        }

        [Button(ButtonSizes.Large)]
        private void Sync()
        {
            ApplyStartingItems();
            BindToView();
        }

        [ButtonGroup]
        public void ApplyStartingItems() => Initialize(_startingItems, _inventorySize, _isPlayerInventory);

        public void Initialize(List<ItemEntry> startingItems = null, int size = 12, bool isPlayerInventory = false) => 
            _inventory = new(startingItems, size, isPlayerInventory);

        [ButtonGroup]
        public void BindToView() => _inventoryView.BindTo(_inventory);

        public void Open()
        {
            if (_confirmationDialog.IsActive) return;
            _inventoryView.OpenInventory();
        }

        public void Close()
        {
            if (_confirmationDialog.IsActive) return;
            _inventoryView.CloseInventory();
        }

        // TODO: make the input connect to the Player's InventoryView another way
        // TODO: Switch to alternative forms of Input instead of relying directly on the legacy Input system
        private void Update()
        {
            if (!_isPlayerInventory) return;
            if (Input.GetKeyDown(KeyCode.I) && !_confirmationDialog.IsActive)
                _inventoryView.ToggleInventory();
        }
    }
}

using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    // Chest, Storage, Backpack, Misc Lootdrop, etc.
    public class InventoryController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Inventory _inventory = new();
        [SerializeField] private InventoryView _inventoryView;

        [Header("Starting Items")]
        [SerializeField] private bool _isPlayerInventory = false;
        [SerializeField] private int _inventorySize = 12;
        [SerializeField] private List<ItemEntry> _startingItems = new();

        private void Awake() => Sync();
        
        [Button(ButtonSizes.Large)]
        public void Sync()
        {
            ApplyStartingItems();
            BindToView();
        }
        
        public void Initialize(List<ItemEntry> startingItems = null, int size = 12, bool isPlayerInventory = false) => 
            _inventory = new(startingItems, size, isPlayerInventory);

        [ButtonGroup("Syncing", ButtonHeight = 25)]
        public void ApplyStartingItems() => Initialize(_startingItems, _inventorySize, _isPlayerInventory);

        [ButtonGroup("Syncing", ButtonHeight = 25)]
        public void BindToView() => _inventoryView.BindTo(_inventory);

        [ButtonGroup("View", ButtonHeight = 20)]
        public void Toggle() => _inventoryView.ToggleInventory();
        
        [ButtonGroup("View", ButtonHeight = 20)]
        public void Open() => _inventoryView.OpenInventory();
        
        [ButtonGroup("View", ButtonHeight = 20)]
        public void Close() => _inventoryView.CloseInventory();
    }
}

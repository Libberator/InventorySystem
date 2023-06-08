using UnityEngine;
using Utilities.UI;

namespace InventorySystem
{
    public class InventoryViewToggler : MonoBehaviour
    {
        [SerializeField] private InventoryView _view;
        [SerializeField] private SpriteButton _openChest;
        [SerializeField] private SpriteButton _closedChest;

        private void OnEnable()
        {
            InventoryView.Opened += OnInventoryOpened;
            InventoryView.Closed += OnInventoryClosed;
            _closedChest.OnClick.AddListener(Open);
            _openChest.OnClick.AddListener(Close);
        }

        private void OnDisable()
        {
            InventoryView.Opened -= OnInventoryOpened;
            InventoryView.Closed -= OnInventoryClosed;
            _closedChest.OnClick.RemoveListener(Open);
            _openChest.OnClick.RemoveListener(Close);
        }

        private void Open() => _view.OpenInventory();

        private void Close() => _view.CloseInventory();

        private void OnInventoryOpened(InventoryView view)
        {
            if (view != _view) return;
            _openChest.gameObject.SetActive(true);
            _closedChest.gameObject.SetActive(false);
        }

        private void OnInventoryClosed(InventoryView view)
        {
            if (view != _view) return;
            _closedChest.gameObject.SetActive(true);
            _openChest.gameObject.SetActive(false);
        }

        private void OnValidate()
        {
            if (_view == null)
                _view = transform.root.GetComponentInChildren<InventoryView>();
        }
    }
}

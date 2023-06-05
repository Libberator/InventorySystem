using UnityEngine;

namespace InventorySystem
{
    public class InventoryViewToggler : MonoBehaviour
    {
        [SerializeField] private InventoryView _view;
        [SerializeField] private GameObject _open;
        [SerializeField] private GameObject _closed;

        private void OnEnable()
        {
            InventoryView.Opened += OnInventoryOpened;
            InventoryView.Closed += OnInventoryClosed;
        }

        private void OnDisable()
        {
            InventoryView.Opened -= OnInventoryOpened;
            InventoryView.Closed -= OnInventoryClosed;
        }

        private void OnInventoryOpened(InventoryView view)
        {
            if (view != _view) return;
            _closed.SetActive(false);
            _open.SetActive(true);
        }

        private void OnInventoryClosed(InventoryView view)
        {
            if (view != _view) return;
            _open.SetActive(false);
            _closed.SetActive(true);
        }

        private void OnValidate()
        {
            if (_view == null)
                _view = transform.root.GetComponentInChildren<InventoryView>();
        }
    }
}

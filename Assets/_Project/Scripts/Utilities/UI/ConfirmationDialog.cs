using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.UI
{
    public class ConfirmationDialog : MonoBehaviour
    {
        public event Action<bool> DialogActiveChanged;

        [Header("Mandatory References")]
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button _confirmButton;
        //[SerializeField] private TextMeshProUGUI _confirmText; // in case you want to customize the button text?
        [SerializeField] private Button _cancelButton;
        //[SerializeField] private TextMeshProUGUI _cancelText; // in case you want to customize the button text?
        [SerializeField] private Toggle _bypassToggle;
        [SerializeField] private GameObject _toggleContainer;

        [Header("For Animations")]
        [SerializeField] private PanelFader _fader;
        [SerializeField] private PanelScaler _scaler;

        private Action _confirmCallback;
        private Action _cancelCallback;

        private static readonly HashSet<string> _bypassed = new();
        private string _currentBypassGroup;
        private bool _bypassDisplayed;

        private void Awake() => ServiceLocator.Register(this);

        #region Public Methods & Properties

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            private set
            {
                _isActive = value;
                DialogActiveChanged?.Invoke(_isActive);
            }
        }

        /// <summary>
        /// Give similar requests the same "<paramref name="bypassGroup"/> title" to check if we've chosen to automatically accept for the day.
        /// </summary>
        public void AskWithBypass(string bypassGroup, string message, Action onConfirm, Action onCancel)
        {
            if (_bypassed.Contains(bypassGroup))
            {
                onConfirm?.Invoke();
                return;
            }
            _currentBypassGroup = bypassGroup;
            _messageText.SetText(message);
            _confirmCallback = onConfirm;
            _cancelCallback = onCancel;

            ShowDialog(withBypass: true);
        }

        /// <summary>
        /// This version won't ask to bypass the confirmation.
        /// </summary>
        public void Ask(string message, Action onConfirm, Action onCancel)
        {
            _messageText.SetText(message);
            _confirmCallback = onConfirm;
            _cancelCallback = onCancel;

            ShowDialog(withBypass: false);
        }

        #endregion

        #region Animations

        [Button]
        private void ShowDialog(bool withBypass = false)
        {
            _confirmButton.interactable = true;
            _cancelButton.interactable = true;

            _bypassDisplayed = withBypass;
            _toggleContainer.SetActive(withBypass);
            _bypassToggle.isOn = false;

            _fader.Show();
            _scaler.Show();

            IsActive = true;
        }

        [Button]
        private void CloseDialog()
        {
            _confirmButton.interactable = false;
            _cancelButton.interactable = false;

            if (_bypassDisplayed && _bypassToggle.isOn)
            {
                _bypassed.Add(_currentBypassGroup);
            }

            _fader.Hide();
            _scaler.Hide();

            IsActive = false;
        }

        #endregion

        #region UI Callbacks

        public void ConfirmClicked()
        {
            _confirmCallback?.Invoke();
            CloseDialog();
        }

        public void CancelClicked()
        {
            _cancelCallback?.Invoke();
            CloseDialog();
        }

        public void BypassToggled(bool isOn)
        {
            _cancelButton.interactable = !isOn;
        }

        #endregion
    }
}
using Sirenix.OdinInspector;
using System;
using TMPro;
using TooltipSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.Cooldown;

namespace AbilitySystem
{
    public class AbilitySlot : MonoBehaviour, IHaveTooltip, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        public static event Action<AbilitySlot> Pressed;
        public static event Action<AbilitySlot> UpgradePressed;

        [OnValueChanged(nameof(OnAbilityChanged))]
        [SerializeField] private Ability _ability;
        [OnValueChanged(nameof(OnKeyCodeChanged))]
        [SerializeField] private KeyCode _key;

        [Header("UI References")]
        [SerializeField] private Image _icon;
        [SerializeField] private Image _radialFill;
        [SerializeField] private TextMeshProUGUI _keyText;
        [SerializeField] private CooldownView _cooldownView;

        public Ability Ability => _ability;
        public CooldownView CooldownView => _cooldownView;

        private void Start()
        {
            // TODO: Grab a reference to the Player(?) for access to Stats, Mana, etc.
            // Or have the AbilityHotbar grab them, and route ability usage through that...
            // What's the purpose of the Hotbar? I think just handle the skill-ups...
            BindTo(_ability);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_key))
                Pressed?.Invoke(this);
        }

        #region Bindings

        public void BindTo(Ability ability)
        {
            if (_ability != null) UnbindFromCurrent();
            if (ability == null) return;

            _ability = ability;

            OnAbilityChanged(_ability);
            _cooldownView.BindTo(ability);
        }

        private void UnbindFromCurrent()
        {
            OnAbilityChanged(null);
            _cooldownView.UnbindFromCurrent();
            _ability = null;
        }

        #endregion

        #region Updating UI

        private void OnAbilityChanged(Ability ability)
        {
            if (ability != null)
            {
                _icon.sprite = ability.Icon;
                _icon.enabled = true;
            }
            else
            {
                _icon.enabled = false;
            }
        }

        private void OnKeyCodeChanged(KeyCode keyCode)
        {
            _keyText.SetText(keyCode.ToString());
        }

        #endregion

        #region Interface Methods

        public void UpgradeButtonPressed() => UpgradePressed?.Invoke(this);

        public Tooltip GetTooltip() => _ability != null ? _ability.GetTooltip() : Tooltip.Empty;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_ability == null) return;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    UpgradeButtonPressed();
                else
                    Pressed?.Invoke(this);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
                Debug.Log($"Right clicked {_ability.Name}");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("Begin Drag");
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("On Drag");
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("On Drop");
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // if we're still holding an ability in the hand, return back to start
            Debug.Log("End Drag");
        }

        #endregion
    }
}

using Sirenix.OdinInspector;
using TMPro;
using TooltipSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    public class AbilitySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IHaveTooltip
    {
        [OnValueChanged(nameof(OnAbilityChanged))]
        [SerializeField] private Ability _ability;
        [OnValueChanged(nameof(OnKeyCodeChanged))]
        [SerializeField] private KeyCode _key;

        [Header("UI References")]
        [SerializeField] private Image _image;
        [SerializeField] private Image _radialFill;
        [SerializeField] private TextMeshProUGUI _keyText;
        [SerializeField] private TextMeshProUGUI _timerText;

        private void Start()
        {
            // TODO: Grab a reference to the Player(?) for access to Stats, Mana, etc.
            // Or have the AbilityHotbar grab them, and route ability usage through that...
            // What's the purpose of the Hotbar? I think just handle the skill-ups...
            if (_ability != null) BindTo(_ability);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_key))
                ActivateAbility();
        }

        public void BindTo(Ability ability)
        {
            if (_ability == ability) return;
            if (_ability != null) UnbindFrom(_ability);
            if (ability == null) return;

            _ability = ability;
            // subscribe to any cooldown-related events
            OnAbilityChanged(_ability);
            OnCooldownChanged(0);
        }

        private void UnbindFrom(Ability ability)
        {
            // unsubscribe from any cooldown listening

            //OnAbilityChanged(null); // might need this later
            OnCooldownChanged(0);
            _ability = null;
        }


        #region Ability Usage

        public bool CanActivateAbility => _ability != null;

        public void AbilityPressed()
        {
            // if we're on cooldown, give reject reason

            // if we don't have enough mana, give reject reason
            ActivateAbility();
        }


        private void ActivateAbility()
        {
            Debug.Log($"Ability activated: {_ability.Name}");
        }

        #endregion

        #region Updating UI

        // Make a Cooldown class, with a static Dictionary lookup for shared Cooldowns
        // BindTo Ability for keeping track of cooldown value

        private void OnAbilityChanged(Ability ability)
        {
            if (ability != null)
            {
                _image.sprite = ability.Icon;
                _image.enabled = true;
            }
            else
            {
                _image.enabled = false;
                //_radialFill.fillAmount = 0;
            }
        }

        private void OnCooldownChanged(float percent)
        {
            _radialFill.fillAmount = percent;
        }

        private void OnKeyCodeChanged(KeyCode keyCode)
        {
            _keyText.SetText(keyCode.ToString());
        }

        #endregion

        #region Button Methods

        public void SkillUpPressed()
        {
            Debug.Log($"Leveled up {_ability.Name}");
        }

        #endregion

        #region Interface Methods

        public void OnPointerClick(PointerEventData eventData)
        {
            // Shift-Click to Level-Up the ability if available
            AbilityPressed();
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

        public string GetTooltipText()
        {
            return _ability.GetTooltipText();
        }

        #endregion
    }
}

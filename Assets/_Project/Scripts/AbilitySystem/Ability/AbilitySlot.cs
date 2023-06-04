using Sirenix.OdinInspector;
using TMPro;
using TooltipSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AbilitySystem
{
    public class AbilitySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IHaveTooltip
    {
        [OnValueChanged(nameof(OnAbilityChanged))]
        [SerializeField] private Ability _ability;
        [OnValueChanged(nameof(OnKeyCodeChanged))]
        [SerializeField] private KeyCode _key;

        [Header("UI References")]
        [SerializeField] private Image _icon;
        [SerializeField] private Image _radialFill;
        [SerializeField] private TextMeshProUGUI _keyText;
        [SerializeField] private TextMeshProUGUI _timerText;
        private Cooldown _cooldownTimer;

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
                AbilityPressed();
        }

        public void BindTo(Ability ability)
        {
            if (_ability != null) UnbindFrom(_ability);
            if (ability == null) return;

            _ability = ability;

            OnAbilityChanged(_ability);

            // TODO: Avoid divide-by-zero. Handle what to do in case Cooldown Duration is 0
            _cooldownTimer = Cooldown.Get(ability);
            _cooldownTimer.Started += OnCooldownStarted;
            _cooldownTimer.Updated += OnCooldownUpdated;
            _cooldownTimer.Completed += OnCooldownCompleted;
            if (_cooldownTimer.IsActive) OnCooldownStarted(_cooldownTimer.Remaining);
            else OnCooldownCompleted();
        }

        private void UnbindFrom(Ability ability)
        {
            if (_cooldownTimer != null)
            {
                _cooldownTimer.Started -= OnCooldownStarted;
                _cooldownTimer.Updated -= OnCooldownUpdated;
                _cooldownTimer.Completed -= OnCooldownCompleted;
            }

            //OnAbilityChanged(null); // might need this later
            //OnCooldownChanged(0);
            _ability = null;
        }

        #region Ability Usage

        public void AbilityPressed()
        {
            if (_cooldownTimer.IsActive)
            {
                Debug.LogWarning($"Timer for {_ability.Name} is still active!");
                return;
            }
            // if we don't have enough mana, give reject reason

            ActivateAbility();
        }

        private void ActivateAbility()
        {
            Debug.Log($"Ability activated: {_ability.Name}");
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

        private void OnCooldownStarted(float remaining)
        {
            OnCooldownUpdated(remaining);
            _timerText.enabled = true;
        }

        private void OnCooldownUpdated(float remaining)
        {
            _radialFill.fillAmount = remaining / _ability.Cooldown;
            _timerText.SetText(remaining.ToString());
        }

        private void OnCooldownCompleted()
        {
            _radialFill.fillAmount = 0f;
            _timerText.enabled = false;
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

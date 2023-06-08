using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Cooldown
{
    public class CooldownView : MonoBehaviour
    {
        [SerializeField] private Image _radialFill;
        [SerializeField] private TextMeshProUGUI _timerText;

        private IHaveCooldown _source;

        private Cooldown _cooldown;
        public Cooldown Cooldown => _cooldown;

        public void BindTo(IHaveCooldown source)
        {
            UnbindFromCurrent();

            _source = source;

            _cooldown = Cooldown.Get(source);
            _cooldown.Started += OnCooldownStarted;
            _cooldown.Updated += OnCooldownUpdated;
            _cooldown.Completed += OnCooldownCompleted;
            if (_cooldown.IsActive) OnCooldownStarted(_cooldown.Remaining);
        }

        public void UnbindFromCurrent()
        {
            if (_cooldown != null)
            {
                _cooldown.Started -= OnCooldownStarted;
                _cooldown.Updated -= OnCooldownUpdated;
                _cooldown.Completed -= OnCooldownCompleted;
                _cooldown = null;
            }

            OnCooldownCompleted();
        }

        private void OnCooldownStarted(float remaining)
        {
            OnCooldownUpdated(remaining);
            _timerText.enabled = true;
        }

        private void OnCooldownUpdated(float remaining)
        {
            _radialFill.fillAmount = remaining / _source.Cooldown;
            _timerText.SetText(remaining.ToString("F1"));
        }

        private void OnCooldownCompleted()
        {
            _radialFill.fillAmount = 0f;
            _timerText.enabled = false;
        }
    }
}

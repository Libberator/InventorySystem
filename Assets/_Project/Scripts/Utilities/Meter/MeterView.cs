using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Meter
{
    /// <summary>
    /// This is the UI that listens to changes from Meter. A Health Bar, Mana Bar, XP, Stamina, etc.
    /// </summary>
    public class MeterView : SerializedMonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private MeterController _source;
        protected Meter _meter;

        [Header("UI References")]
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _text;

        protected virtual void Start()
        {
            if (_source == null)
                _source = GetComponentInParent<MeterController>();
            if (_source != null)
                BindTo(_source.Meter);
        }

        public virtual void BindTo(Meter meter)
        {
            if (_meter != null)
                UnbindFrom(_meter);

            _meter = meter;
            UpdateUI();

            _meter.ValueChanged += OnValueChanged;
            _meter.MaxChanged += OnMaxChanged;
        }

        public virtual void UnbindFrom(Meter meter)
        {
            meter.ValueChanged -= OnValueChanged;
            meter.MaxChanged -= OnMaxChanged;
        }

        protected virtual void OnValueChanged(Meter.MeterEventArgs args) => UpdateUI();
        protected virtual void OnMaxChanged(Meter.MeterEventArgs args) => UpdateUI();

        protected virtual void UpdateUI()
        {
            RefreshSlider();
            RefreshText();
        }

        protected virtual void RefreshSlider()
        {
            _slider.maxValue = _meter.Maximum;
            _slider.value = _meter.Value;
        }

        protected virtual void RefreshText()
        {
            if (_text != null)
                _text.SetText($"{_meter.Value}/{_meter.Maximum}");
        }
    }
}

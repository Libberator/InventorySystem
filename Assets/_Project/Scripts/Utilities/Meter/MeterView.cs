using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Utilities.Meter
{
    /// <summary>
    /// This is the UI that listens to changes from Meter. A Health Bar, Mana Bar, Stamina, etc.
    /// </summary>
    public class MeterView : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private MeterController _source;
        protected Meter _meter;

        [Header("UI References")]
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _text;

        protected virtual void Start()
        {
            // if we assigned in the inspector beforehand
            if (_source != null)
                BindTo(_source);
            // in case we Instantiate at runtime as a child
            else
            {
                var source = GetComponentInParent<IHaveMeter>();
                if (source != null)
                    BindTo(source);
            }
        }

        public virtual void BindTo(IHaveMeter source)
        {
            _meter = source.GetMeter();
            _meter.Refilled += OnRefilled;
            _meter.Depleted += OnDepleted;
            _meter.ValueChanged += OnValueChanged;
            _meter.MaxChanged += OnMaxChanged;
            UpdateUI();
        }

        protected virtual void UpdateUI()
        {
            RefreshSlider();
            RefreshText();
        }

        protected virtual void OnRefilled()
        {
            Debug.Log("We're full!");
        }

        protected virtual void OnDepleted()
        {
            Debug.Log("We're empty!");
        }

        protected virtual void OnValueChanged(Meter.MeterEventArgs args) => UpdateUI();

        protected virtual void OnMaxChanged(Meter.MeterEventArgs args) => UpdateUI();

        protected virtual void RefreshSlider()
        {
            _slider.maxValue = _meter.Maximum;
            _slider.value = _meter.Value;
        }

        protected virtual void RefreshText() => SetText(_meter.Value, _meter.Maximum);

        protected virtual void SetText(int value, int max) => _text.SetText($"{value}/{max}");
    }
}

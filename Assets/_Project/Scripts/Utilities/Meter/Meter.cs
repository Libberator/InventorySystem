using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Utilities.Meter
{
    [Serializable, InlineProperty]
    public class Meter
    {
        public event Action Refilled;
        public event Action Depleted;
        public event Action<MeterEventArgs> ValueChanged;
        public event Action<MeterEventArgs> MaxChanged;

        public Meter(int maximum) : this(maximum, maximum) { }
        public Meter(int maximum, int value)
        {
            Maximum = maximum;
            Value = value;
        }

        [HorizontalGroup, PropertyRange(0, "@_maximum"), HideLabel, LabelWidth(50)]
        [SerializeField] private int _value = 100;
        public int Value
        {
            get => _value;
            set
            {
                if (value > Maximum)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"Maximum Value is {Maximum}. Cannot set Value to: {value}");
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"Meter Value must be a positive number. Cannot set to: {value}");
                var args = new MeterEventArgs
                {
                    Amount = Math.Abs(_value - value),
                    Direction = value.CompareTo(_value)
                };
                _value = value;

                OnValueChanged(args);
                if (_value == 0) OnDepleted();
            }
        }

        [HorizontalGroup(60), LabelText("/"), LabelWidth(8)]
        [SerializeField, Min(1)] private int _maximum = 100;
        public int Maximum
        {
            get => _maximum;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"Meter maximum value must be greater than zero. Cannot set to: {value}");
                var args = new MeterEventArgs
                {
                    Amount = Math.Abs(_maximum - value),
                    Direction = value.CompareTo(_maximum)
                };
                _maximum = value;

                OnMaxChanged(args);
                if (value < Value) Value = Maximum;
            }
        }

        public float PercentageFilled => (float)Value / Maximum * 100;

        /// <summary>
        /// Returns any amount over the maximum that didn't get fully applied.
        /// </summary>
        public int Increase(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(
                    $"Meter increase amount must be a positive number. Cannot increase by {amount}.");

            var afterIncrease = Value + amount;

            if (afterIncrease >= Maximum)
            {
                Value = Maximum;
                OnRefilled();
                return afterIncrease - Maximum;
            }
            Value += amount;
            return 0;
        }

        /// <summary>
        /// Returns any amount beyond 0 (the lower bound) that didn't get fully subtracted.
        /// </summary>
        public int Decrease(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(
                    $"Meter decrease amount must be a positive number. Cannot decrease by {amount}.");
            if (amount > Value)
            {
                var remainder = amount - Value;
                Value = 0;
                return remainder;
            }
            Value -= amount;
            return 0;
        }

        public int Refill()
        {
            var required = Maximum - Value;
            Increase(required);
            return required;
        }

        public int Deplete()
        {
            var amountToDeplete = Value;
            Decrease(amountToDeplete);
            return amountToDeplete;
        }

        /// <summary>
        /// Returns the amount that Value gets adjusted by, positive or negative
        /// </summary>
        public int FillToPercent(float percent)
        {
            if (percent < 0)
                throw new ArgumentOutOfRangeException(nameof(percent),
                    $"Percent to fill to must be a positive number, cannot fill to {percent}%");
            if (percent > 100)
                throw new ArgumentOutOfRangeException(nameof(percent),
                    $"Cannot fill Meter over 100%. Cannot fill to {percent}% ");
            var desiredValue = (int)(percent * Maximum);

            if (desiredValue > Value)
            {
                var increase = desiredValue - Value;
                Increase(increase);
                return increase;
            }

            var decrease = Value - desiredValue;
            Decrease(decrease);
            return -decrease;
        }

        protected virtual void OnRefilled() => Refilled?.Invoke();

        protected virtual void OnDepleted() => Depleted?.Invoke();

        protected virtual void OnValueChanged(MeterEventArgs e) => ValueChanged?.Invoke(e);

        protected virtual void OnMaxChanged(MeterEventArgs e) => MaxChanged?.Invoke(e);

        public class MeterEventArgs : EventArgs
        {
            public int Amount { get; set; }
            public int Direction { get; set; }
        }
    }
}

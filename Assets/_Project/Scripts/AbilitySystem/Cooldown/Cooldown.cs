using System;
using System.Collections.Generic;

namespace AbilitySystem
{
    public class Cooldown
    {
        public event Action<float> Started;
        public event Action<float> Updated;
        public event Action Completed;

        private readonly float _duration;
        private float _remaining = 0f;

        public bool IsActive => _remaining > 0f;
        public float Remaining => _remaining;

        public Cooldown(float duration, bool start = false)
        {
            _duration = duration;
            if (start) Start();
        }

        public void Start()
        {
            if (_activeCooldowns.Contains(this)) return;
            _remaining = _duration;
            _activeCooldowns.Add(this);
            Started?.Invoke(_remaining);
        }

        public void Update(float deltaTime)
        {
            _remaining -= deltaTime;
            Updated?.Invoke(_remaining);
            if (_remaining <= 0f) Complete();
        }

        public void Complete()
        {
            _activeCooldowns.Remove(this);
            Completed?.Invoke();
        }

        #region Static Helpers

        private static readonly Dictionary<IHaveCooldown, Cooldown> _lookup = new();
        private static readonly List<Cooldown> _activeCooldowns = new();

        public static Cooldown Get(IHaveCooldown cooldownSource, bool start = false)
        {
            if (_lookup.TryGetValue(cooldownSource, out var cooldown))
            {
                if (start) cooldown.Start();
                return cooldown;
            }
            cooldown = new Cooldown(cooldownSource.Cooldown, start);
            _lookup[cooldownSource] = cooldown;
            return cooldown;
        }

        // Call this method periodically from 1 source to decrement cooldowns.
        // Just make sure we're consistent *when* this is called:
        // before any Updates, or after, just not in-between other stuff (i.e. "up to the engine")
        // LateUpdate seems like an okay option. Perhaps w/ unscaledDeltaTime if pauses shouldn't affect it
        public static void UpdateCooldowns(float deltaTime)
        {
            for (int i = _activeCooldowns.Count - 1; i >= 0; i--)
                _activeCooldowns[i].Update(deltaTime);
        }

        #endregion
    }
}

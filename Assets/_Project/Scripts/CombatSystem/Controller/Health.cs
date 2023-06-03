using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.Meter;

namespace CombatSystem
{
    public class Health : MonoBehaviour, IHaveMeter, IHaveHP
    {
        [field: SerializeField] public Meter Meter { get; private set; }

        // TODO: have a reference to the player stats, buffs/debuffs, which routes the combat calculations

        [Button]
        public void Damage(int amount)
        {
            // TODO: take into account damage source,
            // calculate any mitigation from stats like armor or a "split/share the pain" ally buff
            // then apply the Decrease to the meter
            Meter.Decrease(amount);
            // then return a DamageResult so they know how much dmg they dealt - for stat tracking
        }

        [Button]
        public void Heal(int amount)
        {
            // TODO: take into account healing source,
            // calculate any mitigation from healing debuffs (i.e. "grievous wounds")
            // then apply the Increase to the meter
            Meter.Increase(amount);
            // then return a HealingResult(?) so they know how much healing they've done - for stat tracking
        }
    }
}

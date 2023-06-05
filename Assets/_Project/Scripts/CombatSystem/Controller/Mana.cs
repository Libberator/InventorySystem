using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.Meter;

namespace CombatSystem
{
    public class Mana : MonoBehaviour, IHaveMeter, IHaveMana
    {
        [SerializeField] private Meter _meter;
        public Meter Meter => _meter;

        // TODO: have a reference to the player stats, buffs/debuffs

        [Button]
        public void UseMana(int amount)
        {
            Meter.Decrease(amount);
            // do anything with this for stat tracking?
        }

        [Button]
        public void RestoreMana(int amount)
        {
            // calculate with any buffs/debuffs that might change the amount
            // then apply the Increase to the meter
            Meter.Increase(amount);
        }
    }
}

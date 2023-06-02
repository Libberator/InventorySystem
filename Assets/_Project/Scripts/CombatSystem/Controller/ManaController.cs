using Sirenix.OdinInspector;
using Utilities.Meter;

namespace CombatSystem
{
    public class ManaController : MeterController, IHaveMana
    {
        // TODO: have a reference to the player stats, buffs/debuffs

        [Button]
        public void UseMana(int amount)
        {
            _meter.Decrease(amount);
            // do anything with this for stat tracking?
        }

        [Button]
        public void RestoreMana(int amount)
        {
            // calculate with any buffs/debuffs that might change the amount
            // then apply the Increase to the meter
            _meter.Increase(amount);
        }
    }
}

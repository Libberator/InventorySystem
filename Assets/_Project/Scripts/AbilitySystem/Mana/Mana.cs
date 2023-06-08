using Sirenix.OdinInspector;
using Utilities.Meter;

namespace AbilitySystem
{
    public class Mana : MeterController, IHaveMana
    {
        // TODO: have a reference to the player stats, buffs/debuffs

        public bool CanSpendMana(int amount) => amount >= Meter.Value;

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

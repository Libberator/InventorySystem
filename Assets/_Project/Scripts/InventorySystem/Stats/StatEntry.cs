using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [Serializable]
    [InlineProperty]
    public class StatEntry
    {
        public event Action<float> ValueChanged;

        [HorizontalGroup, HideLabel]
        [SerializeField] private Stat _stat;

        // intrinsic value, level-based typically
        [HorizontalGroup(40), HideLabel]
        [SerializeField] private float _baseValue;

        // buffs, debuffs, equipment, etc.
        [ReadOnly]
        [SerializeField] protected List<StatModifier> _statModifiers = new();

        protected bool _isDirty = true;
        private float _finalValue;

        public Stat Stat => _stat;

        public float BaseValue
        {
            get => _baseValue;
            set
            {
                _baseValue = value;
                _isDirty = true;
            }
        }

        public float FinalValue
        {
            get // may fire event when polled
            {
                if (_isDirty)
                    FinalValue = CalculateFinalValue();
                return _finalValue;
            }
            set
            {
                if (_finalValue != value)
                {
                    _finalValue = value;
                    ValueChanged?.Invoke(_finalValue);
                }
            }
        }

        public StatEntry() : this(null, 0) { }
        public StatEntry(Stat stat, int baseValue)
        {
            _stat = stat;
            _baseValue = baseValue;
        }

        #region Adding & Removing StatModifiers

        public virtual void AddModifier(StatModifier mod)
        {
            _isDirty = true;
            _statModifiers.Add(mod);
        }

        public virtual bool RemoveModifier(StatModifier mod)
        {
            if (_statModifiers.Remove(mod))
            {
                _isDirty = true;
                return true;
            }
            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            int qtyRemoved = _statModifiers.RemoveAll(mod => mod.Source == source);

            if (qtyRemoved > 0)
            {
                _isDirty = true;
                return true;
            }
            return false;
        }

        #endregion

        #region Calculating Stat Values

        protected virtual float CalculateFinalValue()
        {
            float flatAdd = 0f;
            float sumPercentAdd = 0f;
            float percentMultiplier = 1f;

            foreach (var mod in _statModifiers)
            {
                switch (mod.ModType)
                {
                    case StatModType.Flat:
                        flatAdd += mod.Value;
                        break;
                    case StatModType.PercentAdd:
                        sumPercentAdd += mod.Value;
                        break;
                    case StatModType.PercentMult:
                        percentMultiplier *= mod.Value;
                        break;
                    default:
                        break;
                }
            }

            var finalValue = (flatAdd + BaseValue) * percentMultiplier * sumPercentAdd;

            _isDirty = false;

            // Workaround for float calculation errors, like displaying 12.00001 instead of 12
            return (float)Math.Round(finalValue, 4);
        }

        #endregion
    }
}

// needs to know about equipments, buffs, debuffs, etc.

// potential angles:
// strength, agility, intelligence/wisdom, charisma, vitality
// constitution, poise (knockback/stun/slow resist)

// elemental, true damage

// attack damage, magic damage, armor, magic resistance,
// armor pen, magic pen, critical strike change/damage, attack speed,
// movement speed, health regen, mana regen, lifesteal, spellvamp

// health (not sure if it counts as a "stat"), mana



// source: https://leagueoflegends.fandom.com/wiki/Champion_statistic

// Statistic = bonus + base + g * (n - 1) * (0.7025 + 0.0175 * (n - 1))
// bonus = bonus statistic from items, runes, abilities, or buffs
// base = initial statistic value
// g = growth statistic (amount per level)
// n = champion level
// (n - 1) = total amount of level ups

// extra source: https://leagueoflegends.fandom.com/wiki/Armor

// From LoL on stacking multiplicitively
//* const initial = 0.1; // Current Armor Penetration: 10%
//* const value = 0.1; // Additional Armor Penetration: 10%
//* const result = multiplicative(initial, value); // 19% Armor Penetration
//*/
//export function multiplicative(initial: number, value: number) {
//    return 1 - (1 - initial) * (1 - value);
//}
// otherwise, 110% * 110% = 121% (21% bonus instead of 19%)

// attack speed: base + ratio * bonus

// movement speed cap:
//  function msCap(msRaw: number): number {
//  if (msRaw > 490) {
//    return msRaw* 0.5 + 230;
//  }
//  if (msRaw > 415) {
//    return msRaw* 0.8 + 83;
//  }
//if (msRaw < 220)
//{
//    return msRaw * 0.5 + 110;
//}
//return msRaw;

// movement speed total:
//number {
//const _flat = flat * (1 + bonusMultiplier);
//const _percent = percent * (1 + bonusMultiplier);
//const _percentMultiplicative =
//(percentMultiplicative - 1) * (1 + bonusMultiplier) + 1;
//return (base + _flat) * (1 + _percent) * _percentMultiplicative;

// damage resist
//export function dmgx(resist: number)
//{
//    if (resist > 0)
//    {
//        return 100 / (100 + resist);
//    }
//    return 2 - 100 / (100 - resist);

// lethality:
// * const lethality = 100;
// * const level = 13;
// * const armorPenFlat = lethality * lethalityx(level); // ~88.9
//export function lethalityx(lvl: number) : number {
//  return 0.6 + 0.4 * (lvl / 18);

// post-reduction resistance:
//export function postReductionResist(
// resist: number,
// flatReduction: number,
//  percentReduction: number,
//  percentPenetration: number,
//  flatPenetration: number,
//)
//{
//    // Don't modify original.
//    let newResist = resist;
//    newResist -= flatReduction;
//    // Only flat resist reduction will apply below 0.
//    if (newResist > 0)
//    {
//        newResist *= 1 - percentReduction;
//        newResist *= 1 - percentPenetration;
//        newResist -= flatPenetration;
//        // We cannot penetrate below 0.
//        if (newResist< 0)
//        {
//            newResist = 0;
//        }
//}
//return newResist;
//}

//* Magic penetration and magic resist reduction work exactly like armor
//* penetration and armor reduction.penetration and reduction are considered on
//* the target champion in the following order:
//*
//* 1. Reduction, flat.
//* 2. Reduction, percentage.
//* 3. Penetration, percentage.
//* 4. Penetration, flat.
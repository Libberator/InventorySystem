using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace StatSystem
{
    public class StatSheet : SerializedMonoBehaviour
    {
        [SerializeField] private Dictionary<string, Stat> _stats = new();

        // needs to know about equipments, buffs, debuffs, etc.

        // potential angles:
        // strength, agility, intelligence/wisdom, charisma, vitality
        // constitution, poise (knockback/stun/slow resist)

        // elemental, true damage

        // attack damage, magic damage, armor, magic resistance,
        // armor pen, magic pen, critical strike change/damage, attack speed,
        // movement speed, health regen, mana regen, lifesteal, spellvamp

        // health (not sure if it counts as a "stat"), mana
    }
}
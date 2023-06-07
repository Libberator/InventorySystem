using TooltipSystem;
using UnityEngine;
using Utilities.Cooldown;

namespace InventorySystem
{
    // some enum for what type of consumable - permanent, temporary, conditional

    [CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory System/Consumable")]
    public class Consumable : Item, IHaveCooldown
    {
        [SerializeField] private float _cooldown = 0.5f;
        public StatModifier[] StatModifiers = new StatModifier[0];

        public float Cooldown => _cooldown;
        public virtual void Use() // IStatHolder, IHaveStats, ICharacter, I..
        {
            Debug.Log($"Used {Name}");
        }

        public override Tooltip GetTooltip()
        {
            _sb.Clear();
            _sb.Append($"{ColoredName}");
            if (StatModifiers.Length != 0)
            {
                _sb.AppendLine();
                _sb.AppendLine();
                for (int i = 0; i < StatModifiers.Length - 1; i++)
                    _sb.AppendLine(StatModifiers[i].ToString());
                _sb.Append(StatModifiers[^1].ToString());
            }
            if (!string.IsNullOrEmpty(Description))
            {
                _sb.AppendLine();
                _sb.AppendLine();
                _sb.Append($"<i>{Description}</i>");
            }

            return _sb.ToString();
        }
    }
}
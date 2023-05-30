using UnityEngine;
using StatSystem;
using Sirenix.OdinInspector;
using System.Text;

namespace InventorySystem
{
    public enum EquipmentType
    {
        PrimaryWeapon,
        SecondaryWeapon,
        TwoHandedWeapon,
        Helmet,
        Chest,
        Gloves,
        Pants,
        Boots,
        Accessory,
    }

    [CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory System/Equipment")]
    public class Equipment : Item
    {
        public EquipmentType EquipmentType;

        public StatModifier[] StatModifiers;

        private readonly StringBuilder _sb = new();

        public override string GetTooltipText()
        {
            _sb.Clear();
            _sb.Append($"{ColoredName} - {EquipmentType.EnumToString()}");
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

        public virtual void Equip()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Unequip()
        {
            throw new System.NotImplementedException();
        }

        private void OnValidate()
        {
            foreach (var mod in StatModifiers)
                mod.Source = this;
        }
    }
 
    public static class EquipmentTypeExtension
    {
        public static string EnumToString(this EquipmentType type)
        {
            return type switch
            {
                EquipmentType.PrimaryWeapon => "Primary Weapon",
                EquipmentType.SecondaryWeapon => "Secondary Weapon",
                EquipmentType.TwoHandedWeapon => "Two-Handed Weapon",
                _ => type.ToString(),
            };
        }
    }
}
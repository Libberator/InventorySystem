using Sirenix.OdinInspector;
using System.Text;
using TooltipSystem;
using UnityEngine;
using Utilities;

namespace InventorySystem
{
    public abstract class Item : ScriptableObject, IHaveTooltip
    {
        protected readonly StringBuilder _sb = new();
        
        [PreviewField(Alignment = ObjectFieldAlignment.Center, Height = 120f)]
        public Sprite Icon;

        [DetailedInfoBox("Preview Tooltip", "@GetTooltip().Text", infoMessageType: InfoMessageType.None)]
        [Tooltip("If the Display Name is different than the Asset Name")]
        [SerializeField] private string _nameOverride;

        [Multiline]
        public string Description;

        public Rarity Rarity;

        [Min(1)]
        public int MaxStack = 1;

        public virtual string Name => string.IsNullOrEmpty(_nameOverride) ? name : _nameOverride;
        public virtual string ColoredName => Name.WithColor(Rarity.TextColor);
        public virtual bool IsStackable => MaxStack > 1;
        public virtual Tooltip GetTooltip()
        {
            _sb.Clear();
            _sb.AppendLine(ColoredName);
            _sb.AppendLine();
            _sb.Append($"<i>{Description}</i>");

            return _sb.ToString();
        }
    }
}
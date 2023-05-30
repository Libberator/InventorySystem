using UnityEngine;
using Sirenix.OdinInspector;
using TooltipSystem;

namespace InventorySystem
{
    public abstract class Item : ScriptableObject, IHaveTooltip
    {
        [PreviewField(Alignment = ObjectFieldAlignment.Center, Height = 120f)] 
        public Sprite Icon;

        [DetailedInfoBox("Preview Tooltip", "$GetTooltipText", infoMessageType: InfoMessageType.None)]
        [Tooltip("If the Display Name is different than the Asset Name")]
        [SerializeField] private string _nameOverride;
        
        [Multiline]
        public string Description;
        
        public Rarity Rarity;
        
        [Min(1)] 
        public int MaxStack = 1;

        public virtual string Name => string.IsNullOrEmpty(_nameOverride) ? name : _nameOverride;
        public virtual string ColoredName => Rarity.ApplyColor(Name);
        public virtual string GetTooltipText() => $"{ColoredName}\n\n<i>{Description}</i>";
        public virtual bool IsStackable => MaxStack > 1;
    }
}
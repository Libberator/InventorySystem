using Sirenix.OdinInspector;
using TooltipSystem;
using UnityEngine;
using Utilities.Cooldown;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Ability System/Ability")]
    public class Ability : ScriptableObject, IHaveCooldown, IHaveTooltip
    {
        [PreviewField(Alignment = ObjectFieldAlignment.Center, Height = 120f)]
        public Sprite Icon;

        [DetailedInfoBox("Preview Tooltip", "@GetTooltip().Text", infoMessageType: InfoMessageType.None)]
        [Tooltip("If the Display Name is different than the Asset Name")]
        [SerializeField] private string _nameOverride;

        [Multiline]
        public string Description;

        [field: SerializeField] public float Cooldown { get; private set; }

        [Min(0)] public int Cost;

        public virtual string Name => string.IsNullOrEmpty(_nameOverride) ? name : _nameOverride;
        public virtual Tooltip GetTooltip() => $"{Name} ({Cost} Mana)\n\n{Description}";
    }
}

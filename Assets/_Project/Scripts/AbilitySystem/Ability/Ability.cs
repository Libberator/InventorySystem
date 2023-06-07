using Sirenix.OdinInspector;
using System.Text;
using TooltipSystem;
using UnityEngine;
using Utilities.Cooldown;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Ability System/Ability")]
    public class Ability : ScriptableObject, IHaveCooldown, IHaveTooltip
    {
        protected readonly StringBuilder _sb = new();
        
        [PreviewField(Alignment = ObjectFieldAlignment.Center, Height = 120f)]
        public Sprite Icon;

        [DetailedInfoBox("Preview Tooltip", "@GetTooltip().Text", infoMessageType: InfoMessageType.None)]
        [Tooltip("If the Display Name is different than the Asset Name")]
        [SerializeField] private string _nameOverride;

        [Multiline] public string Description;

        [Min(0)] public int Cost;
        
        [SerializeField] private float _cooldown;
        
        [SerializeField, Min(0)] private int _level = 1;

        public Ability NextLevelAbility;
        
        public virtual string Name => string.IsNullOrEmpty(_nameOverride) ? name : _nameOverride;
        public int Level => _level;
        public float Cooldown => _cooldown;
        public bool CanBeUpgraded => NextLevelAbility != null;
        public virtual Tooltip GetTooltip()
        {
            _sb.Clear();
            
            _sb.AppendLine($"{Name} (Lvl. {Level})");
            _sb.Append($"<color=lightblue>{Cost} Mana</color>");
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

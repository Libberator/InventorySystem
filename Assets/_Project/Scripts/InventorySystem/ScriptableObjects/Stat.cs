using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "New Stat", menuName = "Stat System/Stat")]
    public class Stat : ScriptableObject
    {
        [SerializeField] private string _nameOverride;
        [SerializeField] private string _abbreviation;
        [SerializeField, Multiline] private string _description;

        public string Name => string.IsNullOrEmpty(_nameOverride) ? name : _nameOverride;
        public string Abbreviation => _abbreviation;
        public string Description => _description;
    }
}
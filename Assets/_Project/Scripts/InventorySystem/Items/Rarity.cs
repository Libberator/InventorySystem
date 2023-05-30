using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "New Rarity", menuName = "Inventory System/Rarity")]
    public class Rarity : ScriptableObject
    {
        [SerializeField]
        private Color _textColor = Color.white;
        
        [SerializeField, Tooltip("Used for things like an ItemSlot Background")]
        private Color _primaryColor = Color.white;

        [SerializeField, Tooltip("Used as an accent, like for an ItemSlot Frame")]
        private Color _secondaryColor = Color.white;

        public Color TextColor => _textColor;
        public Color PrimaryColor => _primaryColor;
        public Color SecondaryColor => _secondaryColor;

        public string ApplyColor(string text)
        {
            var hexColor = ColorUtility.ToHtmlStringRGB(TextColor);
            return $"<color=#{hexColor}>{text}</color>";
        }
    }
}
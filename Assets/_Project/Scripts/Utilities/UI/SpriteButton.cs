using UnityEngine;
using UnityEngine.Events;

// okay *technically* this one isn't for "UI"
namespace Utilities.UI
{
    /// <summary>
    /// Recommend PolygonCollider2D for world-based Sprites.
    /// If you use PolygonCollider2D and change the sprite afterwards,
    /// a Reset will fix the collider shape
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class SpriteButton : MonoBehaviour
    {
        public UnityEvent OnClick;

        private void OnMouseUp() => OnClick.Invoke();
    }
}
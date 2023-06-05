using UnityEngine;
using UnityEngine.Events;

// okay *technically* this one isn't for "UI"
namespace Utilities.UI
{
    // Recommend PolygonCollider2D for world-based Sprites
    [RequireComponent(typeof(Collider2D))]
    public class SpriteButton : MonoBehaviour
    {
        public UnityEvent OnClick;

        private void OnMouseUp() => OnClick.Invoke();
    }
}
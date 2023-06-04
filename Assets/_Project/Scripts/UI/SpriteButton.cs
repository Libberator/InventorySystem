using UnityEngine;
using UnityEngine.Events;

// Recommend PolygonCollider2D for world-based Sprites
[RequireComponent(typeof(Collider2D))]
public class SpriteButton : MonoBehaviour
{
    public UnityEvent OnClick;

    private void OnMouseUp() => OnClick.Invoke();
}

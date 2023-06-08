using InventorySystem;
using UnityEngine;

namespace SystemsDemo
{
    public class ParticleEventListener : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _garbageParticle;

        private void OnEnable()
        {
            ItemEntryDragger.ItemDisposed += OnItemDisposed;
        }

        private void OnDisable()
        {
            ItemEntryDragger.ItemDisposed -= OnItemDisposed;
        }

        private void OnItemDisposed(object sender, ItemEntry entry)
        {
            var screenPos = (sender as Transform).position;
            var pos = Camera.main.ScreenToWorldPoint(screenPos);
            _garbageParticle.transform.position = new(pos.x, pos.y, 0f);
            _garbageParticle.Play();
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    public class AbilitySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] private Ability _ability;
        [SerializeField] private KeyCode _key;

        [Header("UI References")]
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _keyText;

        private void Update()
        {
            if (Input.GetKeyDown(_key))
                ActivateAbility();
        }

        public void ActivateAbility()
        {
            Debug.Log($"Ability activated: {_ability.Name}");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Shift-Click to Level-Up the ability if available
            ActivateAbility();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // if we're still holding an ability in the hand, return back to start
            throw new System.NotImplementedException();
        }

        public void OnDrop(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}

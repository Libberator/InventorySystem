using System.Collections.Generic;
using UnityEngine;

namespace Utilities.UI
{
    /// <summary>
    /// Acts like a Vertical Layout Group without the "snappiness" that those have.
    /// </summary>
    public class NotificationQueue : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _maxDisplayed;
        [SerializeField] private bool _spawnsFromTop;

        [Header("References")]
        [SerializeField] private GameObject _prefab;

        private Queue<GameObject> _queue;


    }
}

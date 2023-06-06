using UnityEngine;

namespace Utilities.Cooldown
{
    public class CooldownUpdater : MonoBehaviour
    {
        #region Singleton

        private static CooldownUpdater _instance;
        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        #endregion

        private void LateUpdate() => Cooldown.UpdateCooldowns(Time.deltaTime);
    }
}

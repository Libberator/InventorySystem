using UnityEngine;

namespace Utilities.Cooldown
{
    public static class CooldownUpdaterBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void SpawnCooldownUpdater()
        {
            var obj = new GameObject("[Cooldown Updater]");
            obj.AddComponent<CooldownUpdater>();
        }
    }
}
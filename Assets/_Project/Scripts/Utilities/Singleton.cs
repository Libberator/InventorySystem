using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    private static readonly object _lock = new();
    private static bool _applicationIsQuitting = false;

    protected virtual void Awake() => Instance = this as T;

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' won't be returned because application is quitting.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError($"[Singleton] This shouldn't happen. More than 1 '{typeof(T)}' found.");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        Debug.Log($"[Singleton] Lazily instantiating an instance of '{typeof(T)}'.");
                        _instance = new GameObject($"[Singleton] {typeof(T)}").AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
        private set
        {
            lock (_lock)
            {
                if (_instance != null && _instance != value)
                {
                    Debug.Log($"[Singleton] An instance of '{typeof(T)}' exists already. Destoying self.");
                    Destroy(value.gameObject);
                    return;
                }
                _instance = value;
            }
        }
    }

    // Object exists independent of Scene lifecycle, assume that means it has DontDestroyOnLoad set
    private static bool IsDontDestroyOnLoad => (_instance.gameObject.hideFlags & HideFlags.DontSave) == HideFlags.DontSave;

    // Could potentially use OnApplicationQuit() instead

    public void OnDestroy()
    {
        if (_instance == this as T)
        {
            if (IsDontDestroyOnLoad)
            {
                _applicationIsQuitting = true;
            }
        }
    }
}

public class PersistentSingleton<T> : Singleton<T> where T : Singleton<T>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
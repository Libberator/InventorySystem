using System;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class ComponentUtils
    {
        public static Coroutine DelayThenDo(this MonoBehaviour source, float delay, Action callback) => 
            source.StartCoroutine(DelayRoutine(delay, callback));

        private static IEnumerator DelayRoutine(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        /// <summary>Searches Self, then Parents, then Children for the component.</summary>
        public static Component GetComponentInHierarchy(this Component source, Type t, bool includeInactive = false)
        {
            if (source.TryGetComponent(t, out var component))
                return component;

            component = source.GetComponentInParent(t, includeInactive);
            if (component != null) return component;

            return source.GetComponentInChildren(t, includeInactive);
        }

        /// <summary>Searches Self, then Parents, then Children for the component.</summary>
        public static T GetComponentInHierarchy<T>(this Component source, bool includeInactive = false)
        {
            return (T)(object)GetComponentInHierarchy(source, typeof(T), includeInactive);
        }

        /// <summary>Searches Self, then Parents, then Children for the component.</summary>
        public static bool TryGetComponentInHierarchy<T>(this Component source, out T component, bool includeInactive = false)
        {
            component = source.GetComponentInHierarchy<T>(includeInactive);
            return component != null;
        }

        /// <summary>Searches Self, then Parents, then Children for the component.</summary>
        public static Component GetComponentInHierarchy(this GameObject gameObject, Type t, bool includeInactive = false)
        {
            if (gameObject.TryGetComponent(t, out var component))
                return component;

            component = gameObject.GetComponentInParent(t, includeInactive);
            if (component != null) return component;

            return gameObject.GetComponentInChildren(t, includeInactive);
        }

        /// <summary>Searches Self, then Parents, then Children for the component.</summary>
        public static T GetComponentInHierarchy<T>(this GameObject source, bool includeInactive = false)
        {
            return (T)(object)GetComponentInHierarchy(source, typeof(T), includeInactive);
        }

        /// <summary>Searches Self, then Parents, then Children for the component.</summary>
        public static bool TryGetComponentInHierarchy<T>(this GameObject source, out T component, bool includeInactive = false)
        {
            component = source.GetComponentInHierarchy<T>(includeInactive);
            return component != null;
        }
    }
}

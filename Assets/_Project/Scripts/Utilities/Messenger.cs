using System;
using System.Collections.Generic;

// Slightly modified from source: https://github.com/Wokarol/Messenger
namespace Utilities.MessageSystem
{
    public static class Messenger
    {
        private static readonly Dictionary<Type, Delegate> _topics = new();

        #region Public Methods

        /// <summary>Adds listener for event T</summary>
        public static void AddListener<T>(Action<T> handler)
        {
            var type = typeof(T);
            AddListener(handler, type);
        }

        /// <summary>Sends message of type T to all listeners</summary>
        public static void SendMessage<T>(T message)
        {
            var type = typeof(T);
            SendMessage(message, type);
        }

        /// <summary>Removes given handler from event</summary>
        public static void RemoveListener<T>(Action<T> handler)
        {
            Type type = typeof(T);
            RemoveListener(handler, type);
        }

        /// <summary>Removes all listeners from all events for a given target</summary>
        public static void RemoveAllListenersFor(object target)
        {
            // Stores all keys to iterate over (iterating over them directly causes error becase d[k] = x is treated as changing a collection)
            Type[] keys = new Type[_topics.Count];
            RemoveAllListenersFor(target, keys);
        }

        #endregion

        #region Private Methods

        private static void AddListener<T>(Action<T> handler, Type type)
        {
            // Adds delegate to dictionary if there's no of given type
            if (!_topics.ContainsKey(type))
            {
                _topics.Add(type, null);
            }

            // Adds new delegate to old one
            _topics[type] = Delegate.Combine(_topics[type], handler);
        }

        private static void SendMessage<T>(T message, Type type)
        {
            if (!_topics.ContainsKey(type)) return;

            // Checks if type is class without it, null check generates 17 B of Garbage for each call because of boxing
            if (type.IsClass && message == null) throw new ArgumentNullException();

            // Casts and invokes delegate assuming it's an Action<T> (should always be)
            ((Action<T>)_topics[type]).Invoke(message);
        }

        private static void RemoveListener<T>(Action<T> handler, Type type)
        {
            if (!_topics.ContainsKey(type)) return;

            // Removes handler from old delegate
            var newDelegate = Delegate.Remove(_topics[type], handler);

            // Removes delegate from dictionary is it was nullified
            if (newDelegate != null)
            {
                _topics[type] = newDelegate;
            }
            else
            {
                _topics.Remove(type);
            }
        }

        private static void RemoveAllListenersFor(object target, Type[] keys)
        {
            _topics.Keys.CopyTo(keys, 0);

            // Stores all keys that should be removed after for loop iteration
            List<Type> keysToClear = new();

            // Iterates over every type of event
            for (int i = 0; i < keys.Length; i++)
            {
                Type key = keys[i];
                var invocations = _topics[key].GetInvocationList();
                var newDelegate = _topics[key];

                // Check if any delegate matches target and if so removes it
                for (int j = 0; j < invocations.Length; j++)
                {
                    if (invocations[j].Target == target) newDelegate = Delegate.Remove(newDelegate, invocations[j]);
                }

                // Add key to "toClear" list if delegate has no listeners after last operation           
                if (newDelegate != null)
                {
                    _topics[key] = newDelegate;
                }
                else
                {
                    keysToClear.Add(key);
                }
            }

            // Removes all now empty keys
            for (int i = 0; i < keysToClear.Count; i++)
            {
                _topics.Remove(keysToClear[i]);
            }
        }
        #endregion
    }
}
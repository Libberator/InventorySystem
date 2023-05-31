using System.Collections.Generic;

namespace Utilities
{
    public static class CollectionUtils
    {
        public static T GetRandom<T>(this IList<T> items) => items[UnityEngine.Random.Range(0, items.Count)];

        public static T GetRandom<T>(T[] array) => array[UnityEngine.Random.Range(0, array.Length)];

    }
}
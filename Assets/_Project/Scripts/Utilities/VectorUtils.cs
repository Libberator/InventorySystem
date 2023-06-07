using System;
using UnityEngine;

namespace Utilities
{
    public static class VectorUtils
    {
        public static Vector2 ToVector2(this TextAnchor anchor) => anchor switch
        {
            TextAnchor.UpperLeft => new(0, 1),
            TextAnchor.UpperCenter => new(0.5f, 1),
            TextAnchor.UpperRight => new(1, 1),
            TextAnchor.MiddleLeft => new(0, 0.5f),
            TextAnchor.MiddleCenter => new(0.5f, 0.5f),
            TextAnchor.MiddleRight => new(1, 0.5f),
            TextAnchor.LowerLeft => new(0, 0),
            TextAnchor.LowerCenter => new(0.5f, 0),
            TextAnchor.LowerRight => new(1, 0),
            _ => throw new IndexOutOfRangeException(),
        };
    }
}

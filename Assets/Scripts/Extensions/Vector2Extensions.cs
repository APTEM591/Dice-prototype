using System;
using UnityEngine;

namespace Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2Int Normalize(this Vector2Int vector)
        {
            var x = Math.Clamp(vector.x, -1, 1);
            var y = Math.Clamp(vector.y, -1, 1);
            return new Vector2Int(x, y);
        }
    }
}
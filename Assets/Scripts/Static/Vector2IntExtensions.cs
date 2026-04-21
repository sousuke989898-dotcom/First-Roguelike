using UnityEngine;


public static class Vector2IntExtensions
{
    public static Vector2Int NormalizedVector(this Vector2Int vector)
    {
        int x = (vector.x == 0) ? 0 : (vector.x > 0 ? 1 : -1);
        int y = (vector.y == 0) ? 0 : (vector.y > 0 ? 1 : -1);
        return new Vector2Int(x, y);
    }

}
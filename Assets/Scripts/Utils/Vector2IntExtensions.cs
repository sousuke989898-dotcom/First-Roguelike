using UnityEngine;


public static class Vector2IntExtensions
{
    public static Vector2Int NormalizedVector(this Vector2Int vector)
    {
        return new Vector2Int(
        vector.x == 0 ? 0 : (int)Mathf.Sign(vector.x),
        vector.y == 0 ? 0 : (int)Mathf.Sign(vector.y)
        );
    }

    public static Direction GetDirection(this Vector2Int vector)
    {
        Vector2Int normalized = vector.NormalizedVector();
        if (DirectionTool.VectorDirections.TryGetValue(normalized, out var dir))
        {
            return dir;
        }
        return Direction.None;
    }

    public static float GetDistance(this Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    public static int GetChebyshevDistance(this Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return Mathf.Max(dx, dy);
    }

}
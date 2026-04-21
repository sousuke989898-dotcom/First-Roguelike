using System.Collections.Generic;
using UnityEngine;


[System.Flags]
public enum Direction {
    None = 0,
    Upper = 1,
    Right = 2,
    Down = 4,
    Left = 8,
}

public static class DirectionExtensions
{
    public static Direction[] baseDirs = { Direction.Left, Direction.Right, Direction.Down, Direction.Upper };

    public static Direction GetOpposite(this Direction dir)
    {
        Direction result = Direction.None;
        if (dir.HasFlag(Direction.Upper)) result |= Direction.Down;
        if (dir.HasFlag(Direction.Down))  result |= Direction.Upper;
        if (dir.HasFlag(Direction.Left))  result |= Direction.Right;
        if (dir.HasFlag(Direction.Right)) result |= Direction.Left;
        return result;
    }

    public static readonly Direction[] AllDirs = {
        Direction.Upper, Direction.Upper | Direction.Right,
        Direction.Right, Direction.Down  | Direction.Right,
        Direction.Down,  Direction.Down  | Direction.Left,
        Direction.Left,  Direction.Upper | Direction.Left
    };

    public static readonly Dictionary<Direction, Vector2Int> DirectionVectors = new()
    {
        {Direction.Upper,                   Vector2Int.up},
        {Direction.Upper | Direction.Right, new Vector2Int( 1, 1)},
        {Direction.Right,                   Vector2Int.right},
        {Direction.Down | Direction.Right,  new Vector2Int( 1,-1)},
        {Direction.Down ,                   Vector2Int.down},
        {Direction.Down | Direction.Left,   new Vector2Int(-1,-1)},
        {Direction.Left ,                   Vector2Int.left},
        {Direction.Upper | Direction.Left,  new Vector2Int(-1, 1)},
        {Direction.None,                    Vector2Int.zero}
    };

    public static Vector2Int GetVector(this Direction dir) => DirectionVectors[dir.ValidatedDir()];

    public static Direction ValidatedDir(this Direction dir)
    {
        if (dir.HasFlag(Direction.Upper) && dir.HasFlag(Direction.Down)) dir &= ~(Direction.Upper | Direction.Down);
        if (dir.HasFlag(Direction.Right) && dir.HasFlag(Direction.Left)) dir &= ~(Direction.Right | Direction.Left);
        return dir;
    }
}

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

public static class DirectionTool
{
    public static readonly Direction[] baseDirs = { Direction.Left, Direction.Right, Direction.Down, Direction.Upper };

    public static readonly Direction[] AllDirs = {
        Direction.Upper, Direction.Right | Direction.Upper,
        Direction.Right, Direction.Right | Direction.Down,
        Direction.Down,  Direction.Left  | Direction.Down,
        Direction.Left,  Direction.Left  | Direction.Upper
    };

    public static readonly Dictionary<Direction, Vector2Int> DirectionVectors = new()
    {
        {Direction.Upper,                   Vector2Int.up},         //上
        {Direction.Right | Direction.Upper, new Vector2Int( 1, 1)}, //右上
        {Direction.Right,                   Vector2Int.right},      //右
        {Direction.Right | Direction.Down,  new Vector2Int( 1,-1)}, //右下
        {Direction.Down ,                   Vector2Int.down},       //下
        {Direction.Left | Direction.Down,   new Vector2Int(-1,-1)}, //左下
        {Direction.Left ,                   Vector2Int.left},       //左
        {Direction.Left | Direction.Upper,  new Vector2Int(-1, 1)}, //左上
        {Direction.None,                    Vector2Int.zero}        //なし
    };

    public static readonly Dictionary<Vector2Int, Direction> VectorDirections = new()
    {
        {Vector2Int.up,         Direction.Upper},                  //上
        {new Vector2Int( 1, 1), Direction.Right | Direction.Upper},//右上
        {Vector2Int.right,      Direction.Right},                  //右
        {new Vector2Int( 1,-1), Direction.Right | Direction.Down}, //右下
        {Vector2Int.down,       Direction.Down},                   //下
        {new Vector2Int(-1,-1), Direction.Left  | Direction.Down}, //左下
        {Vector2Int.left,       Direction.Left},                   //左
        {new Vector2Int(-1, 1), Direction.Left  | Direction.Upper},//左上
        {Vector2Int.zero,       Direction.None}                    //なし
    };
}

public static class DirectionExtensions
{
    public static Direction GetOpposite(this Direction dir)
    {
        Direction result = Direction.None;
        if (dir.HasFlag(Direction.Upper)) result |= Direction.Down;
        if (dir.HasFlag(Direction.Down))  result |= Direction.Upper;
        if (dir.HasFlag(Direction.Left))  result |= Direction.Right;
        if (dir.HasFlag(Direction.Right)) result |= Direction.Left;
        return result;
    }

    public static Vector2Int Vector(this Direction dir)
    {
        if (DirectionTool.DirectionVectors.TryGetValue(dir.ValidatedDir(), out var vec))
        {
            return vec;
        }
        return Vector2Int.zero;
    }

    public static Direction ValidatedDir(this Direction dir)
    {
        if (dir.HasFlag(Direction.Upper) && dir.HasFlag(Direction.Down))
            dir &= ~(Direction.Upper | Direction.Down);
        if (dir.HasFlag(Direction.Right) && dir.HasFlag(Direction.Left))
            dir &= ~(Direction.Right | Direction.Left);
        return dir;
    }
}

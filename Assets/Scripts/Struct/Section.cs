using UnityEngine;

public class Section
{
    public RectInt rect;

    public int X => rect.xMin;
    public int Y => rect.yMin;

    public int Width => rect.width;
    public int Height => rect.height;

    public int SectionArea => Width * Height;
    public Vector2Int SectionCenter => new(X + (Width/2), Y + (Height/2));

    public RectInt roomRect;

    public Direction activeDoors = Direction.None;

    public Vector2Int? upperDoor, rightDoor, downDoor, leftDoor;//特定方向のドアが存在しない可能性あり

    public Vector2Int? UpperRoadEnd => upperDoor.HasValue 
        ? new Vector2Int(upperDoor.Value.x, rect.yMax -1) 
        : null;

    public Vector2Int? RightRoadEnd => rightDoor.HasValue 
        ? new Vector2Int(rect.xMax -1, rightDoor.Value.y) 
        : null;

    public Vector2Int? DownRoadEnd => downDoor.HasValue 
        ? new Vector2Int(downDoor.Value.x, rect.yMin) 
        : null;


    public Vector2Int? LeftRoadEnd => leftDoor.HasValue 
        ? new Vector2Int(rect.xMin, leftDoor.Value.y) 
        : null;

    public MapData map;


    public Section(RectInt rectInt)
    {
        rect = rectInt;
    }

    public Section(int x, int y, int w, int h)
    {
        rect = new RectInt(x,y,w,h);
    }

    /// <summary>
    /// 向きに基づいたドアの場所を取得する
    /// </summary>
    /// <param name="s">対象Section</param>
    /// <param name="dir">向き</param>
    /// <returns></returns>
    public Vector2Int? GetDoor( Direction dir) => dir switch
    {
        Direction.Upper => upperDoor,
        Direction.Right => rightDoor,
        Direction.Down  => downDoor,
        Direction.Left  => leftDoor,

        _ => null,
    };

    /// <summary>
    /// 向きに基づいた道の端を取得する
    /// </summary>
    /// <param name="s">対象Section</param>
    /// <param name="dir">向き</param>
    /// <returns></returns>
    public Vector2Int? GetRoadEnd(Direction dir) =>dir switch
    {
        Direction.Upper => UpperRoadEnd,
        Direction.Right => RightRoadEnd,
        Direction.Down  => DownRoadEnd,
        Direction.Left  => LeftRoadEnd,
        _ => null,
    };

    public void ClearUnusedDoors()
    {
        if (!activeDoors.HasFlag(Direction.Upper)) upperDoor = null;
        if (!activeDoors.HasFlag(Direction.Right)) rightDoor = null;
        if (!activeDoors.HasFlag(Direction.Down )) downDoor  = null;
        if (!activeDoors.HasFlag(Direction.Left )) leftDoor  = null;
    }


}

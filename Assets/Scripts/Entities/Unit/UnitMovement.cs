using System.Collections.Generic;
using UnityEngine;

public class UnitMovement
{
    private Unit _owner;
    public List<Vector2Int> ActionReservation {get; private set;} = new();
    public Direction ActionDir {get; private set;}

    public bool PlanningAction => ActionReservation.Count > 0;

    public UnitMovement(Unit owner) => _owner = owner;



    /// <summary>
    /// 行動リストに予定を追加する
    /// </summary>
    /// <param name="AbsPos"></param>
    /// <returns></returns>
    public bool SetPath(Vector2Int AbsPos)
    {
        ClearPath();

        List<Vector2Int> poss = PathFinder.GetPath(_owner.Pos, AbsPos);

        if (poss == null || poss.Count == 0) return false;

        ActionReservation.AddRange(poss);
        return true;
    }

    public void AddPath(Direction dir)
    {
        Vector2Int vector = dir.Vector();
        Vector2Int lastPos = GetLastPos();

        Vector2Int targetPos = lastPos + vector;
        if (MapManager.Instance.MapData.IsFloor(targetPos))
        {
            ActionReservation.Add(targetPos);
        }
    }

    public Vector2Int GetNextPos()
    {
        Vector2Int nextPos = ActionReservation[0];
        ActionReservation.RemoveAt(0);
        return nextPos;
    }

    public void ClearPath() =>ActionReservation.Clear();

    private Vector2Int GetLastPos() =>PlanningAction ? ActionReservation[^1] : _owner.Pos;
}
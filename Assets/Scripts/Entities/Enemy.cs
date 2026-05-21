using UnityEngine;
using System.Collections.Generic;
using Game.UnitSystem.UnitCommand;

public enum EnemyMoveState{Idle, Chase, Escape, Sleep}
public class Enemy : Unit
{
    public EnemyMoveState EnemyState {get; protected set;}

    // public override void InitUnit(int hp, IntRange atkRange, Vector2Int pos, string name)
    // {
    //     base.InitUnit(hp,atkRange,pos,name);
    //     EnemyState = EnemyMoveState.Chase;//試験用
    // }

    public override void InitUnit(UnitData data, Vector2Int pos)
    {
        base.InitUnit(data, pos);
        EnemyState = EnemyMoveState.Chase;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EnemyManager.Instance.RemoveEnemy(this);
    }

    // public override UnitCommand DicideAction()
    // {
    //     Vector2Int diff = GetPlayerDiff();
    //     UnitMovement.AddPath(diff.GetDirection());
    //     return base.DicideAction();
    // }

    public override UnitCommand DicideAction()
    {
        {
            if (ActionState != UnitActionState.Idle) return null;

            Vector2Int playerPos = GameManager.Player.Pos;

            if (Vector2IntExtensions.GetChebyshevDistance(Pos,playerPos) == 1) //隣にいるなら
            {
                return new AttackCommand(this, GameManager.Player);
            }

            List<Vector2Int> path = PathFinder.GetPath(Pos, playerPos);

            if (path == null) Debug.Log("path is null");
            if (path != null && path.Count > 0)
            {
                Debug.Log(string.Join(", ", path));

                Vector2Int nextStep = path[0];
                return new MoveCommand(this, nextStep);
            }
            return null;
        }
    }


    // public override bool TakeTurn(HashSet<Unit> planningToMoveUnits, HashSet<Unit> planningToAttackUnits)
    // {
    //     Vector2Int diff = GetPlayerDiff();
    //     AddPath(diff.GetDirection());

    //     return base.TakeTurn(planningToMoveUnits, planningToAttackUnits);
    // }

    /// <summary>
    /// プレイヤーとの相対座標を取得する
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetPlayerDiff()
    {
        Player p = GameManager.Player;
        if (p == null) return Vector2Int.zero;
        return p.Pos - Pos;
    }
}


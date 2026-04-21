using UnityEngine;

public enum EnemyMoveState{Idle, Chase, Escape, Sleep}
public class Enemy : Unit
{
    public EnemyMoveState EnemyState {get; protected set;}

    public override void InitUnit(int hp, RandomRange atkRange, Vector2Int pos, string name)
    {
        base.InitUnit(hp,atkRange,pos,name);
        EnemyState = EnemyMoveState.Chase;//試験用
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EnemyManager.Instance.RemoveEnemy(this);
    }

    public void TakeAction()
    {
        if (ActionState == UnitActionState.Dead) return;
        Vector2Int diff = GetPlayerDiff();

        Interact(diff.NormalizedVector());

    }

    /// <summary>
    /// プレイヤーとの相対座標を取得する
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetPlayerDiff()
    {
        Player p = GameManager.Instance.CurrentPlayer;
        if (p == null) return Vector2Int.zero;
        return p.Pos - Pos;
    }
}


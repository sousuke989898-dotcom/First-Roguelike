using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Unitの行動状態を管理し、アニメーションに使用する
/// Idle=待機,Sleep=眠り,Move=移動,Attack=攻撃,Dead=死亡,Destoroy=破壊まで待機
/// </summary>
public enum UnitActionState {Idle, Sleep, Move, Attack, Dead, Destroy};


public class Unit : Entity, IHasStatus
{
    public string Name {get; protected set;}

    public Status Status {get; protected set;}

    public List<Item> Items {get; private set;} = new();

    /// <summary>アニメーション用</summary>
    public UnitActionState ActionState {get; protected set;}

    // AbsPos
    public List<Vector2Int> ActionReservation {get; private set;} = new();

    public Vector2Int ActionDir {get; private set;} //(±1,±1)

    public event Action<Unit> OnStartAction;
    public event Action<Unit> OnEndAction;
    public event Action<Unit> OnDead;

    public float AnimTimer {get; protected set;}
    public float MaxTime {get; protected set;}
    public const float AnimTime = 0.1f;

    private UnitAnim unitAnim;

    [SerializeField] protected SpriteRenderer spriteRenderer ;

    protected Slider hpSlider;

    public virtual void InitUnit(int hp, IntRange atk, Vector2Int pos, string name)
    {
        InitEntity(pos);
        Name = name;
        Status = new(hp, atk, new(0));
        ActionState = UnitActionState.Idle;
        unitAnim = new UnitAnim(transform, spriteRenderer);
        UnitManager.Instance.AddUnit(this); //用変更
    }



//-------基本動作-------

    protected Vector2Int nextMovePos;
    protected IHasStatus nextAttackTarget;


    public virtual bool DecideAction(HashSet<Unit> planningToMoveUnits, HashSet<Unit> planningToAttackUnits)
    {
        if (ActionState != UnitActionState.Idle) return false;
        if (ActionReservation.Count == 0) return false;

        var targetPos = ActionReservation[0];
        ActionReservation.RemoveAt(0);

        ActionDir = targetPos - Pos;
        HashSet<Entity> entities = MapManager.Instance.MapData.GetEntities(targetPos);
        IHasStatus target = entities.GetHasStatus();


        if (target != null && targetPos != Pos /*自分自身は除外*/) //攻撃
        {
            nextAttackTarget = target;
            planningToAttackUnits.Add(this);
        }
        else //移動
        {
            nextMovePos = targetPos;
            planningToMoveUnits.Add(this);
        }
        return true;
    }

    public virtual IEnumerator AttackCoroutine() //TurnMagerから呼び出す
    {
        if (nextAttackTarget == null) yield break; // 自傷可
        ActionState = UnitActionState.Attack;
        OnStartAction?.Invoke(this);


        nextAttackTarget.TakeDamage(Status); //todo 攻撃タイミングを攻撃アニメーションの半分が終わった時にする
        nextAttackTarget = null;

        yield return StartCoroutine(unitAnim.AttackAnimationCoroutine(OldPos, Pos + ActionDir));
        OnEndAction?.Invoke(this);
    }

    public virtual IEnumerator MoveCoroutine() //TurnMagerから呼び出す
    {
        if (SetPos(nextMovePos))
        {
            ActionState = UnitActionState.Move;
            OnStartAction?.Invoke(this);

            nextMovePos = Vector2Int.zero;
            yield return StartCoroutine(unitAnim.MoveAnimCoroutine(OldPos, Pos));
            OnEndAction?.Invoke(this);
        }
    }


    public int TakeDamage(Status attakerStatus)
    {
        int damage = Status.TakeDamage(attakerStatus);
        if (Status.IsDead) Death();
        return damage;
    }

    public void Death()
    {
        if (!Status.IsDead) return;
        if (ActionState == UnitActionState.Dead) return;
        ActionState = UnitActionState.Dead;
        OnDead?.Invoke(this);
        base.Dispose();
        StartCoroutine(unitAnim.DieAnimationCoroutine());
        Destroy(gameObject, 1.0f); //アニメーションが終わった後にオブジェクトを破壊
    }

//-------移動-------

    /// <summary>
    /// 行動リストに予定を追加する
    /// </summary>
    /// <param name="AbsPos"></param>
    /// <returns></returns>
    public bool GetPath(Vector2Int AbsPos) //絶対座標で動作する
    {
        ActionReservation.Clear();

        var poss = PathFinder.GetPath(Pos,AbsPos);
        if (poss.Count <= 0) return false;

        if (poss == null || poss.Count <= 0) return false;

        ActionReservation.AddRange(poss);
        return true;
    }

    public void AddPath(Direction dir)
    {
        Vector2Int vector = dir.Vector();
        Vector2Int lastPos;

        if (ActionReservation.Count == 0) lastPos = Pos;
        else lastPos = ActionReservation[^1]; //行動予約の最後

        Vector2Int targetPos = lastPos + vector;
        if (MapManager.Instance.MapData.IsFloor(targetPos))
        {
            ActionReservation.Add(targetPos);
        }
    }

    public void AddPath(Vector2Int AbsPos)
    {
        Vector2Int lastPos;

        if (ActionReservation.Count == 0) lastPos = Pos;
        else lastPos = ActionReservation[^1]; //行動予約の最後

        Vector2Int diff = AbsPos - lastPos;
        Vector2Int targetPos = lastPos + diff;
        if (MapManager.Instance.MapData.IsFloor(targetPos))
        {
            ActionReservation.Add(targetPos);
        }
    }

    public void ClearPath()
    {
        ActionReservation.Clear();
    }


//-------UI-------

    public void SetHPSlider(Slider slider)
    {
        hpSlider = slider;
        InitStatus();
    }


    public void InitStatus()
    {
        Status.OnHpChanged += (hp, maxhp) =>
        {
            hpSlider.maxValue = maxhp;
            hpSlider.value = hp;
        };
        Status.SetHP(Status.MaxHp);
    }

}
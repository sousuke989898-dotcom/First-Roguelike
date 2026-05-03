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


public class Unit : Entity, IHasStatus, IHasTakeTurn
{
    public string Name {get; protected set;}

    public Status Status {get; protected set;}

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

    [SerializeField] protected SpriteRenderer spriteRenderer ;

    protected Slider hpSlider;

    public virtual void InitUnit(int hp, IntRange atk, Vector2Int pos, string name)
    {
        InitEntity(pos);
        Name = name;
        Status = new(hp, atk, new(0));
        ActionState = UnitActionState.Idle;
        UnitManager.Instance.AddUnit(this); //用変更
    }

//-------アニメーション-------

    

    protected void EndAnim()
    {
        ActionState = UnitActionState.Idle;
        SetTransform(Pos);
        OnEndAction?.Invoke(this);
    }



//-------基本動作-------

    public virtual bool TakeTurn()
    {
        if (ActionState != UnitActionState.Idle) return false;
        if (ActionReservation.Count == 0) return false;

        var targetPos = ActionReservation[0];
        ActionReservation.RemoveAt(0);

        if (Interact(targetPos))
        {
            Status.OnTurnEnd();
            return true;
        }
        return false;
    }


    protected bool Interact(Vector2Int targetPos)
    {
        ActionDir = targetPos - Pos;
        HashSet<Entity> entities = MapManager.Instance.Data.GetEntities(targetPos);
        IHasStatus target = entities.GetHasStatus();

        if (target != null && targetPos != Pos /*自分自身*/)
        {
            return AttackAction(target);
        }
        else if (entities != null && entities.Count > 0)
        {
            //アイテムを使用など
        }
        else
        {
            return MoveAction(targetPos);
        }
        return false;
    }

    public bool AttackAction(IHasStatus target)
    {
        if (target == null) return false; // 自傷可
        StartCoroutine(AttackAnimationCoroutine());
        target.TakeDamage(Status);
        OnEndAction?.Invoke(this);
        // UnitManager.Instance.OnStartAttack(this);

        return true;
    }

    public bool MoveAction(Vector2Int targetPos)
    {
        if (SetPos(targetPos))
        {
            StartCoroutine(MoveAnimationCoroutine());
            OnEndAction?.Invoke(this);
            // StartAnimation(0.1f, UnitActionState.Move);
            // UnitManager.Instance.OnStartMove(this);
            return true;
        }
        return false;
    }

    private IEnumerator MoveAnimationCoroutine()
    {
        ActionState = UnitActionState.Move;
        float elapsed = 0f;
        float duration = AnimTime;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            SetTransform(Vector2.Lerp(OldPos, Pos, progress));
            yield return null;
        }
        EndAnim();
    }

    private IEnumerator AttackAnimationCoroutine()
    {
        ActionState = UnitActionState.Attack;
        float elapsed = 0f;
        float duration = AnimTime;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float weight = Mathf.Sin(progress * Mathf.PI) * 0.2f;
            SetTransform(Pos + ((Vector2)ActionDir * weight));
            yield return null;
        }

        EndAnim();
    }

    private IEnumerator DieAnimationCoroutine()
    {
        ActionState = UnitActionState.Dead;
        float elapsed = 0f;
        float duration = 1.0f;
        Color c = spriteRenderer.color;


        OnDead?.Invoke(this);
        MapManager.Instance?.Data.RemoveEntity(this);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            spriteRenderer.color = new Color(c.r, c.g, c.b, 1.0f - progress);
            yield return null;
        }

        ActionState = UnitActionState.Destroy;
        Destroy(gameObject);
    }


    public int TakeDamage(Status attakerStatus)
    {
        int damage = Status.TakeDamage(attakerStatus);
        if (Status.IsDead) Death();
        return damage;
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
        if (MapManager.Instance.Data.IsFloor(targetPos))
        {
            ActionReservation.Add(targetPos);
        }
    }

    public void ClearPath()
    {
        ActionReservation.Clear();
    }


    public void Death()
    {
        if (!Status.IsDead) return;
        if (ActionState == UnitActionState.Dead) return;
        StartCoroutine(DieAnimationCoroutine());
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
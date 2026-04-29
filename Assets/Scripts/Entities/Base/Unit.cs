using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Unitの行動状態を管理し、アニメーションに使用する
/// Idle=待機,Sleep=眠り,Move=移動,Attack=攻撃,Dead=死亡,Destoroy=破壊まで待機
/// </summary>
public enum UnitActionState {Idle, Sleep, Move, Attack, Dead, Destroy};


public class Unit : Entity
{
    public string Name {get; protected set;}

    public Status Status {get; protected set;}

    /// <summary>アニメーション用</summary>
    public UnitActionState ActionState {get; protected set;}

    // AbsPos
    public List<Vector2Int> ActionReservation {get; private set;} = new();

    public Vector2Int ActionDir {get; private set;} //(±1,±1)

    public float AnimTimer {get; protected set;}
    public float MaxTime {get; protected set;}

    [SerializeField] protected SpriteRenderer spriteRenderer ;

    protected Slider hpSlider;

    public virtual void InitUnit(int hp, IntRange atk, Vector2Int pos, string name)
    {
        InitEntity(pos);
        Name = name;
        Status = new(hp, atk, new(0));
        ActionState = UnitActionState.Idle;
        UnitManager.Instance.AddUnit(this);
    }


    void Update()
    {
        UpdateAnimation();
    }

    protected virtual void UpdateAnimation()
    {
        if (ActionState == UnitActionState.Idle || ActionState == UnitActionState.Destroy) return;
        AnimTimer -= Time.deltaTime;
        float progress = (MaxTime != 0) ? 1 - (AnimTimer / MaxTime) : 0; // (0.0 ~ 1.0)

        switch (ActionState)
        {
            case UnitActionState.Move:
                MoveAnimation(progress);
                break;
            case UnitActionState.Attack:
                AttackAnimation(progress);
                break;
            case UnitActionState.Dead:
                DieAnimation(progress);
                break;
        }
    }

    protected void StartAnimation(float time, UnitActionState state)
    {
        MaxTime = time;
        AnimTimer = time;
        ActionState = state;
    }

    protected void EndAnimation()
    {
        ActionState = UnitActionState.Idle;
        SetTransform(Pos);
        UnitManager.Instance.EndAnim(this);
    }

    protected void MoveAnimation(float progress)
    {
        if (AnimTimer <= 0f)
        {
            EndAnimation();
        }
        else
        {
            SetTransform(OldPos + (((Vector2)ActionDir) * progress ));
        }
    }

    protected void AttackAnimation(float progress)
    {
        if (AnimTimer <= 0f)
        {
            EndAnimation();
        }
        else
        {
            float weight = Mathf.Sin(progress * 180) * 0.2f;
            SetTransform(Pos + (((Vector2)ActionDir) * weight));
        }
    }

    protected void DieAnimation(float progress)
    {
        Color c = spriteRenderer.color;
        if (AnimTimer <= 0f)
        {
            ActionState = UnitActionState.Destroy;
            spriteRenderer.color = new Color(c.r, c.g, c.b, 0);
            Destroy(gameObject);
        }
        else
        {
            spriteRenderer.color = new Color(c.r, c.g, c.b, 1-progress);
        }
    }


    public virtual bool TakeTurn()
    {
        if (ActionState != UnitActionState.Idle) return false;

        if (ActionReservation == null || ActionReservation.Count == 0) return false;

        var targetPos = ActionReservation[0];
        ActionReservation.RemoveAt(0);

        return Interact(targetPos);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    protected bool Interact(Vector2Int targetPos)
    {
        ActionDir = targetPos - Pos;

        var action = MapManager.Instance.Data.InteractCell(targetPos);
        return action switch
        {
            InteractResult.Unit => AttackAction(targetPos),
            InteractResult.Move => MoveAction(targetPos),
            _ => false,
        };
    }

    public bool AttackAction(Vector2Int targetPos)
    {
        Unit target = MapManager.Instance.Data.GetUnit(targetPos);
        if (target == null) return false;

        StartAnimation(0.1f, UnitActionState.Attack);
        Attack(target);

        UnitManager.Instance.OnStartAttack(this);
        return true;
    }

    public bool MoveAction(Vector2Int targetPos)
    {
        if (SetPos(targetPos))
        {
            StartAnimation(0.1f, UnitActionState.Move);
            UnitManager.Instance.OnStartMove(this);

            return true;
        }
        return false;
    }

    /// <summary>
    /// 行動リストに予定を追加する
    /// </summary>
    /// <param name="targetAbsPos"></param>
    /// <returns></returns>
    public bool GetPath(Vector2Int targetAbsPos) //絶対座標で動作する
    {
        ActionReservation.Clear();

        var poss = PathFinder.GetPath(Pos,targetAbsPos);
        if (poss.Count <= 0) return false;

        if (poss == null || poss.Count <= 0) return false;

        ActionReservation.AddRange(poss);
        return true;
    }

    public void AddPath(Direction dir)
    {
        Vector2Int vector = dir.GetVector();
        Vector2Int lastPos;
        if (ActionReservation.Count == 0)
        {
            lastPos = Pos;
        }
        else
        {
            lastPos = ActionReservation[^1];
        }

        Vector2Int targetPos = lastPos + vector;
        if (MapManager.Instance.Data.IsFloor(targetPos))
        {
            ActionReservation.Add(targetPos);
        }
    }


    protected int Attack(Unit target) => target.TakeDamage(Status);

    public int TakeDamage(Status attakerStatus)
    {
        int damage = Status.TakeDamage(attakerStatus);
        UpdateSlider();
        if (Status.IsDead) Death();
        return damage;
    }


    public void Death()
    {
        if (!Status.IsDead) return;
        StartAnimation(1f, UnitActionState.Dead);

        UnitManager.Instance.RemoveUnit(this);
        MapManager.Instance.Data.RemoveEntity(this);
    }

    public void SetHPSlider(Slider slider)
    {
        hpSlider = slider;
        UpdateSlider();
    }

    public void UpdateSlider()
    {
        if (hpSlider == null) return;
        hpSlider.maxValue = Status.MaxHp;
        hpSlider.value = Status.HP;
    }
}
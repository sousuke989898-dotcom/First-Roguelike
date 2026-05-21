using System;
using System.Collections;
using System.Collections.Generic;
using Game.UnitSystem.UnitCommand;
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

    public event Action<Unit> OnDead;

    private UnitAnim unitAnim;
    public UnitMovement UnitMovement {get; protected set;}

    public bool IsPlaningAction => UnitMovement.ActionReservation.Count > 0; //todo 名前の



    [SerializeField] protected SpriteRenderer spriteRenderer ;

    protected Slider hpSlider;


    public virtual void InitUnit(UnitData data, Vector2Int pos)
    {
        InitEntity(data, pos);
        Name = data.name;
        Status = new(data.DefaultMaxHP, data.DefaultAtk, data.DefaultDef);
        ActionState = UnitActionState.Idle;
        unitAnim = new UnitAnim(transform, spriteRenderer);
        UnitMovement = new(this);
        UnitManager.Instance.AddUnit(this);
    }



//-------基本動作-------

    public virtual UnitCommand DicideAction()
    {
        if (ActionState != UnitActionState.Idle || !UnitMovement.PlanningAction) return null;
        
        Vector2Int targetPos = UnitMovement.GetNextPos();
        
        HashSet<Entity> entities = MapManager.Instance.MapData.GetEntities(targetPos);
        Unit target = entities.GetUnit();

        if (target != null)
        {
            return new AttackCommand(this, target);
        }
        else
        {
            return new MoveCommand(this, targetPos);
        }
    }


    public virtual IEnumerator AttackCoroutine(Unit target)
    {
        Debug.Log(target);

        if (target != null && !target.Status.IsDead)
        {

            ActionState = UnitActionState.Attack;
            target.TakeDamage(Status);
            yield return StartCoroutine(unitAnim.AttackAnimationCoroutine(Pos, target.Pos));
            ActionState = UnitActionState.Idle;
        }
        else
        {
            //todo ターン処理中に対象が死んだということなので移動処理に移行？
        }
    }

    public virtual IEnumerator MoveCoroutine(Vector2Int targetPos)
    {
        if (SetPos(targetPos))
        {
            ActionState = UnitActionState.Move;
            yield return StartCoroutine(unitAnim.MoveAnimCoroutine(OldPos, Pos));
            ActionState = UnitActionState.Idle;
            
        }
        else
        {
            //todo ターン開始時に空いていたところにUnitがいるということなので、敵味方判別をした後に攻撃するか別の位置に移動するかを決める
        }
    }

    //-----体力関係-----

    public int TakeDamage(Status attakerStatus)
    {
        int damage = Status.TakeDamage(attakerStatus);
        if (Status.IsDead) Death();
        return damage;
    }

    private void Death()
    {
        if (!Status.IsDead || ActionState == UnitActionState.Dead) return;
        ActionState = UnitActionState.Dead;
        OnDead?.Invoke(this);
        base.Dispose();
        StartCoroutine(unitAnim.DieAnimationCoroutine());
        Destroy(gameObject, 1.0f); //アニメーションが終わった後にオブジェクトを破壊
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
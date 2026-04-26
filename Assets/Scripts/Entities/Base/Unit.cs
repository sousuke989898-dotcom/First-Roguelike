using System.Collections.Generic;
using System.Linq;
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

    public int HP {get; protected set ;}
    public int MaxHP {get; protected set;}

    public RandomRange Defence {get; protected set;}

    /// <summary>無敵かどうか</summary>
    public bool IsInvincible{get; protected set;} //使うかどうかは未定

    public bool IsDead => HP <= 0;

    public RandomRange AttackDamage {get; private set;}


    /// <summary>アニメーション用</summary>
    public UnitActionState ActionState {get; protected set;}

    // AbsPos
    public List<Vector2Int> ActionReservation {get; private set;} = new();

    public Vector2Int ActionDir {get; private set;} //(±1,±1)

//  (±1,±1)の範囲に限定したVector2Intのstruct絶対座標のVector2Intのstruct 相対座標のVector2Intのstructをそれぞれ作るべき? そうした場合は大規模な改修になるけど((±1,±1)の範囲に限定したやつは相対座標で統一しても良い)


    public float AnimTimer {get; protected set;}
    public float TimeWhenTimerStarted {get; private set;}

    [SerializeField] protected SpriteRenderer spriteRenderer ;

    protected Slider hpSlider;

    /// <summary>
    /// ユニットの初期化
    /// </summary>
    /// <param name="hp">初期HP</param>
    /// <param name="atkRange">攻撃力の最小値と最大値</param>
    /// <param name="pos">初期位置</param>
    public virtual void InitUnit(int hp, RandomRange atkRange, Vector2Int pos, string name)
    {
        InitEntity(pos);
        Name = name;
        InitializeHP(hp);
        AttackDamage = atkRange;
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
        float progress = AnimTimer / TimeWhenTimerStarted; 

        Debug.Log(progress);
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
        TimeWhenTimerStarted = time;
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
            Vector2 vector2 = Pos - OldPos;
            Vector2 currentLerpPos = OldPos + (vector2 * (1 - progress));
            float height = Mathf.Sin(progress * Mathf.PI) * 0.1f;
            SetTransform(currentLerpPos + new Vector2(0, height));
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
            float weight = Mathf.Sin(progress * Mathf.PI) * 0.2f;
            SetTransform(Pos + (new Vector2(ActionDir.x, ActionDir.y) * weight));
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
            spriteRenderer.color = new Color(c.r, c.g, c.b, progress);
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
        Debug.Log(action.ToString() +". " +targetPos);
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

    /// <summary>
    /// 攻撃する
    /// </summary>
    /// <param name="target">標的</param>
    /// <returns>与えたダメージ量</returns>
    protected int Attack(Unit target)
    {
        return target.TakeDamage(AttackDamage);
    }

    /// <summary>
    /// 攻撃する
    /// </summary>
    /// <param name="pos">標的の座標</param>
    /// <returns>与えたダメージ量</returns>
    protected int Attack(Vector2Int pos)
    {
        Unit target = MapManager.Instance.Data.GetUnit(pos);
        Debug.Log(target);
        return Attack(target);
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="amount">量</param>
    /// <returns>与えたダメージ</returns>
    public int TakeDamage(int amount)
    {
        if (IsInvincible) return 0;
        int damage = DamageFormula.Damagecalculation(amount,Defence);
        if (damage <= 0) return 0;
        SetHP(HP - damage);
        return damage;
    }

    public int TakeDamage(RandomRange range)
    {
        int amount = range.GetRandomValue();
        return TakeDamage(amount);
    }

    /// <summary>
    /// 最大HPと現在HPを新しく設定する
    /// </summary>
    /// <param name="amount">値</param>
    public void InitializeHP(int amount)
    {
        if (amount <= 0) return;
        SetMaxHP(amount);
        SetHP(amount);
    }

    /// <summary>
    /// 実体力を設定する
    /// </summary>
    /// <param name="amount">MaxHP > amount > 0</param>
    protected void SetHP(int amount)
    {
        HP = amount;
        if (HP > MaxHP) HP = MaxHP;
        if (HP <= 0) Death();

        if (hpSlider != null) hpSlider.value = HP;
    }

    /// <summary>
    /// 体力を回復する
    /// </summary>
    /// <param name="amount">amout > 0</param>
    /// <returns>回復した量</returns>
    public int HealHP(int amount)
    {
        if (amount <= 0) return 0;
        SetHP(HP + amount);
        return amount;
    }

    /// <summary>
    /// 体力最大値を設定する 実体力は変更しない
    /// </summary>
    /// <param name="amount">変更後</param>
    protected void SetMaxHP(int amount)
    {
        if(amount <= 0) return;
        MaxHP = amount;
        if (hpSlider != null) hpSlider.maxValue = MaxHP;
    }

    /// <summary>
    /// 体力最大値を変更する 実体力は変更しない
    /// </summary>
    /// <param name="amount">量</param>
    public void ChangeMaxHP(int amount)
    {
        if(MaxHP + amount <= 0)
        {
            MaxHP = 1;
            return;
        }
        SetMaxHP(MaxHP + amount);
    }

    /// <summary>
    /// ディフェンスの範囲を設定する
    /// </summary>
    /// <param name="range"></param>
    public void SetDefence(RandomRange range)
    {
        Defence = range;
    }

    /// <summary>
    /// 死ぬ・破壊される
    /// </summary>
    public void Death()
    {
        if (HP > 0) return;
        HP = 0;
        StartAnimation(1f, UnitActionState.Dead);

        UnitManager.Instance.RemoveUnit(this);
        MapManager.Instance.Data.RemoveEntity(this);
    }

    public void SetHPSlider(Slider slider)
    {
        hpSlider = slider;
        hpSlider.maxValue = MaxHP;
        hpSlider.value = HP;
    }
}
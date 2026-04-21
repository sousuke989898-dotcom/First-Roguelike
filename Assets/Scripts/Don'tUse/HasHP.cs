
/// <summary>
/// 破壊可能なオブジェクト
/// </summary>
public class HasHP
{
    public int HP {get; private set ;}
    public int MaxHP {get; private set;}

    public RandomRange Defence {get; private set;}

    /// <summary>無敵かどうか</summary>
    public bool IsInvincible{get; private set;} //使うかどうかは未定

    public bool IsDead {get; private set;}


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
    public void SetHP(int amount)
    {
        HP = amount;
        if (HP > MaxHP) HP = MaxHP;
        if (HP <= 0) Death();
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
    public void SetMaxHP(int amount)
    {
        if(amount <= 0) return;
        MaxHP = amount;
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
        MaxHP += amount;
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
        IsDead = true;
    }

}

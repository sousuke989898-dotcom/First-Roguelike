
public class CanAttack
{
    /// <summary>攻撃力の範囲</summary>
    public RandomRange AttackDamage;

    public CanAttack(RandomRange attackDamage)
    {
        AttackDamage = attackDamage;
    }

    /// <summary>
    /// 攻撃する
    /// </summary>
    /// <param name="amount">量</param>
    /// <param name="target">HPを持った目標</param>
    /// <returns>与えたダメージ量</returns>
    public int Attack(Unit target)
    {
        return target.TakeDamage(AttackDamage);
    }
}

public static class DamageFormula
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="defence"></param>
    /// <returns></returns>
    public static int Damagecalculation(int damage, int defence) //削除予定
    {
        int result = damage - defence;
        if (result < 0) result = 0;
        return result;
    }

    public static int Damagecalculation(Status attaker, Status defender)
    {
        int damage = attaker.Atk.Total.Value - defender.Def.Total.Value;
        return damage;
    }
}

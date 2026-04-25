
public static class DamageFormula
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="defence"></param>
    /// <returns></returns>
    public static int Damagecalculation(int damage, int defence)
    {
        int result = damage - defence;
        if (result < 0) result = 0;
        return result;
    }
}

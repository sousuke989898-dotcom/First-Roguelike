using System;
using UnityEngine;

/// <summary>
/// 値域を持ち、ランダムな値を返す
/// </summary>
[Serializable]
public struct RandomRange
{
    
    [SerializeField] private int min;
    [SerializeField] private int max;
    public int Min => min;
    public int Max => max;

    public RandomRange(int min, int max)
    {
        this.min = Math.Min(min,max);
        this.max = Math.Max(min,max);
    }

    public RandomRange(int value)
    {
        min = value;
        max = value;
    }


    public static implicit operator int(RandomRange range)
    {
        return range.GetRandomValue();
    }

    /// <summary>
    /// a,bの最小値・最大値を加算する
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static RandomRange operator +(RandomRange a, RandomRange b)
    {
        return new RandomRange(a.Min + b.Min, a.Max + b.Max);
    }

    /// <summary>
    /// 値域のランダムな値を取得する
    /// </summary>
    /// <param name="min">最小値</param>
    /// <param name="max">最大値</param>
    public static int GetRandomValue(int min, int max)
    {
        return UnityEngine.Random.Range(min,max + 1);
    }

    /// <summary>
    /// ランダムな値を取得する
    /// </summary>
    public int GetRandomValue()
    {
        return GetRandomValue(Min,Max);
    }

}
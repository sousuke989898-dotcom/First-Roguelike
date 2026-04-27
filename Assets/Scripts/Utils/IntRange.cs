using System;

/// <summary>
/// 最小値と最大値による数値範囲を扱い、ランダム値の取得や範囲判定を行う
/// </summary>
[Serializable]
public struct IntRange
{
    public int min;
    public int max;

    public static IntRange None = new(0);

    /// <summary>
    /// 範囲を指定して初期化。min > max の場合は自動的に入れ替えて保持する。
    /// </summary>
    public IntRange(int min, int max)
    {
        this.min = Math.Min(min, max);
        this.max = Math.Max(min, max);
    }

    public IntRange(int value)
    {
        min = value;
        max = value;
    }

    // Unityで使用する場合
    public readonly int Value => UnityEngine.Random.Range(min, max +1);

    /* // 純粋なC#環境で使用する場合
    private static readonly System.Random _sysRand = new System.Random();
    public readonly int Value => _sysRand.Next(min, max + 1);
    */
    public readonly int Range => max - min;

    public readonly int Clamp(int value) => Math.Clamp(value, min, max);
    public readonly IntRange ClampRange(int minLimit, int maxLimit) =>
        new(Math.Clamp(min, minLimit, maxLimit), Math.Clamp(max, minLimit, maxLimit));

    public readonly bool Contains(int value) => value >= min && value <= max;

    public readonly int Lerp(float t) => (int)Math.Round(min + (double)Range * Math.Clamp(t, 0f, 1f));
    /// <summary> 指定した値が範囲内のどの位置（0.0 ~ 1.0）にあるかを返す </summary>
    public readonly float InverseLerp(int value) =>  (min != max) ? Math.Clamp((value - min) / Range, 0f, 1f) : 0f; 

    public readonly bool Equals(IntRange other) => other.min == min && other.max == max;
    public override readonly bool Equals(object obj) => obj is IntRange other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(min, max);
    public override readonly string ToString() => $"{min} ~ {max}";

    public static implicit operator int(IntRange r) => r.Value;

    public static IntRange Add(IntRange r1, IntRange r2) => new(r1.min + r2.min, r1.max + r2.max);
    public static IntRange Sub(IntRange r1, IntRange r2) => new(r1.min - r2.min, r1.max - r2.max);
    public static IntRange Mul(IntRange r, int m) => new(r.min * m, r.max * m);
    public static IntRange Div(IntRange r, int d) => new(r.min / d, r.max / d);

    public static IntRange operator +(IntRange r1, IntRange r2) => Add(r1, r2);
    public static IntRange operator -(IntRange r1, IntRange r2) => Sub(r1, r2);
    public static IntRange operator *(IntRange r, int m) => Mul(r, m);
    public static IntRange operator /(IntRange r, int d) => Div(r, d);

    public static bool operator ==(IntRange r1, IntRange r2) => r1.Equals(r2);
    public static bool operator !=(IntRange r1, IntRange r2) => !r1.Equals(r2);

}
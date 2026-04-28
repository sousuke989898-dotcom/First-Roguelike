using System.Collections.Generic;

public class Param
{
    public IntRange BaseValue {get; private set;}

    public Dictionary<int, IntRange> _flatModifiers = new();
    public Dictionary<int, float> _percentModifiers = new(); //0.1f = +10%

    private int _nextId = 0;

    public IntRange Total =>  GetFlatSum() * GetPercentSum();

    public Param(IntRange range)
    {
        BaseValue = range;
    }

    public int Addmodifier(IntRange range) => AddFlatModifier(range);
    public int Addmodifier(float f) => AddPercentModifier(f);

    public void RemoveModifier(int key)
    {
        _flatModifiers.Remove(key);
        _percentModifiers.Remove(key);
    }

    private int AddFlatModifier(IntRange range)
    {
        int id = _nextId++;
        _flatModifiers.Add(id, range);
        return id;
    }

    private int AddPercentModifier(float f)
    {
        int id = _nextId++;
        _percentModifiers.Add(id, f);
        return id;
    }


    public IntRange GetFlatSum() =>  BaseValue + GetFlatModifiersSum();

    public IntRange GetFlatModifiersSum()
    {
        IntRange result = IntRange.None;
        foreach(var r in _flatModifiers.Values) result += r;
        return result;
    }

    public float GetPercentSum()
    {
        float result = 1.0f;
        foreach(var p in _percentModifiers.Values) result += p;
        return System.Math.Max(0f, result);;
    }

    public override string ToString()
    {
        return $"({BaseValue} + {GetFlatModifiersSum()}) * {System.Math.Round(GetPercentSum() * 100)} %";
    }

    public static implicit operator int(Param p) => p.Total.Value;

    public void SetBase(IntRange range) => BaseValue = range;
    public void AddBase(IntRange range) => BaseValue += range;

}
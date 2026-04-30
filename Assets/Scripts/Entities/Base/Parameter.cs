using System.Collections.Generic;

public class Param
{
    public IntRange BaseValue {get; private set;}

    public Dictionary<int, IntRange> _flatModifiers = new();
    public Dictionary<int, float> _percentModifiers = new(); //0.1f = +10%

    public HashSet<Modifier> _Modifiers = new();

    public IntRange Total => GetSum();

    public Param(IntRange range)
    {
        BaseValue = range;
    }

    public Param(int value)
    {
        BaseValue = new(value);
    }

    public void Addmodifier(Modifier modifier)
    {
        _Modifiers.Add(modifier);
    }

    public void RemoveModifier(Modifier modifier)
    {
        _Modifiers.Remove(modifier);
    }

    public IntRange GetSum()
    {
        IntRange flatValue = BaseValue;
        float magnification = 1.0f;
        foreach (Modifier modifier in _Modifiers)
        {
            if (modifier is RangeModifier rangeModifier)
            {
                flatValue += rangeModifier.Value;
            }
            else if (modifier is FloatModifier floatModifier)
            {
                magnification += floatModifier.Value;
            }
        }
        return flatValue * magnification;
    }

    // public override string ToString()
    // {
        
    // }

    public static implicit operator int(Param p) => p.Total.Value;

    public void SetBase(IntRange range) => BaseValue = range;
    public void AddBase(IntRange range) => BaseValue += range;

    public void SetBase(int value) => BaseValue = new IntRange(value);
    public void AddBase(int value) => BaseValue += new IntRange(value);


}
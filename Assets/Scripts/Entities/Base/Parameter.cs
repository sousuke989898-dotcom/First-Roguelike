using System.Collections.Generic;

public class Param
{
    public IntRange BaseValue {get; private set;}

    public List<IModifier> _Mods = new();

    public IntRange Sum;

    public Param(IntRange range)
    {
        BaseValue = range;
        SetSum();
    }

    public Param(int value)
    {
        BaseValue = new(value);
        SetSum();
    }

    public void AddMod(IModifier modifier)
    {
        _Mods.Add(modifier);
        SetSum();
    }
    

    public void RemoveMod(IModifier modifier)
    {
        _Mods.Remove(modifier);
        SetSum();
    }

    public void SetSum()
    {
        IntRange flatValue = BaseValue;
        float magnification = 1.0f;
        foreach (IModifier modifier in _Mods)
        {
            modifier.Apply(ref flatValue, ref magnification);
        }
        Sum = flatValue * magnification;
    }


    // public override string ToString()
    // {
        
    // }

    public static implicit operator int(Param p) => p.Sum.Value;

    public void SetBase(IntRange range) => BaseValue = range;
    public void AddBase(IntRange range) => BaseValue += range;

    public void SetBase(int value) => BaseValue = new IntRange(value);
    public void AddBase(int value) => BaseValue += new IntRange(value);


}
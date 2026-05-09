public class RangeModifier : Modifier
{
    public IntRange Value;

    public RangeModifier(IntRange Value, ModifierType Type)
    {
        this.Value = Value;
        this.Type = Type;
    }

    public RangeModifier(int min, int max, ModifierType Type)
    {
        Value = new IntRange(min,max);
        this.Type = Type;
    }

    public RangeModifier(int Value, ModifierType Type)
    {
        this.Value = new IntRange(Value, Value);
        this.Type = Type;
    }

}
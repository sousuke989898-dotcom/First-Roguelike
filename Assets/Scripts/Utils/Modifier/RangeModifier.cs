public class RangeModifier : IModifier
{
    public IntRange Value;

    public RangeModifier(IntRange Value)
    {
        this.Value = Value;
    }
    
    public RangeModifier(int Value)
    {
        this.Value = new(Value);
    }

    public void Apply(ref IntRange flatValue, ref float magnification)
    {
        flatValue += Value;
    }
}
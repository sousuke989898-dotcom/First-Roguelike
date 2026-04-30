public class RangeModifier : Modifier
{
    public IntRange Value;

    public RangeModifier(int Duration, IntRange Value) : base(Duration)
    {
        this.Value = Value;
    }
}
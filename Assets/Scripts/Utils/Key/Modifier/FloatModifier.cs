public class FloatModifier : Modifier
{
    public float Value;

    public FloatModifier(int Duration, float Value) : base(Duration)
    {
        this.Value = Value;
    }
}
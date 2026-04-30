public class FloatModifier : IModifier
{
    public float Value;

    public FloatModifier(float Value)
    {
        this.Value = Value;
    }

    public void Apply(ref IntRange flatValue, ref float magnification)
    {
        magnification += Value;
    }
}
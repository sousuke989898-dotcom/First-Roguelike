using System.Collections.Generic;

public abstract class ModEffect : Effect
{
    public IntRange baseRange;
    public RangeModifier Mod {get; protected set;}

    public ModEffect(int duration) : base(duration) {}

    public override void AddStack(Status status, int stack) //継承時はオーバーライド必須
    {
        base.AddStack(status, stack);
        Mod.Value += baseRange * stack;
    }

    public abstract void  OnApply(Status status); //継承時はオーバーライド必須
    public abstract void OnRemove(Status status); //継承時はオーバーライド必須
}

public static class ModEffectTool
{
    public static Dictionary<EffectType, IntRange> modEffectsValue = new()
    {
        {EffectType.AddStr, new(1,1)}
    };

    public static IntRange GetModEffectsValue(EffectType type) =>
        modEffectsValue.ContainsKey(type) ? modEffectsValue[type] : IntRange.None;
}

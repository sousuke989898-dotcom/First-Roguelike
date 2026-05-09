namespace Game.Effect
{
    using System.Collections.Generic;

    public abstract class ModEffect : Effect
    {
        public IntRange baseRange;
        public RangeModifier Mod {get; protected set;}

        public ModEffect(Status target, int duration) : base(target, duration) {}

        public override void AddStack(int stack)
        {
            base.AddStack(stack);
            Mod.Value += baseRange * stack;
        }

        public void  OnApply(Status status) => status.AddModifier(Mod);
        public void OnRemove(Status status) => status.RemoveModifier(Mod);
    }

    public static class ModEffectTool
    {
        public static Dictionary<EffectType, IntRange> modEffectsValue = new()
        {
            {EffectType.AddStr, new(1,1)} //todo　マジックナンバーの解消
        };

        public static IntRange GetModEffectsValue(EffectType type) =>
            modEffectsValue.ContainsKey(type) ? modEffectsValue[type] : IntRange.None;
    }
}

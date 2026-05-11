namespace Game.Effect
{
    using System.Collections.Generic;

    public abstract class ModEffect : StackableEffect
    {
        public IntRange baseRange;
        public RangeModifier Mod {get; protected set;}

        public ModEffect(int duration, int stack, Status status) : base(duration, stack)
        {
            OnApply(status);
        }

        public ModEffect(int duration, int stack) : base(duration, stack) {} //OnApplyを別で呼び出す必要あり

        public override void AddStack(int duration, int stack)
        {
            base.AddStack(duration, stack);
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

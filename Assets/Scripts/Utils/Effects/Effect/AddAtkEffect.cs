namespace Game.Effect
{
    public class AddAtkEffect : ModEffect
    {
        public override EffectType EffectType => EffectType.AddStr;
        public override int maxDuration => EffectTool.effectsDefaultDuration[EffectType];

        public AddAtkEffect(int duration, int stack, Status status) : base(duration, stack, status)
        {
            baseRange = ModEffectTool.GetModEffectsValue(EffectType);
            Mod = new(baseRange, ModifierType.Atk);
        }
    }
}

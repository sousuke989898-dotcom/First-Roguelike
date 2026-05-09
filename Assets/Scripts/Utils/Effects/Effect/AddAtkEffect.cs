namespace Game.Effect
{
    public class AddAtkEffect : ModEffect
    {
        public override EffectType EffectType => EffectType.AddStr;

        public AddAtkEffect(Status target, int duration) : base(target, duration)
        {
            baseRange = ModEffectTool.GetModEffectsValue(EffectType);
            Mod = new(baseRange, ModifierType.Atk);
        }

        public override void AddStack(int stack)
        {
            base.AddStack(stack);
            Target.Atk.SetSum();
        }

    }
}

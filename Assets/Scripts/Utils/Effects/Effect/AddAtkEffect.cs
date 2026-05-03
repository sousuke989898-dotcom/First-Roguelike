namespace Game.Effect
{
    public class AddAtkEffect : ModEffect
    {
        public override EffectType EffectType => EffectType.AddStr;

        public AddAtkEffect(int duration) : base(duration)
        {
            baseRange = ModEffectTool.GetModEffectsValue(EffectType);
            Mod = new(baseRange);
        } //todo　マジックナンバーの解消

        public override void AddStack(Status status, int stack)
        {
            base.AddStack(status, stack);
            status.Atk.SetSum();
        }

        public override void OnApply(Status status) => status.Atk.AddMod(Mod);
        public override void OnRemove(Status status) => status.Atk.RemoveMod(Mod);

    }
}

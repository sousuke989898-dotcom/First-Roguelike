namespace Game.Effect
{
    public class HealEffect : StackableEffect
    {
        public override EffectType EffectType => EffectType.Heal;
        public override int maxDuration => EffectTool.effectsDefaultDuration[EffectType];

        public HealEffect(int duration, int stack) : base(duration, stack) {}

        public override bool Tick(Status status)
        {
            status.HealHP(0.01f * Stack); // (1%) * stack
            return base.Tick(status);
        } //todo　マジックナンバーの解消
    }
}
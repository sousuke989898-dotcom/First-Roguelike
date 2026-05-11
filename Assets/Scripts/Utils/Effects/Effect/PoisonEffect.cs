namespace Game.Effect
{
    public class PoisonEffect : StackableEffect
    {
        public override EffectType EffectType => EffectType.Poison;
        public override int maxDuration => EffectTool.effectsDefaultDuration[EffectType];

        public PoisonEffect(int duration, int stack) : base(duration, stack) {}

        public override bool Tick(Status status)
        {
            status.HurtHP(0.01f * Stack); // (1%) * stack
            return base.Tick(status);
        } //todo　マジックナンバーの解消
    }
}


namespace Game.Effect
{
    public class PoisonEffect : Effect
    {
        public override EffectType EffectType => EffectType.Poison;
        public PoisonEffect(Status target, int duration) : base(target, duration) {}

        public override bool Tick()
        {
            Target.HurtHP(0.01f * Stack); // (1%) * stack
            return base.Tick();
        } //todo　マジックナンバーの解消
    }
}


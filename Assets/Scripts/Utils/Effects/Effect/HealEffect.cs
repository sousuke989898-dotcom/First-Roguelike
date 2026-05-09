namespace Game.Effect
{
    public class HealEffect : Effect
    {
        public override EffectType EffectType => EffectType.Heal;
        public HealEffect(Status target, int duration) : base(target, duration) {}

        public override bool Tick()
        {
            Target.HealHP(0.01f * Stack); // (1%) * stack
            return base.Tick();
        } //todo　マジックナンバーの解消
    }
}
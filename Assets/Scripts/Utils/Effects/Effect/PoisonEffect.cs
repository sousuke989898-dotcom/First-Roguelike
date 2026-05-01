public class PoisonEffect : Effect
{
    public override EffectType EffectType => EffectType.Poison;
    public PoisonEffect(int duration) : base( duration) {}

    public override bool Tick(Status status)
    {
        status.HurtHP(0.01f * Stack); // (1%) * stack
        return base.Tick(status);
    } //todo　マジックナンバーの解消
}




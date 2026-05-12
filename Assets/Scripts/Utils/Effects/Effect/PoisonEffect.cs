namespace Game.Effect
{
    public class PoisonEffect : DefaultEffect
    {

        public PoisonEffect(EffectData effectData) : base(effectData) {}

        public override bool Tick(Status status)
        {
            status.HurtHP(Data.Power * Stack);
            return base.Tick(status);
        }
    }
}


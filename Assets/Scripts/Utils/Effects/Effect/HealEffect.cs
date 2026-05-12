namespace Game.Effect
{
    public class HealEffect : DefaultEffect
    {

        public HealEffect(EffectData data) : base(data) {}

        public override bool Tick(Status status)
        {
            status.HealHP(Data.Power * Stack);
            return base.Tick(status);
        }
    }
}
namespace Game.Effect
{
    public abstract class DefaultModEffect : DefaultEffect, IModEffect
    {
        public RangeModifier Modifier{ get; protected set;}

        public DefaultModEffect(EffectData data) : base(data)
        {
            Modifier = new RangeModifier(data.baseRange, data.modType);
        }

        public virtual void OnApply(Status status) => status.AddModifier(Modifier);
        public virtual void OnRemove(Status status) => status.RemoveModifier(Modifier);
    }

}

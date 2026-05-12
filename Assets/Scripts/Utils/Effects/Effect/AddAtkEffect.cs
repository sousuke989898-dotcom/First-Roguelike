namespace Game.Effect
{
    public class AddAtkEffect : DefaultModEffect
    {
        public AddAtkEffect(EffectData data) : base(data) {}

        public override void AddStack(int stack)
        {
            base.AddStack(stack);
            Modifier.Value += Data.baseRange * stack;
        }
    }
}

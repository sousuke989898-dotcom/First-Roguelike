namespace Game.Effect
{
    public abstract class StackableEffect : Effect
    {
        public virtual int Stack {get; protected set;}

        public StackableEffect(int duration, int stack) : base(duration)
        {
            Stack = stack;
        }

        public virtual void AddStack(int duration, int stack)
        {
            Stack += stack;
            Duration = EffectTool.effectsDefaultDuration[EffectType];
        }
    }
}
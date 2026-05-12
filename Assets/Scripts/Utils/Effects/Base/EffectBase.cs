namespace Game.Effect
{

    public abstract class Effect
    {
        public EffectData Data {get; protected set;}

        public Effect(EffectData data)
        {
            Data = data;
        }
    }

    public abstract class DefaultEffect : Effect, ITicableEffect, IStackableEffect
    {
        public int Duration {get; protected set;}
        public int Stack {get; protected set;}
        public int MaxStack {get; protected set;}

        public DefaultEffect(EffectData data) : base(data)
        {
            Duration = data.defaultDuration;
            Stack = data.defaultStack;
            MaxStack = data.defaultMaxStack;
        }

        public virtual bool Tick(Status status)
        {
            Duration--;
            return Duration <= 0;
        }

        public virtual void AddStack(int stack)
        {
            Stack = System.Math.Min(Stack + stack, MaxStack);
            Duration = Data.defaultDuration; //スタックが増えると効果時間がリセット
        }
    }

}
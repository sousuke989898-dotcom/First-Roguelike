namespace Game.Effect
{
    using System.Collections.Generic;

    public abstract class Effect
    {
        public abstract EffectType EffectType {get;}
        public int Duration {get; protected set;}
        public abstract int maxDuration{get;}

        public Effect(int duration) 
        {
            Duration = duration;
        }

        public virtual bool Tick(Status status)
        {
            Duration--;
            return Duration <= 0;
        }

    }

    public static class EffectTool
    {
        public static Dictionary<EffectType, int> effectsDefaultDuration = new()
        {
            {EffectType.Poison, 5},
            {EffectType.Heal, 5},
            {EffectType.AddStr, 20}
            
        };


    }
}
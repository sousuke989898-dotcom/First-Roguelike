namespace Game.Effect
{
    using System.Collections.Generic;

    public abstract class Effect
    {
        public abstract EffectType EffectType {get;}
        public int Duration {get; protected set;}
        public int Stack {get; protected set;}
        public Status Target {get; protected set;}

        public Effect(Status target, int duration)
        {
            Target = target;
            Duration = duration;
        }

        public virtual bool Tick()
        {
            Duration--;
            return Duration <= 0;
        }

        public virtual void AddStack(int count)
        {
            Stack += count;
        }
    }

    public static class EffectTool
    {
        public static Dictionary<EffectType, int> effectsDefaultDuration = new()
        {
            {EffectType.Poison, 3},
            {EffectType.Heal, 3},
            {EffectType.AddStr, 20}
            
        };


        public static Effect GetEffect(EffectType effectType, Status target, int duration)
        {
            return effectType switch
            {
                EffectType.Poison => new PoisonEffect(target, duration),
                EffectType.Heal => new HealEffect(target, duration),
                EffectType.AddStr => new AddAtkEffect(target, duration),
                _ => null,
            };
        }
    }
}
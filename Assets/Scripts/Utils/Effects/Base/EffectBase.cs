using System.Collections.Generic;


public abstract class Effect
{
    public abstract EffectType EffectType {get;}
    public int Duration {get; protected set;}
    public int Stack {get; protected set;}

    public Effect(int duration)
    {
        Duration = duration;
    }

    public virtual bool Tick(Status status)
    {
        Duration--;
        return Duration <= 0;
    }

    public virtual void AddStack(Status status, int count)
    {
        Stack += count;
    }
}

public static class EffectTool
{
    public static Dictionary<EffectType, int> effectsDefaultDuration = new()
    {
        {EffectType.Poison, 3},
        {EffectType.AddStr, 20}
    };


    public static Effect GetEffect(EffectType effectType, int duration)
    {
        return effectType switch
        {
            EffectType.Poison => new PoisonEffect(duration),
            EffectType.AddStr => new AddAtkEffect(duration),
            _ => null,
        };
    }
}
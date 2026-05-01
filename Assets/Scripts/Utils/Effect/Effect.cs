using System.Collections.Generic;

public enum EffectType{Poison,AddStr}

public abstract class Effect
{
    public abstract EffectType EffectType {get;}
    public int Duration {get; protected set;}

    public Effect(int duration)
    {
        Duration = duration;
    }

    public virtual bool Tick(Status status)
    {
        Duration--;
        return Duration <= 0;
    }

    public static Effect GetEffect(EffectType effectType) //todo　マジックナンバーの解消
    {
        return effectType switch
        {
            EffectType.Poison => new PoisonEffect(20),
            EffectType.AddStr => new AddSTrEffect(20, new RangeModifier(1)),
            _ => null,
        };
    }
}

public interface IModEffect
{
    void OnApply(Status status);
    void OnRemove(Status status);
}



public class PoisonEffect : Effect
{
    public override EffectType EffectType => EffectType.Poison;
    public PoisonEffect(int duration) : base( duration) { }

    public override bool Tick(Status status)
    {
        status.HurtHP(0.01f); // (1%)
        return base.Tick(status);
    }
}

public class AddSTrEffect : Effect, IModEffect
{
    public IModifier Mod {get; protected set;}
    public override EffectType EffectType => EffectType.AddStr;

    
    public AddSTrEffect(int duration, IModifier mod) : base(duration)
    {
        Mod = mod;
    }

    public void OnApply(Status status) => status.Atk.AddMod(Mod);
    public void OnRemove(Status status) => status.Atk.RemoveMod(Mod);

}
using System;

public enum EffectType{Poison}

public class Effect
{
    public EffectType effectType;
    public int count;

    public Action Action;

    public Effect(EffectType effectType, int count)
    {
        this.effectType = effectType;
        this.count = count;
    }

    public virtual bool Tick()
    {
        count --;
        return count == 0;
    }

}

// public class StatusEffect : Effect
// {
//     public Param param;
//     public IModifier mod;

//     public StatusEffect(, int count, Param param, IModifier mod) : base(name,count)
//     {
//         this.param = param;
//         this.mod = mod;
//     }

//     public void OnApply() => param.AddMod(mod);
//     public void OnRemove() => param.RemoveMod(mod);
// }

// public class ConditionEffect : Effect
// {
//     public ConditionEffect(string name, int count, Action action) : base(name, count)
//     {
//         Action += action;
//     }

//     public override bool Tick()
//     {
//         Action.Invoke();
//         return base.Tick();
//     }
// }
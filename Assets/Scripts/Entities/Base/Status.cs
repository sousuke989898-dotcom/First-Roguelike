using System;
using System.Collections.Generic;
using UnityEngine;

public enum ParamType{MaxHP, Atk, Def}

public class Status
{
    public int HP;
    public Param MaxHp;

    public Param Atk;
    public Param Def;

    public bool IsDead => HP == 0;

    public event Action<int,int> OnHpChanged;

    public List<Effect> effects;

    public Status(int hp, IntRange atk, IntRange def)
    {
        MaxHp = new Param(hp); 
        Atk = new Param(atk);
        Def = new Param(def);
        effects = new();
        InitHP(hp);
    }

    public void OnTurnEnd()
    {
        List<Effect> removeEffects = new();
        foreach (Effect effect in effects)
        {
            if (effect.Tick(this)) removeEffects.Add(effect);
        }

        foreach (Effect effect in removeEffects)
        {
            if (effect is IModEffect modEffect)
            {
                modEffect.OnRemove(this);
            }

            effects.Remove(effect);
        }
    }

    public int DealDamage(Status target) => target.TakeDamage(this);

    public int TakeDamage(Status attker)
    {
        int damage = DamageFormula.Damagecalculation(attker, this);
        damage = Math.Max(0, damage);
        HurtHP(damage);
        return damage;
    }

    public void SetHP(int amount)
    {
        HP = Math.Clamp(amount, 0, MaxHp);
        OnHpChanged?.Invoke(HP, MaxHp);
    }
    /// <param name="amount">(0.0 ~ 1.0)</param>
    public void SetHP(float amount) => SetHP(Mathf.Lerp(0, MaxHp, Mathf.Clamp(amount,0,1)));

    public void HealHP(int amount) => SetHP(HP + Math.Max(0, amount)); 
    /// <param name="amount">(0.0 ~ 1.0)</param>
    public void HealHP(float amount) => SetHP(HP + (int)Math.Ceiling(MaxHp * Math.Clamp(amount,0f,1f)));//切り上げ

    public void HurtHP(int amount) => SetHP(HP - Math.Max(0,amount));
    /// <param name="amount">(0.0 ~ 1.0)</param>
    public void HurtHP(float amount) => SetHP(HP - (int)Math.Ceiling(MaxHp * Math.Clamp(amount,0f,1f)));//切り上げ

    public void InitHP(int amount)
    {
        MaxHp.SetBase(amount);
        SetHP(amount);
    }

    public void ApplyEffect(EffectType effectType)
    {
        Effect effect = Effect.GetEffect(effectType);
        effects.Add(effect);
        if (effect is IModEffect modEffect)
        {
            modEffect.OnApply(this);
        }
    }



}
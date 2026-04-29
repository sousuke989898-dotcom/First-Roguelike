using System;
using UnityEngine;

public class Status
{
    public int HP;
    public Param MaxHp;

    public Param Atk;
    public Param Def;
    public bool IsDead => HP == 0;

    public Status(int hp, IntRange atk, IntRange def)
    {
        MaxHp = new Param(hp); 
        Atk = new Param(atk);
        Def = new Param(def);
        InitHP(hp);
    }


    public virtual int DealDamage(Status target) => target.TakeDamage(this);

    public virtual int TakeDamage(Status attker)
    {
        int damage = DamageFormula.Damagecalculation(attker, this);
        damage = Math.Max(0, damage);
        HurtHP(damage);
        return damage;
    }

    public void SetHP(int amount) => HP = Math.Clamp(amount, 0, MaxHp);
    /// <param name="amount">(0.0 ~ 1.0)</param>
    public void SetHP(float amount) => SetHP(Mathf.Lerp(0, MaxHp, Mathf.Clamp(amount,0,1)));

    public void HealHP(int amount) => SetHP(HP + Math.Max(0, amount)); 
    /// <param name="amount">(0.0 ~ 1.0)</param>
    public void HealHP(float amount) => SetHP(HP + (MaxHp * Math.Clamp(amount,0f,1f)));

    public void HurtHP(int amount) => SetHP(HP - Math.Max(0,amount));
    /// <param name="amount">(0.0 ~ 1.0)</param>
    public void HurtHP(float amount) => SetHP(HP - (MaxHp * Math.Clamp(amount,0f,1f)));

    public void InitHP(int amount)
    {
        MaxHp.SetBase(amount);
        SetHP(amount);
    }

}
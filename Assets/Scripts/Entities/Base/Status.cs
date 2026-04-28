using System;
using UnityEngine;

public class Status //todo 変更予定
{
    private int _hp;
    public int HP {get => _hp; private set => _hp = Math.Clamp(value, 0, _maxHP); }

    private int _maxHP;
    public int MaxHp {get => _maxHP; private set => _maxHP = Math.Max(1, value);}

    public Param Atk;
    public Param Def;
    public bool IsDead => HP == 0;

    public Status(int hp, IntRange atk, IntRange def)
    {
        InitHP(hp);
        Atk = new(atk);
        Def = new(def);
    }


    public virtual int DealDamage(Status target) => target.TakeDamage(Atk);

    public virtual int TakeDamage(Param atk) => TakeDamage(atk.Total.Value);

    public virtual int TakeDamage(int amount)
    {
        int damage = DamageFormula.Damagecalculation(amount,Def);
        HurtHP(damage);
        return damage;
    }

    public void SetHP(int amount) => HP = amount;

    public void SetHP(float value)
    {
        SetHP(Mathf.Lerp(0,MaxHp,Mathf.Clamp(value,0,1)));
    }


    public void HealHP(int amount)
    {
        if (amount <= 0) return;
        SetHP(HP + amount);
    }

    public void HurtHP(int amount)
    {
        if (amount <= 0) return;
        SetHP(HP - amount);
    }


    public void InitHP(int amount)
    {
        SetMaxHP(amount);
        SetHP(amount);
    }


    public void SetMaxHP(int amount) => MaxHp = amount;
    public void AddMaxHp(int amount) => MaxHp += amount;

    public void SetAtk(IntRange r) => Atk = new(r);
    public void AddAtkBase(IntRange r) => Atk.AddBase(r);
    public int AddAtkModifier(IntRange r) => Atk.Addmodifier(r);

    public void SetDef(IntRange r) => Def = new(r);
    public void AddDef(IntRange r) => Def.AddBase(r);
    public int AddDefModifier(IntRange r) => Def.Addmodifier(r);

    public virtual bool Death() => IsDead;

}
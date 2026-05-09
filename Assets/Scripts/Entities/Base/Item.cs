using UnityEngine;
using Game.Effect;

public class Item : Entity
{
    public void InitItem(Vector2Int pos)
    {
        base.InitEntity(pos);
    }

    public virtual void PickedUp(Unit unit)
    {
        unit.Items.Add(this);
        base.OnDestroy();
        Destroy(gameObject);
    }

    public virtual void Dropped(Unit unit, Vector2Int pos)
    {
        if (unit.Items.Remove(this))
        {
            InitEntity(pos);
        }
    }
}

public class Equipment : Item, IEquipable
{
    public RangeModifier Modifier {get; protected set;}

    public Equipment(RangeModifier mod)
    {
        Modifier = mod;
    }

    public virtual void Equip(Unit unit)
    {
        unit.Status.AddModifier(Modifier);
    }

    public virtual void Unequip(Unit unit)
    {
        unit.Status.RemoveModifier(Modifier);
    }
}

public class HpPosion : Item, IUsable
{
    public void Use(Unit unit)
    {
        unit.Status.ApplyEffect(EffectType.Heal, 1);
    }
}


public interface IUsable
{
    public void Use(Unit unit);
}

public interface IEquipable
{
    public void Equip(Unit equipper);
    public void Unequip(Unit equipper);
}
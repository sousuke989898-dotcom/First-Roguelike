public abstract class Modifier
{
    public ModifierType Type{get; protected set;}
}


public enum ModifierType
{
    None,
    MaxHp,
    Atk,
    Def,
}
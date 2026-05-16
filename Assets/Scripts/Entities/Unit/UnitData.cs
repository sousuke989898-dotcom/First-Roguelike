using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/UnitData")]
public class UnitData : EntityData
{
    public int DefaultMaxHP;
    public IntRange DefaultAtk;
    public IntRange DefaultDef;
}


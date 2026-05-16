using UnityEngine;
using Game.Effect;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemData")]
public class ItemData : EntityData
{
    public int price;
    public EffectData effectData;
    public int stackAmount = 1;
}
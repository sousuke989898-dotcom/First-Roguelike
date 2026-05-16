using UnityEngine;

public class Player : Unit
{
    [Header("初期値")]
    [SerializeField] private int InitHP;
    [SerializeField] private IntRange atkRange;

    public override void InitUnit(UnitData data, Vector2Int pos)
    {
        base.InitUnit(data, pos);
    }


}

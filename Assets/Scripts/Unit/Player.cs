using UnityEngine;

public class Player : Unit
{
    [Header("初期値")]
    [SerializeField] private int InitHP;
    [SerializeField] private RandomRange atkRange;


    public void InitPlayer(Vector2Int pos)
    {
        InitUnit(InitHP,atkRange,pos,"Player");
    }

    // public override void Initialize(int hp, RandomRange atkRange, Vector2Int pos, string name)
    // {
    //     base.Initialize(hp,atkRange,pos,name);
    // }


}

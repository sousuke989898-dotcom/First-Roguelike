using System.Linq;
using UnityEngine;

public class Player : Unit
{
    [Header("初期値")]
    [SerializeField] private int InitHP;
    [SerializeField] private IntRange atkRange;

    public void InitPlayer(Vector2Int pos)
    {
        InitUnit(InitHP,atkRange,pos,"Player");
    }

}

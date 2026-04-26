using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitManager : MonoBehaviour
{

    // public Dictionary<Vector2Int,Unit> UnitsDic {get; private set;} = new();
    // public Unit[,] UnitsMap {get; private set;}

    public static UnitManager Instance {get; private set;}
    public HashSet<Unit> Units {get; private set;} = new();

    public HashSet<Unit> AttackingUnits {get; private set;} = new();
    public HashSet<Unit> MovingUnits {get; private set;} = new();

    [SerializeField] private Canvas healthBarCanvas;


    void Awake()
    {
        Instance = this;
        Units = new();
    }

    /// <summary>
    /// ユニットのアニメーションが終了しているかを取得する
    /// </summary>
    /// <returns>true = 終了している,false = まだ終了していない</returns>
    public bool AreAllUnitsIdle() //要検討(効率が悪い)
    {
        foreach (Unit unit in Units)
        {
            if (unit.ActionState != UnitActionState.Idle)
            {
                return false;
            }
        }
        return true;
    }

    // public void StartAttack() //todo EnemyManagerのStartEnemyTurnの代わりに使う
    // {
    //     foreach(Unit unit in AttackingUnits)
    //     {
    //         if(unit is Player) return;
    //         unit.Attack();
    //     }
    // }

    // public void StartMove() //todo EnemyManagerのStartEnemyTurnの代わりに使う
    // {
    //     foreach(Unit unit in AttackingUnits)
    //     {
    //         if(unit is Player) return;
    //         unit.Move();
    //     }
    // }

    /// <summary>
    /// List<Unit>への追加とHPバーの追加を行う
    /// </summary>
    /// <param name="unit"></param>
    public bool AddUnit(Unit unit)
    {
        SetSlider(unit);
        return Units.Add(unit);
    }

    public bool RemoveUnit(Unit unit)
    {
        EndAnim(unit);
        return Units.Remove(unit);
    }

    /// <summary>
    /// Unitが攻撃アニメーションを始めたときに呼び出す
    /// </summary>
    public bool OnStartAttack(Unit unit) => AttackingUnits.Add(unit);

    /// <summary>
    /// Unitが移動アニメーションを始めたときに呼び出す
    /// </summary>
    public bool OnStartMove(Unit unit) => MovingUnits.Add(unit);
    
    /// <summary>
    /// Unitのアニメーションが終わったときに呼び出す
    /// </summary>
    public bool EndAnim(Unit unit) => MovingUnits.Remove(unit) || AttackingUnits.Remove(unit);

    /// <summary>
    /// UnitにHPバーを追加する
    /// </summary>
    /// <param name="unit"></param>
    public void SetSlider(Unit unit)
    {
        Canvas canvasInstance = Instantiate(healthBarCanvas, unit.transform);
    
        Slider slider = canvasInstance.GetComponentInChildren<Slider>();
        
        if (slider != null)
        {
            unit.SetHPSlider(slider);
        }
    }

}

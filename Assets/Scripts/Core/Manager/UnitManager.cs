using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitManager : MonoBehaviour
{

    // public Dictionary<Vector2Int,Unit> UnitsDic {get; private set;} = new();
    // public Unit[,] UnitsMap {get; private set;}

    public static UnitManager Instance {get; private set;}
    public HashSet<Unit> Units {get; private set;} = new();

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
    public bool AreAllUnitsIdle() //要検討
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

    /// <summary>
    /// List<Unit>への追加とHPバーの追加を行う
    /// </summary>
    /// <param name="unit"></param>
    public void AddUnit(Unit unit)
    {
        Units.Add(unit);
        Canvas canvasInstance = Instantiate(healthBarCanvas, unit.transform);
    
        Slider slider = canvasInstance.GetComponentInChildren<Slider>();
        
        if (slider != null)
        {
            unit.SetHPSlider(slider);
        }
    }

    public void RemoveUnit(Unit unit)
    {
        if (Units.Contains(unit)) Units.Remove(unit);
    }


}

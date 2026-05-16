using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnitManager : MonoBehaviour //削除予定
{

    public static UnitManager Instance {get; private set;}
    public HashSet<Unit> Units {get; private set;} = new();



    [SerializeField] private Canvas healthBarCanvas;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            enabled = false;
            Debug.LogError($"{this}が複数存在しています。");
        }
    }



    // public bool SpownUnit(UnitData unitData, Vector2Int pos)
    // {
        
    // }


    /// <summary>
    /// List<Unit>への追加とHPバーの追加を行う
    /// </summary>
    /// <param name="unit"></param>
    public bool AddUnit(Unit unit)
    {
        if (unit == null) return false;
        SetSlider(unit);
        return Units.Add(unit);
    }

    public bool RemoveUnit(Unit unit)
    {
        if (unit == null) return false;
        return Units.Remove(unit);
    }


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

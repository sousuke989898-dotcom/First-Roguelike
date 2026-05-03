using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitManager : MonoBehaviour //削除予定
{

    public static UnitManager Instance {get; private set;}
    public HashSet<Unit> Units {get; private set;} = new();


    public HashSet<Unit> ActingUnits {get; private set;} = new();
    public HashSet<Unit> WillAttack{get; private set;} = new();

    [SerializeField] private Canvas healthBarCanvas;


    void Awake()
    {
        if (Instance != null) Instance = this;
        else
        {
            enabled = false;
            Debug.LogError($"{this}が複数存在しています。");
        }
    }

    /// <summary>
    /// ユニットのアニメーションが終了しているかを取得する
    /// </summary>
    /// <returns>true = 終了している,false = まだ終了していない</returns>
    public bool AreAllUnitsIdle()
    {
        return ActingUnits.Count == 0;
    }


    /// <summary>
    /// List<Unit>への追加とHPバーの追加を行う
    /// </summary>
    /// <param name="unit"></param>
    public bool AddUnit(Unit unit)
    {
        if (unit == null) return false;
        SetSlider(unit);
        unit.OnStartAction +=  StartAction;
        unit.OnEndAction   +=  EndAction;
        return Units.Add(unit);
    }


    public bool RemoveUnit(Unit unit)
    {
        if (unit == null) return false;

        // メモリリーク防止：登録したイベントを必ず解除する
        unit.OnStartAction -= StartAction;
        unit.OnEndAction   -= EndAction;

        ActingUnits.Remove(unit);
        return Units.Remove(unit);
    }

    private void StartAction(Unit unit)
    {
        ActingUnits.Add(unit);
    }

    private void EndAction(Unit unit)
    {
        ActingUnits.Remove(unit);
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

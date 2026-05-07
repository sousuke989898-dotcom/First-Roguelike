using UnityEngine;
using System.Collections.Generic;

public enum Turn {Player, Enemy}
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            enabled = false;
            Debug.LogError($"{this}が複数存在しています。");
        }
    }

    


    public Turn CurrentTurn {get; private set;} = Turn.Player;

    public List<Unit> PlanningToMoveUnits {get; private set;} = new();
    public List<Unit> PlanningToAttackUnits {get; private set;} = new();

    void Update()
    {
        if (CurrentTurn == Turn.Player)
        {
            if (StartPlayerTurn())
            {
                
            }
        }
        else
        {
            if (StartEnemyTurn())
            {
                
            }
        }
    }


    public bool CanStartTurn(Turn turn)
    {
        return UnitManager.Instance.AreAllUnitsIdle() && CurrentTurn == turn;
    }

    private bool StartEnemyTurn()
    {
        if (CurrentTurn != Turn.Enemy) return false;
        foreach(Unit unit in UnitManager.Instance.Units)
        {
            if (unit is Enemy enemy)
            {
                enemy.TakeTurn(PlanningToMoveUnits, PlanningToAttackUnits);
            }
        }
        return true;
    }

    public bool StartPlayerTurn()
    {
        if (CurrentTurn != Turn.Player) return false;
        return GameManager.Instance.CurrentPlayer.TakeTurn(PlanningToMoveUnits, PlanningToAttackUnits);
    }


    public void EndTurn(Turn turn) //削除予定
    {
        CurrentTurn = (turn == Turn.Enemy) ? Turn.Player : Turn.Enemy;
    }

    // public void ChangeTurn() //削除予定
    // {
    //     if (CurrentTurn == Turn.Player) StartEnemyTurn();
    //     else StartEnemyTurn();
    // }


    public void StartTurn()
    {
        //todo 移動と攻撃の命令をする
    }


    // public void EndPlayerTurn()
    // {
    //     if (CurrentTurn != Turn.Player) return;
    //     CurrentTurn = Turn.Enemy;
    // }

    // public void EndEnemyTurn()
    // {
    //     if (CurrentTurn != Turn.Enemy) return;
    //     CurrentTurn = Turn.Player;
    // }


}

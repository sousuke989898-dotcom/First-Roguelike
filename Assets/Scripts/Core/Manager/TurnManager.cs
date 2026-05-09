using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

    public HashSet<Unit> PlanningToMoveUnits {get; private set;} = new();
    public HashSet<Unit> PlanningToAttackUnits {get; private set;} = new();

    private bool acted = false;


    public IEnumerator StartTurnRoutine()
    {
        while (true)
        {
            if (CurrentTurn == Turn.Player)
            {
                yield return new WaitUntil(() => GameManager.Instance.CurrentPlayer.ActionReservation.Count > 0);
                PlayerTakeTurn();
            }
            else
            {
                Debug.Log(CurrentTurn.ToString());
                EnemeisTakeTurn();
            }
            acted = false;

            if (UnitManager.Instance.AreAllUnitsIdle())
            {
                yield return StartCoroutine(StartUnitsMove(PlanningToMoveUnits));
                yield return StartCoroutine(StartUnitsAttack(PlanningToAttackUnits));
            }

            if (acted)
            {
                Debug.Log(CurrentTurn.ToString());
                EndTurn(CurrentTurn);
            }
            yield return null;
        }
    }

    void Update()
    {
        
        // if (UnitManager.Instance.AreAllUnitsIdle())
        // {
        //     if (CurrentTurn == Turn.Player) EnemeisTakeTurn();
        //     else PlayerTakeTurn();
        //     StartCoroutine(StartUnitsMove(UnitManager.Instance.PlanningToMoveUnits));
        //     StartCoroutine(StartUnitsAttack(UnitManager.Instance.PlanningToAttackUnits));
        // }
    }


    public void EndTurn(Turn turn) //削除予定
    {
        CurrentTurn = (turn == Turn.Enemy) ? Turn.Player : Turn.Enemy;
    }

    public void EnemeisTakeTurn()
    {
        foreach(Unit unit in UnitManager.Instance.Units)
        {
            if (unit is Enemy enemy)
            {
                enemy.DecideAction(PlanningToMoveUnits,PlanningToAttackUnits);
            }
        }
    }

    public void PlayerTakeTurn()
    {
        GameManager.Instance.CurrentPlayer.DecideAction(PlanningToMoveUnits,PlanningToAttackUnits);
    }

    public IEnumerator StartUnitsMove(HashSet<Unit> units)
    {
        if (units.Count == 0) yield break;
        acted = true;
        foreach (Unit unit in units)
        {
            StartCoroutine(unit.MoveCoroutine());
        }
        yield return new WaitUntil(() => UnitManager.Instance.AreAllUnitsIdle());
    }

    public IEnumerator StartUnitsAttack(HashSet<Unit> units)
    {
        if (units.Count == 0) yield break;
        acted = true;
        foreach (Unit unit in units)
        {
            StartCoroutine(unit.AttackCoroutine());
        }
        yield return new WaitUntil(() => UnitManager.Instance.AreAllUnitsIdle());
    }



}

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

    private HashSet<Unit> _activeMovingUnits = new();
    private HashSet<Unit> _activeAttackingUnits = new();

    public HashSet<Unit> PlanningToMoveUnits {get; private set;} = new();
    public HashSet<Unit> PlanningToAttackUnits {get; private set;} = new();


    public IEnumerator StartTurnRoutine()
    {
        while (true)
        {
            HashSet<Unit> planningToMoveUnits = new();
            HashSet<Unit> planningToAttackUnits = new();

            if (CurrentTurn == Turn.Player)
            {
                yield return new WaitUntil(() => GameManager.Instance.CurrentPlayer.ActionReservation.Count > 0);
                GameManager.Instance.CurrentPlayer.DecideAction(planningToMoveUnits, planningToAttackUnits);
            }
            else
            {
                foreach (Unit unit in UnitManager.Instance.Units)
                {
                    if (unit is Enemy enemy)
                    {
                        enemy.DecideAction(planningToMoveUnits, planningToAttackUnits);
                    }
                }
            }

            Debug.Log(planningToMoveUnits.Count + " " + planningToAttackUnits.Count + " " + GameManager.Instance.CurrentPlayer.ActionReservation.ToArray().ToString());

            if (planningToMoveUnits.Count > 0)
            {
                foreach (Unit unit in planningToMoveUnits)
                {
                    _activeMovingUnits.Add(unit);
                    unit.OnEndAction += OnUnitMoveEnd;
                    StartCoroutine(unit.MoveCoroutine());
                }
                yield return new WaitUntil(() => _activeMovingUnits.Count == 0);
            }

            if (planningToAttackUnits.Count > 0)
            {
                foreach (Unit unit in planningToAttackUnits)
                {
                    _activeAttackingUnits.Add(unit);
                    unit.OnEndAction += OnUnitAttackEnd;
                    StartCoroutine(unit.AttackCoroutine());
                }
                yield return new WaitUntil(() => _activeAttackingUnits.Count == 0);
            }

            yield return new WaitUntil(() => AreAllUnitsIdle());

            ChangeTurn();

            yield return null;
        }
    }

    private bool AreAllUnitsIdle()
    {
        foreach (Unit unit in UnitManager.Instance.Units)
        {
            if (unit.ActionState != UnitActionState.Idle) return false;
        }
        return true;
    }


    public void ChangeTurn() 
    {
        CurrentTurn = CurrentTurn == Turn.Player ? Turn.Enemy : Turn.Player;
    }

    private void OnUnitMoveEnd(Unit unit)
    {
        unit.OnEndAction -= OnUnitMoveEnd;
        _activeMovingUnits.Remove(unit);
    }

    private void OnUnitAttackEnd(Unit unit)
    {
        unit.OnEndAction -= OnUnitAttackEnd;
        _activeAttackingUnits.Remove(unit);
    }



}

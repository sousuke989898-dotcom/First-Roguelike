using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Game.UnitSystem.UnitCommand;

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


    public IEnumerator StartRoutine()
    {
        while (true)
        {
            List<UnitCommand> unitCommands = new();
            if (CurrentTurn == Turn.Player)
            {
                yield return new WaitUntil(() => GameManager.Player.IsPlaningAction);
                UnitCommand playerCmd = GameManager.Player.DicideAction();
                if (playerCmd != null) unitCommands.Add(playerCmd);

            }
            else
            {
                foreach(Unit unit in UnitManager.Instance.Units)
                {
                    if (unit is Enemy enemy)
                    {
                        UnitCommand enemyCmd = enemy.DicideAction();
                        if (enemyCmd != null) unitCommands.Add(enemyCmd);
                    }
                }
            }

            foreach (UnitCommand command in unitCommands)
            {
                yield return command.ExcuteRoutine();
            }


            // foreach (Unit unit in UnitManager.Instance.Units)
            // {
            //     unit.Status.OnTurnEnd();
            // } //ここだとプレイヤーとエネミーが行動した後に動作し一ターンで二ターン分の処理が起こる

            ChangeTurn();

            yield return null;

        }
    }

    public void ChangeTurn()
    {
        CurrentTurn = (CurrentTurn == Turn.Player) ? Turn.Enemy : Turn.Player;
    }
}

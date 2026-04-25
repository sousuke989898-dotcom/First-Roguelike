using Unity.VisualScripting;
using UnityEngine;

public enum Turn {Player, Enemy}
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    void Awake()
    {
        Instance = this;
    }


    public Turn CurrentTurn {get; private set;}

    void Update()
    {
        if (CanStartTurn(Turn.Enemy))
        {
            StartEnemyTurn();
            EndTurn(Turn.Enemy); //todo プレイヤーのターンにするのをここでやらず、Enemyの行動が終わってからにする
        }
    }

    public bool CanStartTurn(Turn turn)
    {
        return UnitManager.Instance.AreAllUnitsIdle() && CurrentTurn == turn;
    }

    private void StartEnemyTurn()
    {
        EnemyManager.Instance.StartEnemyTurn();
    }

    public void EndTurn(Turn turn)
    {
        CurrentTurn = (turn == Turn.Enemy) ? Turn.Player : Turn.Enemy;
    }

    public void ChangeTurn()
    {
        if (CurrentTurn == Turn.Player) StartEnemyTurn();
        else StartEnemyTurn();
    }

}

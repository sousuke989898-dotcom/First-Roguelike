using UnityEngine;

public enum Turn {Player, Enemy}
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    void Awake()
    {
        if (Instance != null) Instance = this;
        else
        {
            enabled = false;
            Debug.LogError($"{this}が複数存在しています。");
        }
    }


    public Turn CurrentTurn {get; private set;} = Turn.Player;

    void Update()
    {
        if (CurrentTurn == Turn.Player)
        {
            if (GameManager.Instance.CurrentPlayer.TakeTurn())
            {
                EndTurn(Turn.Player);
            }
        }
        else
        {
            StartEnemyTurn();
            EndTurn(Turn.Enemy);
        }

        // if (CanStartTurn(Turn.Enemy))
        // {
        //     StartEnemyTurn();
        //     EndTurn(Turn.Enemy); //todo プレイヤーのターンにするのをここでやらず、Enemyの行動が終わってからにする
        // }
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

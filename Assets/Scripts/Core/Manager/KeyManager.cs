using System.Collections.Generic;
using UnityEngine;

public enum InputCommand { None, Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight, Wait }

public class KeyManager : MonoBehaviour
{
    private Dictionary<KeyCode, InputCommand> keyBindings;

    private Dictionary<InputCommand, Vector2Int> keyMovements;

    private float inputBufferTimer = 0f;
    public const float combinationWindow = 0.04f; 

    private List<InputCommand> inputCommands = new();

    void Awake()
    {

        keyMovements = new()
        {
            {InputCommand.Up, Vector2Int.up},
            {InputCommand.Down, Vector2Int.down},
            {InputCommand.Left, Vector2Int.left},
            {InputCommand.Right, Vector2Int.right},
            
        };


        keyBindings = new()
        {
            {KeyCode.W, InputCommand.Up}, {KeyCode.UpArrow, InputCommand.Up},
            {KeyCode.S, InputCommand.Down}, {KeyCode.DownArrow, InputCommand.Down},
            {KeyCode.A, InputCommand.Left}, {KeyCode.LeftArrow, InputCommand.Left},
            {KeyCode.D, InputCommand.Right}, {KeyCode.RightArrow, InputCommand.Right},
            // {KeyCode.Q, InputCommand.UpLeft},
            // {KeyCode.E, InputCommand.UpRight},
            // {KeyCode.Z, InputCommand.DownLeft},
            // {KeyCode.C, InputCommand.DownRight}
        };
    }

    public void Update()
    {
        if (!TurnManager.Instance.CanStartTurn(Turn.Player)) return;

        inputBufferTimer -= Time.deltaTime;
        GetInputCommand();

        if (inputBufferTimer <= 0)
        {
            if (inputCommands.Count != 0)
            if (ExecuteMove())
            {
                TurnManager.Instance.EndTurn(Turn.Player);
            }
        }
    }



    private void GetInputCommand()
    {
        foreach (var binding in keyBindings)
        {
            if (Input.GetKeyDown(binding.Key))
            {
                InputCommand currentInput = binding.Value;
                if (inputCommands.Contains(currentInput)) continue;

                var combined = GetDiagonalComponents(currentInput);
                if (combined.Count > 0)
                {
                    inputCommands = combined;
                    inputBufferTimer = 0f;
                    return;
                }

                inputCommands.Add(currentInput);

                if (keyMovements.ContainsKey(binding.Value) && inputCommands.Count == 1) //今後移動以外の別のショートカットキーを追加することを想定して
                {
                    inputBufferTimer = combinationWindow;
                }
                else
                {
                    inputBufferTimer = 0f;
                }
            }
        }
    }

    private bool ExecuteMove()
    {
        Vector2Int direction = Vector2Int.zero;
        foreach(InputCommand command in inputCommands)
        {
            if (keyMovements[command] != null)
            {
                direction += keyMovements[command];
            }
        }

        inputCommands.Clear();

        if (direction == Vector2Int.zero) return false;
        Player player = GameManager.Instance.CurrentPlayer;
        return player.Interact(direction);
    }

    /// <summary>
    /// 斜め移動の向きを分解する
    /// </summary>
    /// <param name="command">InputComand</param>
    /// <returns>引数が斜め移動だったら分解したものを、そうでなかったら何も入っていないものを返す</returns>
    private List<InputCommand> GetDiagonalComponents(InputCommand command)
    {
        return command switch
        {
            InputCommand.UpLeft    => new() { InputCommand.Up,   InputCommand.Left },
            InputCommand.UpRight   => new() { InputCommand.Up,   InputCommand.Right },
            InputCommand.DownLeft  => new() { InputCommand.Down, InputCommand.Left },
            InputCommand.DownRight => new() { InputCommand.Down, InputCommand.Right },
            _ => new List<InputCommand>()
        };
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class KeyManager : MonoBehaviour
{
    [SerializeField] private Tilemap walkStyleTilemap;

    private float inputBufferTimer = 0f;
    public const float combinationWindow = 0.04f; 

    private bool waitingForInput = false;

    private KeyCombo lastCombo = new(KeyCode.None);

    private Dictionary<KeyCombo,InputCommand> singleKeyBindings = new()
    {
        {new(KeyCode.W), InputCommand.Up},
        {new(KeyCode.D), InputCommand.Right},
        {new(KeyCode.S), InputCommand.Down},
        {new(KeyCode.A), InputCommand.Left},

        {new(KeyCode.UpArrow), InputCommand.Up},
        {new(KeyCode.RightArrow), InputCommand.Right},
        {new(KeyCode.DownArrow), InputCommand.Down},
        {new(KeyCode.LeftArrow), InputCommand.Left},
    };

    private Dictionary<KeyCombo,InputCommand> doubleKeyBindings = new()
    {
        {new(KeyCode.W,KeyCode.D), InputCommand.UpRight},
        {new(KeyCode.D,KeyCode.S), InputCommand.DownRight},
        {new(KeyCode.S,KeyCode.A), InputCommand.DownLeft},
        {new(KeyCode.A,KeyCode.W), InputCommand.UpLeft},

        {new(KeyCode.UpArrow,KeyCode.RightArrow), InputCommand.UpRight},
        {new(KeyCode.DownArrow,KeyCode.RightArrow), InputCommand.DownRight},
        {new(KeyCode.DownArrow,KeyCode.LeftArrow), InputCommand.DownLeft},
        {new(KeyCode.UpArrow,KeyCode.LeftArrow), InputCommand.UpLeft},
    };

    public static KeyManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            enabled = false;
            Debug.LogError($"{this}が複数存在しています。");
        }
    }

    public void Update()
    {
        if (lastCombo.GetStatus() != ComboStatus.None) return;
        lastCombo = KeyCombo.None;

        inputBufferTimer -= Time.deltaTime;
        InputCommand command = GetCommand();
        if (command != InputCommand.None)
        {
            ExecuteCommand(command);
        }
    }

    private InputCommand GetCommand()
    {
        bool anyPartial = false;
        foreach (var pair in doubleKeyBindings)
        {
            var status = pair.Key.GetStatus();
            if (status == ComboStatus.Press)
            {
                waitingForInput = false;
                lastCombo = pair.Key;
                return pair.Value;
            }
            if (status == ComboStatus.Partial)
            {

                anyPartial = true;
            }
        }

        if (anyPartial)
        {
            if (!waitingForInput)
            {
                inputBufferTimer = combinationWindow;
                waitingForInput = true;
            }
        }
        else
        {
            waitingForInput = false;
        }

        if (inputBufferTimer <= 0 || !waitingForInput)
        {
            foreach (var pair in singleKeyBindings)
            {
                if (pair.Key.GetStatus() == ComboStatus.Press)
                {
                    waitingForInput = false;
                    lastCombo = pair.Key;
                    return pair.Value;
                }
            }
        }
        return InputCommand.None;
    }

    private bool ExecuteCommand(InputCommand command)
    {
        if (InputCommandTool.keyMovements.TryGetValue(command,out var dir))
        {
            Player player = GameManager.Player;
            if (player == null) return false;
            player.UnitMovement.AddPath(dir);
            return true;
        }
        return false;
    }
}

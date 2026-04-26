using System.Collections.Generic;

public enum InputCommand { None, Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight, Wait }

public static class InputCommandTool
{

    public static readonly Dictionary<InputCommand, Direction> keyMovements = new()
    {
        {InputCommand.Up, Direction.Upper},
        {InputCommand.Down, Direction.Down},
        {InputCommand.Left, Direction.Left},
        {InputCommand.Right, Direction.Right},
        {InputCommand.UpRight, Direction.Right | Direction.Upper},
        {InputCommand.DownRight, Direction.Right | Direction.Down},
        {InputCommand.UpLeft, Direction.Left | Direction.Upper},
        {InputCommand.DownLeft, Direction.Left | Direction.Down},
    };
}

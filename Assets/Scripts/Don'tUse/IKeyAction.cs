// using System.Collections.Generic;
// using UnityEngine;

// public enum InputCommand { None, Up, Down, Left, Right, Wait }

// public interface IHasKeyAction
// {
//     /// <summary>
//     /// キーごとの動作
//     /// </summary>
//     /// <param name="player">プレイヤー</param>
//     /// <returns>行動成功なら真、失敗なら偽</returns>
//     public bool Action(Player player);
// }

// public enum MoveDirection{Up, Down, Right, Left};
// /// <summary>
// /// 相対座標を持ち、プレイヤーを移動させる
// /// </summary>
// public class MoveKeyAction : IHasKeyAction
// {
//     /// <summary>
//     /// 実際の相対座標
//     /// </summary>
//     private Vector2Int vector2;


//     /// <param name="vector2Int">相対座標</param>
//     private MoveKeyAction(Vector2Int vector2Int)
//     {
//         vector2 = vector2Int;
//     }

//     public bool Action(Player player)
//     {
//         if (!UnitManager.Instance.IsUnitsAreIdle()) return false;
//         return player.Interact(vector2.x, vector2.y);
//     }

//     /// <summary>
//     /// 移動方向に基づいた相対座標の辞書
//     /// </summary>
//     private static Dictionary<MoveDirection, MoveKeyAction> Instances = new()
//     {
//         { MoveDirection.Up,    new MoveKeyAction(new( 0,  1)) },
//         { MoveDirection.Down,  new MoveKeyAction(new( 0, -1)) },
//         { MoveDirection.Left,  new MoveKeyAction(new(-1,  0)) },
//         { MoveDirection.Right, new MoveKeyAction(new( 1,  0)) },
//     };

//     /// <summary>
//     /// 移動方向に基づいた相対座標を取得する
//     /// </summary>
//     /// <param name="direction">移動方向</param>
//     public static MoveKeyAction GetMoveKeyAction(MoveDirection direction)
//     {
//         return Instances[direction];
//     }

// }



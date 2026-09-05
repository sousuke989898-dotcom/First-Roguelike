using System.Collections.Generic;
using UnityEngine;
namespace Game.GridMap
{
    public enum RoomTransformType
    {
        None,
        Rotate90,
        Rotate180,
        Rotate270,
        FlipHorizontal
    }

    public static class RoomTransformer
    {
        /// <summary>
        /// 指定された Room を変換（回転・反転）した新しい Room インスタンスを生成する
        /// </summary>
        public static Room CreateTransformedRoom(Room src, RoomTransformType transform)
        {
            if (transform == RoomTransformType.None || src == null) return src;

            int origW = src.roomData.size.x;
            int origH = src.roomData.size.y;

            // 90度/270度回転時は幅と高さを入れ替える
            bool isSwapped = (transform == RoomTransformType.Rotate90 || transform == RoomTransformType.Rotate270);
            int newW = isSwapped ? origH : origW;
            int newH = isSwapped ? origW : origH;

            var newTerrainMap = new MapObjectData[newW, newH];
            var newGimmickMap = new MapObjectData[newW, newH];
            var newLocalDoors = new Dictionary<Direction, Vector2Int>();

            // 1. 2Dタイルマップの変換
            for (int x = 0; x < origW; x++)
            {
                for (int y = 0; y < origH; y++)
                {
                    var (nx, ny) = ConvertCoordinates(x, y, origW, origH, transform);
                    newTerrainMap[nx, ny] = src.TerrainMap[x, y];
                    newGimmickMap[nx, ny] = src.GimmickMap[x, y];
                }
            }

            // 2. ドア座標と方向の変換
            foreach (var (dir, localPos) in src.LocalDoors)
            {
                var (nx, ny) = ConvertCoordinates(localPos.x, localPos.y, origW, origH, transform);
                Direction newDir = ConvertDirection(dir, transform);
                newLocalDoors[newDir] = new Vector2Int(nx, ny);
            }

            // 追加したコンストラクタを使って Room オブジェクトを生成
            return new Room(src.roomData, newTerrainMap, newGimmickMap, newLocalDoors);

        }

        private static (int nx, int ny) ConvertCoordinates(int x, int y, int w, int h, RoomTransformType transform)
        {
            return transform switch
            {
                RoomTransformType.Rotate90       => (y, w - 1 - x),
                RoomTransformType.Rotate180      => (w - 1 - x, h - 1 - y),
                RoomTransformType.Rotate270      => (h - 1 - y, x),
                RoomTransformType.FlipHorizontal  => (w - 1 - x, y),
                _ => (x, y)
            };
        }

        private static Direction ConvertDirection(Direction dir, RoomTransformType transform)
        {
            return transform switch
            {
                RoomTransformType.Rotate90 => dir switch
                {
                    Direction.Upper => Direction.Right,
                    Direction.Right => Direction.Down,
                    Direction.Down  => Direction.Left,
                    Direction.Left  => Direction.Upper,
                    _ => dir
                },
                RoomTransformType.Rotate180 => dir switch
                {
                    Direction.Upper => Direction.Down,
                    Direction.Down  => Direction.Upper,
                    Direction.Left  => Direction.Right,
                    Direction.Right => Direction.Left,
                    _ => dir
                },
                RoomTransformType.Rotate270 => dir switch
                {
                    Direction.Upper => Direction.Left,
                    Direction.Left  => Direction.Down,
                    Direction.Down  => Direction.Right,
                    Direction.Right => Direction.Upper,
                    _ => dir
                },
                RoomTransformType.FlipHorizontal => dir switch
                {
                    Direction.Left  => Direction.Right,
                    Direction.Right => Direction.Left,
                    _ => dir
                },
                _ => dir
            };
        }
    }
}
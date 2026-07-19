using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.GridMap
{
    /// <summary>
    /// 部屋の形状（地形）と、Entityの配置情報を二次元配列で保持するクラス
    /// </summary>
    public class Room
    {
        public RoomData roomData;
        
        // ★誇らしげに2つの配列をそのまま持ちましょう！
        public TileType[,] TerrainLayer { get; private set; } // 地形層（Floor, Wall）
        public TileType[,] EntityLayer { get; private set; }  // Entity層（None, Door_Closedなど）

        // 1辺1ドア管理用の辞書も併用
        public Dictionary<Direction, Vector2Int> Doors { get; private set; } 

        public static Room CreateFromData(RoomData data)
        {
            Room room = new()
            {
                roomData = data,
                TerrainLayer = new TileType[data.size.x, data.size.y],
                EntityLayer = new TileType[data.size.x, data.size.y],
                Doors = new Dictionary<Direction, Vector2Int>()
            };

            Tilemap[] tilemaps = data.roomPrefab.GetComponentsInChildren<Tilemap>();
            Tilemap terrainMap = tilemaps[0];
            Tilemap entityMap = tilemaps[1];

            for (int x = 0; x < data.size.x; x++)
            {
                for (int y = 0; y < data.size.y; y++)
                {
                    // TileBase terrainTile = terrainMap.GetTile(new Vector3Int(x, y, 0));
                    // room.TerrainLayer[x, y] = tileMapping.GetTileType(terrainTile);

                    // TileBase entityTile = entityMap.GetTile(new Vector3Int(x, y, 0));
                    // TileType entityType = tileMapping.GetTileType(entityTile);
                    // room.EntityLayer[x, y] = entityType;

                    // // ドアの位置と向きを辞書に登録
                    // if (entityType == TileType.Door_Closed)
                    // {
                    //     Vector2Int localPos = new(x, y);
                    //     Direction dir = ConvertPositionToDirection(localPos, data.size);
                    //     if (!room.Doors.ContainsKey(dir))
                    //     {
                    //         room.Doors.Add(dir, localPos);
                    //     }
                    // }
                }
            }
            return room;
        }
        private static Direction ConvertPositionToDirection(Vector2Int localPos, Vector2Int roomSize)
        {
            if (localPos.y == roomSize.y - 1) return Direction.Upper; // 最上列なら上辺
            if (localPos.y == 0) return Direction.Down;  // 最下列なら下辺
            if (localPos.x == 0) return Direction.Left;  // 最左列なら左辺
            if (localPos.x == roomSize.x - 1) return Direction.Right; // 最右列なら右辺

            return Direction.None; // 部屋の内部（基本はあり得ない）
        }
    }
}
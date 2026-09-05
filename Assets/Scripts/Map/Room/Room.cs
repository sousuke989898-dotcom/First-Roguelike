using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.GridMap
{
    public class Room
    {
        public RoomData roomData {get; private set;}
        public MapObjectData[,] TerrainMap {get; private set;}
        public MapObjectData[,] GimmickMap {get; private set;}

        public IReadOnlyDictionary<Direction, Vector2Int> LocalDoors => localDoors;
        private readonly Dictionary<Direction, Vector2Int> localDoors = new();


        public Room(RoomData roomData, MapDatabase database)
        {
            this.roomData = roomData;

            ParsePrefabTilemaps(database);
        }

        /// <summary>
        /// 【追加】回転・反転データからの生成用コンストラクタ
        /// </summary>
        public Room(RoomData roomData, MapObjectData[,] terrainMap, MapObjectData[,] gimmickMap, Dictionary<Direction, Vector2Int> localDoors)
        {
            this.roomData = roomData;
            this.TerrainMap = terrainMap;
            this.GimmickMap = gimmickMap;
            this.localDoors = localDoors ?? new Dictionary<Direction, Vector2Int>();
        }

        private void ParsePrefabTilemaps(MapDatabase database)
        {
            // ---  部屋Prefabから Terrain 用の Tilemap を取得 ---
            Tilemap[] tilemaps = roomData.roomPrefab.GetComponentsInChildren<Tilemap>();
            Tilemap terrainTilemap = null;
            Tilemap gimmickTilemap = null;

            foreach (var tilemap in tilemaps)
            {
                if (tilemap.gameObject.name == "Terrain") terrainTilemap = tilemap;
                else if(tilemap.gameObject.name == "Gimmick") gimmickTilemap = tilemap;
            }

            if (terrainTilemap == null || gimmickTilemap == null)
            {
                Debug.LogError($"[Room] {roomData.roomPrefab.name} に 'Terrain' または 'Gimmick' Tilemap が見つかりません。");
                return;
            }

            int width = roomData.size.x;
            int height = roomData.size.y;

            TerrainMap = new MapObjectData[width, height];
            GimmickMap = new MapObjectData[width, height];
            localDoors.Clear();

            BoundsInt tBounds = terrainTilemap.cellBounds;
            BoundsInt gBounds = gimmickTilemap.cellBounds;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Terrain層の解析
                    Vector3Int tPos = new(tBounds.xMin + x, tBounds.yMin + y, 0);
                    TileBase tTile = terrainTilemap.GetTile(tPos);
                    TerrainMap[x, y] = database.GetObjectDataFromTile(tTile);

                    // Gimmick層の解析
                    Vector3Int gPos = new(gBounds.xMin + x, gBounds.yMin + y, 0);
                    TileBase gTile = gimmickTilemap.GetTile(gPos);
                    GimmickMap[x, y] = database.GetObjectDataFromTile(gTile); // 必要に応じて GimmickMap[x, y] に格納

                    // ドアタイルの検知
                    if (database.IsDoorTile(gTile))
                    {
                        Vector2Int localPos = new(x, y);
                        Direction dir = DetermineDoorDirection(roomData.size, localPos);

                        if (!localDoors.ContainsKey(dir))
                        {
                            localDoors.Add(dir, localPos);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 部屋のローカル座標から壁の方向（North/South/East/West）を特定する
        /// </summary>
        private Direction DetermineDoorDirection(Vector2Int roomSize, Vector2Int localPos)
        {
            if (localPos.y >= roomSize.y - 1) return Direction.Upper;
            if (localPos.y <= 0)              return Direction.Down;
            if (localPos.x >= roomSize.x - 1) return Direction.Right;
            if (localPos.x <= 0)              return Direction.Left;
            return Direction.None;
        }

    }
}
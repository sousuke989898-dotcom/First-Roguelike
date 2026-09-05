using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.GridMap
{
    [CreateAssetMenu(fileName = "MapDatabase", menuName = "Map/Map Database")]
    public class MapDatabase : ScriptableObject
    {
        [Header("基本地形データ")]
        public MapObjectData wallData;
        public MapObjectData floorData;
        public MapObjectData roadData;

        public MapObjectData DoorCloosedData;

        [Header("登録されているすべてのオブジェクト")]
        [SerializeField] private List<MapObjectData> allObjects = new();

        public MapObjectData GetObjectByName(string name)
        {
            return allObjects.Find(data => data != null && data.objectName == name);
        }

        public MapObjectData GetObjectDataFromTile(TileBase targetTile)
        {
            if (targetTile == null) return null;

            // リスト内から TileBase が一致するものを探す
            foreach (var data in allObjects)
            {
                if (data != null && data.tile == targetTile)
                {
                    return data;
                }
            }

            Debug.LogWarning($"[MapDatabase] TileBase '{targetTile.name}' に対応する MapObjectData が見つかりませんでした。");
            return null;
        }

        public bool IsDoorTile(TileBase tile)
        {
            if (tile == null || DoorCloosedData == null) return false;
            return DoorCloosedData.tile == tile;
        }
    }
}
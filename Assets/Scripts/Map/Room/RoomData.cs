using UnityEngine;

namespace Game.GridMap
{
    [CreateAssetMenu(fileName = "NewRoomData", menuName = "Dungeon/Room Data")]
    public class RoomData : ScriptableObject
    {
        [Header("部屋のPrefab (Tilemapを含む)")]
        public GameObject roomPrefab;

        [Header("この部屋のカテゴリ")]
        public RoomCategory category;

        [Header("部屋のサイズ")]
        public Vector2Int size;
    }
}
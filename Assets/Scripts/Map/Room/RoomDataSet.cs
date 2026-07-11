using System.Collections.Generic;
using UnityEngine;

namespace Game.GridMap
{
    [CreateAssetMenu(fileName = "NewRoomDataset", menuName = "Dungeon/Room Dataset")]
    public class RoomDataset : ScriptableObject
    {
        [Header("登録されているすべての部屋プリセット")]
        public List<RoomData> allRooms;

        /// <summary>
        /// 特定のカテゴリの部屋だけをフィルタリングして取得する
        /// </summary>
        public List<RoomData> GetRoomsByCategory(RoomCategory category)
        {
            return allRooms.FindAll(r => r.category == category);
        }
    }
}
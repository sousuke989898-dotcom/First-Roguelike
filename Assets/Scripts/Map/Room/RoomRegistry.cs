using System.Collections.Generic;
using UnityEngine;

namespace Game.GridMap
{
    public class RoomRegistry
    {
        // 変換済みのRoomクラスをカテゴリごとに分けてキャッシュする辞書
        private Dictionary<RoomCategory, List<Room>> _cachedRooms;

        /// <summary>
        /// 初期化時にScriptableObjectからすべてのデータを一括パース（変換）する
        /// </summary>
        public RoomRegistry(RoomDataset dataset, TileMapping tileMapping)
        {
            _cachedRooms = new Dictionary<RoomCategory, List<Room>>();

            foreach (var roomData in dataset.allRooms)
            {
                // ここで前述のファクトリメソッド等を使ってRoomに変換
                Room room = Room.CreateFromData(roomData, tileMapping);

                if (!_cachedRooms.ContainsKey(roomData.category))
                {
                    _cachedRooms[roomData.category] = new List<Room>();
                }
                _cachedRooms[roomData.category].Add(room);
            }
        }

        /// <summary>
        /// MapGeneratorから呼び出され、条件に合う変換済みRoomをランダムに返す
        /// </summary>
        public Room GetRandomRoom(RoomCategory category, Vector2Int maxSize)
        {
            if (!_cachedRooms.TryGetValue(category, out var rooms)) return null;

            // サイズが収まる部屋だけをフィルタリング
            var validRooms = rooms.FindAll(r => r.roomData.size.x <= maxSize.x && r.roomData.size.y <= maxSize.y);

            if (validRooms.Count == 0) return null;

            int randomIndex = Random.Range(0, validRooms.Count);
            return validRooms[randomIndex];
        }
    }
}
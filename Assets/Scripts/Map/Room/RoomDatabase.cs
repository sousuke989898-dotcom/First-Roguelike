using System.Collections.Generic;
using UnityEngine;

namespace Game.GridMap
{
    [CreateAssetMenu(fileName = "NewRoomDataset", menuName = "Dungeon/Room Database")]
    public class RoomDatabase : ScriptableObject
    {
        [Header("登録されているすべての部屋プリセット")]
        public List<RoomData> allRooms;

        // (RoomData, 回転タイプ) をキーにして、生成済みの Room オブジェクトを保持
        private readonly Dictionary<(RoomData data, RoomTransformType transform), Room> roomCache = new();

        /// <summary>
        /// キャッシュから回転済み Room を取得する（初回要求時のみ生成してキャッシュ）
        /// </summary>
        public Room GetRoom(RoomData data, RoomTransformType transform, MapDatabase mapDatabase)
        {
            if (data == null) return null;

            var key = (data, transform);

            // 1. すでにキャッシュにあればそのまま返す
            if (roomCache.TryGetValue(key, out Room cachedRoom))
            {
                return cachedRoom;
            }

            // 2. キャッシュにない場合の生成処理
            Room resultRoom;

            if (transform == RoomTransformType.None)
            {
                // 変換なし（基本）の場合は Prefab から作成
                resultRoom = new Room(data, mapDatabase);
            }
            else
            {
                // 変換ありの場合は、まず 0度の基本Room を取得（これは再帰的にキャッシュされる）
                Room baseRoom = GetRoom(data, RoomTransformType.None, mapDatabase);
                resultRoom = RoomTransformer.CreateTransformedRoom(baseRoom, transform);
            }

            // キャッシュに登録して返す
            roomCache[key] = resultRoom;
            return resultRoom;
        }

        /// <summary>
        /// 変換なし（0度）の基本 Room を取得するショートカット
        /// </summary>
        public Room GetParsedRoom(RoomData roomData, MapDatabase database)
        {
            return GetRoom(roomData, RoomTransformType.None, database);
        }

        /// <summary>
        /// シーン遷移時や再ロード時にキャッシュをクリアしたい場合用
        /// </summary>
        public void ClearCache() => roomCache.Clear();

        /// <summary>
        /// 特定のカテゴリの部屋だけをフィルタリングして取得する
        /// </summary>
        public List<RoomData> GetRoomsByCategory(RoomCategory category)
        {
            return allRooms.FindAll(r => r.category == category);
        }
    }
}
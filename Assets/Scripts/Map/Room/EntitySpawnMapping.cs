using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GridMap
{
    [CreateAssetMenu(fileName = "NewEntitySpawnMapping", menuName = "Dungeon/Entity Spawn Mapping")]
    public class EntitySpawnMapping : ScriptableObject
    {
        [Serializable]
        public struct SpawnEntry
        {
            public TileType type;
            public GameObject prefab;
        }

        public List<SpawnEntry> spawnMappings;

        public GameObject GetPrefab(TileType type)
        {
            var entry = spawnMappings.Find(m => m.type == type);
            return entry.prefab;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.GridMap
{
    [CreateAssetMenu(fileName = "NewTileMapping", menuName = "Dungeon/Tile Mapping")]
    public class TileMapping : ScriptableObject
    {
        [Serializable]
        public struct TileEntry
        {
            public TileBase tile;
            public TileType type;
        }

        public List<TileEntry> mappings;

        public TileType GetTileType(TileBase tile)
        {
            if (tile == null) return TileType.None;
            var entry = mappings.Find(m => m.tile == tile);
            return entry.tile != null ? entry.type : TileType.None;
        }

        public TileBase GetTileBase(TileType type)
        {
            var entry = mappings.Find(m => m.type == type);
            return entry.tile;
        }
    }
}
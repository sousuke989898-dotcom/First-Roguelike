using System.Collections.Generic;
using UnityEngine;

namespace Game.GridMap
{
    public class DungeonLevel
    {
        public TerrainMap Terrain { get; private set; }
        public EntityHolder Holder { get; private set; }

        public DungeonLevel(TileType[,] terrainMatrix, List<Section> sections)
        {
            Terrain = new TerrainMap();
            Terrain.Setup(terrainMatrix, sections);

            Holder = new EntityHolder(Terrain.Width, Terrain.Height);
        }

        public bool CanMoveUnit(Vector2Int absPos)
        {
            if (!Terrain.IsWalkable(absPos)) return false;

            if (Holder.GetUnit(absPos) != null) return false;

            return true;
        }
    }
}
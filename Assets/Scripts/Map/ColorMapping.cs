using System;
using UnityEngine;

namespace Game.GridMap
{
    [Serializable] 
    public struct ColorMapping
    {
        public Color color;
        public TileType tileType;
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Game.GridMap
{
    public class Room
    {
        private RoomData _roomData;
        
        public TileType[,] Terrain {get; private set;}
        public List<TilePoint> SpecialTiles {get; private set;}

        public Room(RoomData data)
        {
            _roomData = data;
            SpecialTiles = new List<TilePoint>();

            ParseTexture(data.RoomTexture);
        }

        private void ParseTexture(Texture2D texture)
        {
            if (texture == null) return;
            int w = texture.width;
            int h = texture.height;
            Terrain = new TileType[w,h];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Color color = texture.GetPixel(x, y);
                    Terrain[x,y] = ColorToTile(color);

                    Vector2Int localPos = new(x, y);
                    TilePoint point = GetTilePoint(Terrain[x,y], localPos);
                    if (point != null)
                    {
                        SpecialTiles.Add(point);
                    }

                }
            }
        }

        private TileType ColorToTile(Color color)
        {
            foreach (var mapping in _roomData.ColorMappings)
            {
                if (Mathf.Approximately(mapping.color.r, color.r) &&
                    Mathf.Approximately(mapping.color.g, color.g) &&
                    Mathf.Approximately(mapping.color.b, color.b))
                {
                    return mapping.tileType; 
                }
            }

            return TileType.Floor;
        }

        private TilePoint GetTilePoint(TileType type, Vector2Int pos)
        {
            switch (type)
            {
                case TileType.UpStairs:
                case TileType.DownStairs:
                case TileType.Door:
                    return new TilePoint(type, pos);
                default :
                    return null;
            };
        }

    }

    /// <summary>
    /// 特殊なタイルの位置を保持するクラス
    /// </summary>
    public class TilePoint
    {
        public TileType type; //タイル情報
        public Vector2Int localPos; //位置情報

        public TilePoint(TileType type, Vector2Int pos)
        {
            this.type = type;
            this.localPos = pos;
        }
    }
}
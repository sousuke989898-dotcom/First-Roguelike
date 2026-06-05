using System.Collections.Generic;
using UnityEngine;

namespace Game.GridMap
{
    /// <summary>
    /// 部屋の形状の二次元配列と、特殊なタイルを取得するクラス
    /// </summary>
    public class Room
    {
        private RoomData _roomData;
        
        public TileType[,] Terrain {get; private set;}
        public List<TilePoint> SpecialTiles {get; private set;} //扉、宝箱、罠などの、ギミックが存在するもの

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
                    TileType tile = ColorToTile(color);
                    Terrain[x,y] = tile;

                    Vector2Int localPos = new(x, y);

                    if (tile == TileType.Door)
                    {
                        Direction dir = Direction.None;

                        if (x == 0)          dir = Direction.Left;  //左端
                        else if (x == w - 1) dir = Direction.Right; //右端
                        else if (y == 0)     dir = Direction.Down;  //上端
                        else if (y == h - 1) dir = Direction.Upper; //下端
                        //部屋の角に存在するアクセスできないドアは考慮していないため注意

                        if (dir == Direction.None)
                        {
                            SpecialTiles.Add(new TilePoint(tile, localPos));
                        }
                        else
                        {
                            SpecialTiles.Add(new DirectedPoint(tile, localPos, dir));
                        }
                    }
                    else if (tile == TileType.UpStairs || tile == TileType.DownStairs)
                    {
                        SpecialTiles.Add(new TilePoint(tile, localPos));
                    }

                }
            }
        }

        private TileType ColorToTile(Color color)
        {
            if (_roomData.Palette == null) return TileType.Floor;

            foreach (var mapping in _roomData.Palette.Mappings)
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


    }

}
using System.Collections.Generic;
using UnityEngine;

namespace Game.GridMap
{
    /// <summary>
    /// ダンジョンの静的地形 (床・壁・部屋構造) のデータと判定を管理するクラス。
    /// キャラクターやアイテムなどの動的オブジェクト (Entity) の情報は含めず、純粋なマップグリッド情報のみを保持する。
    /// </summary>
    public class TerrainMap
    {
        public TileType[,] Matrix {get; protected set;}

        public int Width => Matrix.GetLength(0);
        public int Height => Matrix.GetLength(1);

        public List<Section> Sections {get; protected set;}

        public void Setup(TileType[,] matrix, List<Section> sections)
        {
            Matrix = matrix;
            Sections = sections;
        }

        public bool IsInsideMap(Vector2Int absPos) =>
        absPos.x >= 0 && absPos.x < Width && absPos.y >= 0 && absPos.y < Height;

        public TileType GetTileType(Vector2Int absPos) =>
            IsInsideMap(absPos) ? Matrix[absPos.x, absPos.y] : TileType.None;

        /// <summary>
        /// 指定された座標が、地形として通行可能（歩行可能）かどうかを判定する。
        /// </summary>
        /// <param name="absPos">マップ上の絶対グリッド座標</param>
        /// <returns>通行可能な床や道であれば true。壁またはマップ外であれば false。</returns>
        public bool IsWalkable(Vector2Int absPos) =>
            IsInsideMap(absPos) && Matrix[absPos.x, absPos.y].CanMove();
    }
}
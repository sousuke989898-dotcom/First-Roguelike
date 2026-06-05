using UnityEngine;

namespace Game.GridMap
{
    /// <summary>
    /// 特殊なタイルの位置情報を保持する基底クラス（階段、宝箱など向きがないもの）
    /// </summary>
    public class TilePoint
    {
        public TileType type;
        public Vector2Int localPos;


        public TilePoint(TileType type, Vector2Int localPos)
        {
            this.type = type;
            this.localPos = localPos;
        }
    }

    /// <summary>
    /// 向きの概念を持つ特殊タイルのクラス（ドアや特定のギミックなど）
    /// </summary>
    public class DirectedPoint : TilePoint
    {
        public Direction direction;

        public DirectedPoint(TileType type, Vector2Int localPos, Direction dir) : base(type, localPos)
        {
            direction = dir;
        }
    }
}
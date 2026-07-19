
namespace Game.GridMap
{
    // public enum TileType { None, Wall, Floor, DoorClosed, DoorOpen } // 元々TileTypeという名前のEnumを使っていたため
    public enum ItemType { None, Sword, Potion }
    public enum CharacterType { None, Player, Goblin }

    public class MapCell
    {

        public MapObjectData Terrain { get; set; }   // 地形層
        public MapObjectData Gimmick { get; set; }   // ギミック層
        public MapObjectData Item { get; set; }      // アイテム層
        public MapObjectData Character { get; set; } // キャラクター層

        public bool IsVisible { get; set; }  // 現在プレイヤーの視界内（現在進行形で明るい）か
        public bool IsExplored { get; set; } // 一度でも視界に入った（ミニマップに常時映る）か

        /// <summary>
        /// このマスが移動を遮るかどうか
        /// </summary>
        public bool BlocksMovement
        {
            get
            {
                if (Terrain != null && Terrain.blocksMovement) return true;
                if (Gimmick != null && Gimmick.blocksMovement) return true;
                if (Character != null && Character.blocksMovement) return true;
                return false;
            }
        }

        /// <summary>
        /// このマスが視界（光）を遮るかどうか
        /// </summary>
        public bool BlocksVision
        {
            get
            {
                if (Terrain != null && Terrain.blocksVision) return true;
                if (Gimmick != null && Gimmick.blocksVision) return true;
                return false;
            }
        }

        /// <summary>
        /// ScriptableObjectのレイヤー情報(layerType)を元にデータを割り当てる
        /// </summary>
        public void AssignObject(MapObjectData data)
        {
            if (data == null) return;

            switch (data.layerType)
            {
                case MapLayerType.Terrain:
                    Terrain = data;
                    break;
                case MapLayerType.Gimmick:
                    Gimmick = data;
                    break;
                case MapLayerType.Item:
                    Item = data;
                    break;
                case MapLayerType.Character:
                    Character = data;
                    break;
            }
        }

        /// <summary>
        /// 指定されたレイヤーのデータを空にする
        /// </summary>
        public void ClearLayer(MapLayerType layer)
        {
            switch (layer)
            {
                case MapLayerType.Terrain: Terrain = null; break;
                case MapLayerType.Gimmick: Gimmick = null; break;
                case MapLayerType.Item: Item = null; break;
                case MapLayerType.Character: Character = null; break;
            }
        }

    }
}

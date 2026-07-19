using UnityEngine;
using UnityEngine.Tilemaps;
namespace Game.GridMap
{
    [CreateAssetMenu(fileName = "NewMapObjectData", menuName = "Map/Object Data")]
    public class MapObjectData : ScriptableObject
    {
        [Header("基本情報")]
        public string objectName;
        public MapLayerType layerType;

        [Header("パラメーター")]
        public bool blocksMovement; // ぶつかる（通行不可）か
        public bool blocksVision;   // 視界を遮るか

        [Header("見た目")]
        public TileBase tile;       // 静的オブジェクトならここにTileを登録
        public GameObject prefab;   // 動的オブジェクトならここにPrefabを登録
    }

    public enum MapLayerType
    {
        Terrain,
        Gimmick,
        Item,
        Character
    }
}
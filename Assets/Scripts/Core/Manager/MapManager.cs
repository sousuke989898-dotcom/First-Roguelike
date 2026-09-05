using System.Collections.Generic;
using Game.GridMap;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{

    [Header("描画先・配置先")]
    [SerializeField] private Tilemap MapTile;

    [Header("データアセット")]
    [SerializeField] private DungeonSettings dungeonSettings;

    public MapCell[,] MapCells {get; private set;}
    public SectionNode RootNode {get; private set;}

    public int SizeX => dungeonSettings.mapSizeX;
    public int SizeY => dungeonSettings.mapSizeY;
    public MapData MapData; //todo 削除

    public EntityHolder entitys;

    public static MapManager Instance {get; private set;}
    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            enabled = false;
            Debug.LogError($"{this}が複数存在しています。");
            return;
        }

        InitializeMap();
    }


    void Start()
    {
        // 3. 画面への描画
        VisualizeMap();
    }

    public void InitializeMap()
    {
        (MapCells,RootNode) = MapGenerator.GenerateMap(dungeonSettings);
        BspVisualizer.Instance.RegisterRootNode(RootNode);
    }


    /// <summary>
    /// TileMapに二次元配列のマップを反映する
    /// </summary>
    void VisualizeMap()
    {

        if (MapTile == null)
        {
            Debug.LogError("[MapManager] MapTile (Tilemap) がアサインされていません！");
            return;
        }

        if (MapCells == null)
        {
            Debug.LogError("[MapManager] MapCells が初期化されていません！");
            return;
        }
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                MapCell cell = MapCells[x, y];
                if (cell == null) continue;

                if (cell.Terrain != null && cell.Terrain.tile != null)
                {
                    Vector3Int tilePosition = new(x, y, 0);
                    MapTile.SetTile(tilePosition, cell.Terrain.tile);
                }
            }
        }
    }
}
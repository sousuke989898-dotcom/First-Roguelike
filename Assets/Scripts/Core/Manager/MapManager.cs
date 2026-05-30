using System.Collections.Generic;
using Game.GridMap;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Tilemap Tilemap;
    [Header("Cell")]
    [SerializeField] private Tile WallPrefab;
    [SerializeField] private Tile FloorPrefab;

    [SerializeField] private Tile WallHighLightPrefab;
    [SerializeField] private Tile FloorHighLightPrefab;

    [Header("InitialSize")]

    [SerializeField] private int InitSizeX;
    [SerializeField] private int InitSizeY;

    [SerializeField] private int MaxRoomCount;
    [SerializeField] private int minSize;

    public DungeonLevel CurrentLevel {get; private set;}
    // public int SizeX => CurrentLevel.Terrain.Width;
    // public int SizeY => CurrentLevel.Terrain.Height;


    public MapData MapData {get; private set;}

    public int SizeX => MapData.Map.GetLength(0);
    public int SizeY => MapData.Map.GetLength(1);

    public static MapManager Instance {get; private set;}
    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            enabled = false;
            Debug.LogError($"{this}が複数存在しています。");
        }
        MapData = new();
        InitializeMap(InitSizeX,InitSizeY,MaxRoomCount,minSize);
    }

    void Start()
    {
        VisualizeMap();
    }


    /// <summary>
    /// TileMapに二次元配列のマップを反映する
    /// </summary>
    void VisualizeMap()
    {
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                Tile tile = null;
                switch (MapData.GetTileType(new(x,y)))
                {
                    case TileType.Wall:
                        tile = WallPrefab;
                        break;
                    case TileType.Floor:
                        tile = FloorPrefab;
                        break;
                    case TileType.Door:
                        tile = FloorPrefab;
                        break;
                    case TileType.Road:
                        tile = FloorPrefab;//仮
                        break;
                }

                Vector3Int position = new(x,y,0);
                Tilemap.SetTile(position,tile);
            }
        }
    }


    public void InitializeMap(int sizeX, int sizeY, int maxRoomCount, int minSize)
    {
        TileType[,] terrain = MapGenerator.GenerateMap(sizeX,sizeY,maxRoomCount,minSize);
        MapData.InitMapData(terrain);

        //Entity生成
    }

    public Vector2Int GetSpawnPos()
    {
        List<Vector2Int> positions = MapData.GetCanSpawnPositions();
        return positions[Random.Range(0, positions.Count - 1)];
    }

    // public Vector2Int GetSpawnPos()
    // {
    //     // スポーン可能座標の取得ロジック
    //     List<Vector2Int> emptyPositions = new();

    //     for (int x = 0; x < SizeX; x++)
    //     {
    //         for (int y = 0; y < SizeY; y++)
    //         {
    //             Vector2Int pos = new(x, y);
    //             // 地形がスポーン可能（床）かつ、Entityが何も居ない場所
    //             if (CurrentLevel.Terrain.GetTileType(pos) == TileType.Floor && 
    //                 CurrentLevel.Holder.GetEntities(pos).Count == 0)
    //             {
    //                 emptyPositions.Add(pos);
    //             }
    //         }
    //     }

    //     if (emptyPositions.Count == 0) return Vector2Int.zero;
    //     return emptyPositions[Random.Range(0, emptyPositions.Count)];
    // }

    // public void InitializeMap(int sizeX, int sizeY, int maxRoomCount, int minSize)
    // {
    //     TileType[,] terrain = MapGenerator.GenerateMap(sizeX, sizeY, maxRoomCount, minSize);
        
    //     List<Section> sections = new List<Section>(); 

    //     // 3. 統括クラスの生成
    //     CurrentLevel = new DungeonLevel(terrain, sections);
    // }
}
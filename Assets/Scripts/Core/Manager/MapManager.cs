using System.Collections.Generic;
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


    public MapData Data {get; private set;}

    public int SizeX => Data.Map.GetLength(0);
    public int SizeY => Data.Map.GetLength(1);

    public static MapManager Instance {get; private set;}
    void Awake()
    {
        if (Instance != null) Instance = this;
        else
        {
            enabled = false;
            Debug.LogError($"{this}が複数存在しています。");
        }
        Data = new();
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
                switch (Data.GetTileType(new(x,y)))
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
        Data.Map = terrain;

        //Entity生成
    }

    public Vector2Int GetSpawnPos()
    {
        List<Vector2Int> positions = Data.GetCanSpawnPositions();
        return positions[Random.Range(0, positions.Count - 1)];
    }
}
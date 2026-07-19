using System.Collections.Generic;
using Game.GridMap;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{

    [Header("描画先・配置先")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Transform entityParent; // 生成したドアなどのGameObjectをまとめる親

    [Header("データアセット")]
    [SerializeField] private DungeonSettings dungeonSettings;
    [SerializeField] private EntitySpawnMapping entitySpawnMapping;

    public DungeonLevel CurrentLevel {get; private set;}
    // public int SizeX => CurrentLevel.Terrain.Width;
    // public int SizeY => CurrentLevel.Terrain.Height;

    // public RoomRegistry _roomRegistry { get; private set; }

    public int SizeX => CurrentLevel.Terrain.Matrix.GetLength(0);
    public int SizeY => CurrentLevel.Terrain.Matrix.GetLength(1);

    public MapData MapData; //todo 削除


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

        // if (dungeonSettings == null || dungeonSettings.roomDataset == null || dungeonSettings.tileMapping == null)
        // {
        //     Debug.LogError("DungeonSettings、または内部のアセットがセットされていません。");
        //     return;
        // }

        // _roomRegistry = new RoomRegistry(dungeonSettings.roomDataset, dungeonSettings.tileMapping);

        InitializeMap();
    }


    void Start()
    {
        // 3. 画面への描画
        VisualizeMap();

        // 4. ドアなどの動的オブジェクト（Entity）の生成
        //SpawnEntities();
    }

    public void InitializeMap()
    {
        // ※ MapGenerator側も今後、dungeonSettings や _roomRegistry を受け取る形に修正していきます
        // TileType[,] terrain = MapGenerator.GenerateMap(dungeonSettings, _roomRegistry);
        // List<Section> sections = MapGenerator.LastGeneratedSections;
        
        // 仮のダミーデータ（コンパイルを通すためのプレースホルダー）
        TileType[,] terrain = new TileType[dungeonSettings.mapSizeX, dungeonSettings.mapSizeY];
        List<Section> sections = new List<Section>();

        // 新しい統括クラス（DungeonLevel）のインスタンスを作成
        CurrentLevel = new DungeonLevel(terrain, sections);
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
                // TileType type = CurrentLevel.GetTileType(new(x,y));
                
                // TileBase tile = dungeonSettings.tileMapping.GetTileBase(type);

                //     // ※ドア（Door_Closedなど）は、見た目上「床」として描画したい場合はここで補正
                //     if (type == TileType.Door_Closed)
                //     {
                //         tile = dungeonSettings.tileMapping.GetTileBase(TileType.Floor);
                //     }

                //     Vector3Int position = new(x, y, 0);
                //     tilemap.SetTile(position, tile);
            }
        }
    }


    public void InitializeMap(int sizeX, int sizeY, int maxRoomCount, int minSize)
    {
        //TileType[,] terrain = MapGenerator.GenerateMap(sizeX,sizeY,maxRoomCount,minSize);
        //MapData.InitMapData(terrain);

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
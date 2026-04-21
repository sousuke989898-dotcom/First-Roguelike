// using UnityEngine;
// using UnityEngine.Tilemaps;


// /// <summary>
// /// マップの情報を持つ
// /// </summary>
// public class MapManager : MonoBehaviour
// {
//     [SerializeField] private Tilemap Tilemap;
//     [Header("Cell")]
//     [SerializeField] private Tile WallPrefab;
//     [SerializeField] private Tile FloorPrefab;

//     [Header("InitialSize")]

//     [SerializeField] private int InitSizeX;
//     [SerializeField] private int InitSizeY;

//     [SerializeField] private int MaxRoomCount;
//     [SerializeField] private int minSize;

//     public TileType[,] Map {get; private set;}

//     public MapData MapData {get; private set;}

//     public int SizeX => Map.GetLength(0);
//     public int SizeY => Map.GetLength(1);


//     public static MapManager Instance {get; private set;}
//     void Awake()
//     {
//         Instance = this;
//         InitializeMap(InitSizeX,InitSizeY,MaxRoomCount,minSize);

//         //Awakeの中なので、UnitManager.Instanceがまだ使えない
//         // gameObject.GetComponent<UnitManager>().InitializeUnitsMap(InitSizeX,InitSizeY);
//     }



//     void Start()
//     {
//         VisualizeMap();
//     }


//     /// <summary>
//     /// TileMapに二次元配列のマップを反映する
//     /// </summary>
//     void VisualizeMap()
//     {
//         for (int x = 0; x < SizeX; x++)
//         {
//             for (int y = 0; y < SizeY; y++)
//             {
//                 Tile tile = (Map[x,y] == TileType.Wall) ? WallPrefab : FloorPrefab;
//                 switch (Map[x, y])
//                 {
//                     case TileType.Wall:
//                         tile = WallPrefab;
//                         break;
//                     case TileType.Floor:
//                         tile = FloorPrefab;
//                         break;
//                     case TileType.Door:
//                         tile = FloorPrefab;
//                         break;
//                     case TileType.Road:
//                         tile = FloorPrefab;//仮
//                         break;
//                 }

//                 Vector3Int position = new(x,y,0);
//                 Tilemap.SetTile(position,tile);
//             }
//         }
//     }


//     /// <summary>
//     /// 指定の座標がマップ内かを取得する
//     /// </summary>
//     /// <param name="vector">絶対座標</param>
//     /// <returns>ture = マップ内, false = マップ外</returns>
//     public bool IsInsideMap(Vector2Int vector)
//     {
//         return vector.x >= 0 && vector.x < SizeX 
//             && vector.y >= 0 && vector.y < SizeY;
//     }

//     /// <summary>
//     /// 指定のx,y座標がマップ内かを取得する
//     /// </summary>
//     /// <param name="x">指定先のX座標</param>
//     /// <param name="y">指定先のY座標</param>
//     public bool IsInsideMap(int x, int y) => IsInsideMap(new Vector2Int(x,y));

//     /// <summary>
//     /// 指定の座標が床かを取得する
//     /// </summary>
//     /// <param name="vector">絶対座標</param>
//     /// <returns>ture = 床, false = 床以外</returns>
//     public bool IsFloor(Vector2Int vector)
//     {
//         if (!IsInsideMap(vector.x,vector.y)) return false;
//         TileType tile = Map[vector.x,vector.y];
//         if (tile == TileType.Wall || tile == TileType.None || tile == TileType.Door)
//         {
//             return false;
//         }
//         else
//         {
//             return true;
//         }
//     }

//     /// <summary>
//     /// 指定のx,y座標が床かを取得する
//     /// </summary>
//     /// <param name="x">指定先のX座標</param>
//     /// <param name="y">指定先のY座標</param>
//     /// <returns>ture = 床, false = 床以外</returns>
//     public bool IsFloor(int x, int y) => IsFloor(new Vector2Int(x,y));

//     /// <summary>
//     /// 指定の座標のTileTypeを取得する
//     /// </summary>
//     /// <param name="vector">絶対座標</param>
//     public TileType GetTileType(Vector2Int vector)
//     {
//         if (!IsInsideMap(vector)) return TileType.None;
//         return Map[vector.x, vector.y];
//     }

//     /// <summary>
//     /// 指定の座標のTileTypeを取得する
//     /// </summary>
//     /// <param name="x">絶対座標の</param>
//     /// <param name="y"></param>
//     public TileType GetTileType(int x, int y) => GetTileType(new Vector2Int(x,y));

//     /// <summary>
//     /// マップを床と外枠のみの状態にする
//     /// </summary>
//     public void InitializeMap(int sizeX, int sizeY)
//     {
//         if(sizeX < 5 || sizeY < 5)
//         {
//             Debug.LogError($"{gameObject} マップの生成に失敗しました。マップの大きさは縦横どちらも5以上である必要があります。 ");
//             return;
//         }

//         Map = new TileType[sizeX,sizeY];
//         for (int x = 0; x < sizeX; x++)
//         {
//             for (int y = 0; y < sizeY; y++)
//             {
//                 if(x == 0 || x == sizeX-1 || y == 0 || y == sizeY-1)
//                 {
//                     Map[x,y] = TileType.Wall;
//                 }
//                 else
//                 {
//                     Map[x,y] = TileType.Floor;
//                 }
//             }
//         }
//         return;
//     }

//     public void InitializeMap(int sizeX, int sizeY, int maxRoomCount, int minSize)
//     {
//         Map = MapGenerator.GenerateMap(sizeX,sizeY,maxRoomCount,minSize);
//     }

//     public Vector2Int GetSpownPos()
//     {
//         Vector2Int pos;
//         int timeout = 100;
//         do
//         {
//             pos = new Vector2Int(
//                 RandomRange.GetRandomValue(1, SizeX - 2),
//                 RandomRange.GetRandomValue(1, SizeY - 2)
//             );
//             timeout--;
//         }
//         while ((!IsFloor(pos) || (pos == GameManager.Instance.CurrentPlayer.Pos)) && timeout > 0);
//         return pos;
//     }


// }

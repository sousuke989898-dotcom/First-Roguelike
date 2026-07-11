using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GridMap
{
    public static class MapGenerator
    {


        /// <summary>
        /// マップを生成する
        /// </summary>
        /// <param name="sizeX">横幅</param>
        /// <param name="sizeY">縦幅</param>
        /// <param name="roomCount">部屋の数</param>
        /// <param name="sectionSide">Sectionの一辺の最小サイズ</param>
        /// <returns></returns>
        // public static TileType[,] GenerateMap(int sizeX,int sizeY, int roomCount, int sectionSide)
        // {
        //     if (sizeX <= sectionSide * 2 || sizeY <= sectionSide * 2 || sizeX*sizeY <= sectionSide * sectionSide)
        //     {
        //         Debug.LogError("MapGenerater GenerateMap : マップサイズがSectionのの最小サイズよりも小さいです");
        //         return null;
        //     }

        //     TileType[,] map = new TileType[sizeX, sizeY];

        //     Section parentSection = new(0,0,sizeX, sizeY);
        //     map = FillRect(map, parentSection.SectionRect, TileType.Wall); //マップ全体を壁で埋める

        //     List<Section> sections = GenerateSections(parentSection,roomCount,sectionSide);

        //     foreach (Section section in sections)
        //     {
        //         GenerateRoom(section);
        //     }

        //     List<Vector2Int[]> paths = GetRoadPath(sections);

        //     foreach (Section section in sections)
        //     {
        //         map = FillRect(map, section.RoomRect, TileType.Floor); //部屋を床で埋める

        //         foreach (var dir in DirectionTool.baseDirs)
        //         {
        //             (Vector2Int? door, Vector2Int? end) = dir switch
        //             {
        //                 Direction.Left  => (section.leftDoor,  section.LeftRoadEnd),
        //                 Direction.Right => (section.rightDoor, section.RightRoadEnd),
        //                 Direction.Down  => (section.downDoor,  section.DownRoadEnd),
        //                 Direction.Upper => (section.upperDoor, section.UpperRoadEnd),
        //                 _ => (null, null)
        //             };

        //             if (door.HasValue && end.HasValue)
        //             {
        //                 map = DrawLine(map, door.Value, end.Value, TileType.Road); //ドアから一方向を道で埋める
        //             }
        //         }
        //     }

        //     foreach (Vector2Int[] path in paths)
        //     {
        //         map = DrawLine(map, path[0], path[1], TileType.Road); //合流部分を道で埋める
        //     }

        //     return map;
        // }

        static public DungeonLevel GenerateMap(DungeonSettings settings)
        {
            Section parentSection = new(0,0,settings.mapSizeX, settings.mapSizeY);

            TileType[,] globalTerrain = new TileType[settings.mapSizeX, settings.mapSizeY];
            TileType[,] globalEntities = new TileType[settings.mapSizeX, settings.mapSizeY];
            FillRect(globalTerrain, parentSection.SectionRect, TileType.Wall);
            FillRect(globalEntities, parentSection.SectionRect, TileType.None);

            List <Section> sections = GenerateSections(parentSection, settings); //親セクションを指定された数に分割する

            foreach (var section in sections)
            {
                //Room room = registry.GetRandomRoom(RoomCategory.Basic, section.Size);
                // if (room != null)
                // {
                //     // ★ここで「部屋の2層配列」を、全体の「2層配列」にそれぞれスタンプ（コピー）する！
                //     CopyRoomToGlobal(room, section.Position, globalTerrain, globalEntities);
                // }
            }

            return null;
        }



        // /// <summary>
        // /// Sectionsの道を取得し、それぞれのSectionのドアと道が存在するかを整理する
        // /// </summary>
        // /// <param name="sections">それぞれが1マスずつ重なり合ったもの</param>
        // /// <returns></returns>
        // private static List<Vector2Int[]> GetRoadPath(List<Section> sections)
        // {
        //     List<Vector2Int[]> vectors = new();

        //     for (int i = 0; i < sections.Count; i++)
        //     {
        //         for (int j = i + 1; j < sections.Count; j++) //同じものを比べない用の j = i + 1;
        //         {
        //             Section sI = sections[i];
        //             Section sJ = sections[j];

        //             foreach (var dir in DirectionTool.baseDirs)
        //             {
        //                 Vector2Int? eI = sI.GetRoadEnd(dir);
        //                 Vector2Int? eJ = sJ.GetRoadEnd(DirectionExtensions.GetOpposite(dir));

        //                 if (eI != null && eJ != null && sJ.SectionRect.Contains(eI.Value))
        //                 {
        //                     vectors.Add(new Vector2Int[2] {eI.Value,eJ.Value});

        //                     sI.activeDoors |= dir;
        //                     sJ.activeDoors |= DirectionExtensions.GetOpposite(dir);
        //                 }
        //             }
        //         }
        //     }

        //     for (int i = 0; i < sections.Count; i++)
        //     {
        //         sections[i].ClearUnusedDoors();
        //     }

        //     return vectors;
        // }

        /// <summary>
        /// BSPの分割結果（Section）だけを計算してリストで返す実験用メソッド
        /// </summary>
        public static List<Section> DebugGenerateSections(DungeonSettings settings)
        {
            Section parentSection = new(0, 0, settings.mapSizeY, settings.mapSizeY);
            
            // 空間分割ロジックだけを実行する
            return GenerateSections(parentSection, settings);
        }



        /// <summary>
        /// 引数として受け取ったセクションを指定された数に分割する
        /// </summary>
        /// <param name="parentSection">元のSection</param>
        /// <param name="count">分割数</param>
        /// <param name="sectionSize">一辺の最小サイズ</param>
        /// <returns></returns>
        private static List<Section> GenerateSections(Section parentSection, DungeonSettings settings)
        {
            if (settings.roomCount <= 0)
            {
                Debug.LogError("MapGenerator GenerateSections : 0個以下の部屋は生成できません");
                return new List<Section> { parentSection };
            }

            List<Section> sections = new() { parentSection };
            List<Section> finalSections = new();

            while(sections.Count + finalSections.Count < settings.roomCount && sections.Count > 0)
            {
                sections.Sort((a, b) => b.Size.CompareTo(a.Size));

                Section section = sections[0];
                sections.RemoveAt(0);

                var result = DivideSection(section, settings);

                if (result != null)
                {
                    sections.AddRange(result);
                }
                else
                {
                    finalSections.Add(section);
                }
            }

            sections.AddRange(finalSections);

            if (sections.Count < settings.roomCount)
            {
                Debug.LogWarning($"MapGenerator : MapSizeが足りません。 指定された部屋数:{settings.roomCount}個 現在の部屋数:{sections.Count}個");
            }

            // TODO: 本来は GenerateMap などの上位メソッドに移動させる
            if (BspVisualizer.Instance != null)
            {
                BspVisualizer.Instance.RegisterSections(sections);
            }
            return sections;
        }


        /// <summary>
        /// Sectionの長い方辺を分断して二つのSectionにする
        /// </summary>
        /// <param name="s">元のSection</param>
        /// <param name="sectionSize">Sectionの最小サイズ</param>
        /// <returns>二つに分けられたSection</returns>
        private static Section[] DivideSection(Section s, DungeonSettings settings)
        {
            int minSize = settings.minSectionSize;
    
            // 縦・横どちらも最短の倍以下なら終了
            if (s.Width < minSize * 2 && s.Height < minSize * 2) return null;

            bool divideHorizontally;

            if (s.Width < minSize * 2)       divideHorizontally = false;
            else if (s.Height < minSize * 2) divideHorizontally = true;
            else                             divideHorizontally = s.Width >= s.Height;

            (int min, int max) CalculatePartingRange(int totalLength)
            {
                float minRate = settings.partingMargin;
                float maxRate = 1.0f - settings.partingMargin;

                int minPart = Mathf.Max(minSize, Mathf.RoundToInt(totalLength * minRate));
                int maxPart = Mathf.Min(totalLength - minSize, Mathf.RoundToInt(totalLength * maxRate));
                return (minPart, maxPart);
            }

            int overlapOffset = settings.useOverlap ? 1 : 0;

            if (divideHorizontally)
            {
                var (minParting, maxParting) = CalculatePartingRange(s.Width);
                if (minParting >= maxParting) return null;

                int parting = Random.Range(minParting, maxParting);

                return new Section[2] {
                    new(s.X, s.Y, parting, s.Height),
                    new(s.X + parting - overlapOffset, s.Y, s.Width - parting + overlapOffset, s.Height)
                };
            }
            else
            {
                var (minParting, maxParting) = CalculatePartingRange(s.Height);
                if (minParting >= maxParting) return null;

                int parting = Random.Range(minParting, maxParting);

                return new Section[2] {
                    new(s.X, s.Y, s.Width, parting),
                    new(s.X, s.Y + parting - overlapOffset, s.Width, s.Height - parting + overlapOffset)
                };
            }

        }

        // private const int DistanceFromSectionEnd = 2; //部屋が隣り合ってつながらないように
        // private const int DoorPosOffset = 1; //角にドアができないように
        // /// <summary>
        // /// typeがnullならランダムな部屋を生成し、そうでないならtypeに基づいた部屋情報を取得する
        // /// </summary>
        // /// <param name="type">元のsection</param>
        // public static void GenerateRoom(Section s)
        // {

        //         int w = s.Width /2; //ここはもう少しランダム性を追加してもいいかも?
        //         int h = s.Height/2;
        //         int x = s.X + Random.Range(DistanceFromSectionEnd, w - DistanceFromSectionEnd); //生成したRectIntが元のSectionの中心を含むための処理
        //         int y = s.Y + Random.Range(DistanceFromSectionEnd, h - DistanceFromSectionEnd);
        //         s.RoomRect = new RectInt(x,y,w,h);

        //         s.leftDoor  = new(x    , y + Random.Range(DoorPosOffset, h - DoorPosOffset));
        //         s.rightDoor = new(x + w, y + Random.Range(DoorPosOffset, h - DoorPosOffset));
        //         s.downDoor  = new(x + Random.Range(DoorPosOffset, w - DoorPosOffset), y);
        //         s.upperDoor = new(x + Random.Range(DoorPosOffset, w - DoorPosOffset), y + h);

        //         //本当は部屋生成を完全ランダムにせずに、プリセットを用意してもいいかも?
        //         //部屋にランダムで装飾を置くとしたら大変そう
        // }

        private static void StampRoom()
        {
            
        }


        /// <summary>
        /// 引数として受け取ったTileType[,]を塗りつぶす
        /// </summary>
        /// <param name="map">描画先</param>
        /// <param name="rect">範囲</param>
        private static TileType[,] FillRect(TileType[,] map, RectInt rect, TileType tile)
        {
            if (rect.xMin < 0 || map.GetLength(0) < rect.xMax || rect.yMin < 0 || map.GetLength(1) < rect.yMax)
            {
                Debug.LogError("MapGenerater FillRect : 範囲外を描画しようとしています");
                return map;
            }
            for (int x = rect.xMin; x < rect.xMax; x++)
            {
                for (int y = rect.yMin; y < rect.yMax; y++)
                {
                    map[x,y] = tile;
                }
            }
            return map;
        }

        /// <summary>
        /// 引数として受け取ったTileTyep[,]に線を描画する
        /// </summary>
        /// <param name="map">描画先</param>
        /// <param name="start">始点</param>
        /// <param name="end">終点</param>
        private static TileType[,] DrawLine(TileType[,] map, Vector2Int start,Vector2Int end, TileType tile)
        {
            int minX = Mathf.Min(start.x, end.x);
            int maxX = Mathf.Max(start.x, end.x);
            int minY = Mathf.Min(start.y, end.y);
            int maxY = Mathf.Max(start.y, end.y);

            if (minX < 0 || map.GetLength(0) < maxX || minY < 0 || map.GetLength(1) < maxY)
            {
                Debug.LogError("MapGenerater DrawLine : 範囲外を描画しようとしています");
                return map;
            }
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    map[x, y] = tile;
                }
            }
            return map;
        }


    }
}

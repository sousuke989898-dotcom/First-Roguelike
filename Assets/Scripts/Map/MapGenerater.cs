using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public static TileType[,] GenerateMap(int sizeX,int sizeY, int roomCount, int sectionSide)
    {
        if (sizeX <= sectionSide * 2 || sizeY <= sectionSide * 2 || sizeX*sizeY <= sectionSide * sectionSide)
        {
            Debug.LogError("MapGenerater GenerateMap : マップサイズがSectionのの最小サイズよりも小さいです");
            return null;
        }

        TileType[,] map = new TileType[sizeX, sizeY];

        Section parentSection = new(0,0,sizeX, sizeY);
        map = FillRect(map, parentSection.rect, TileType.Wall); //マップ全体を壁で埋める

        List<Section> sections = GenerateSections(parentSection,roomCount,sectionSide);

        foreach (Section section in sections)
        {
            GenerateRoom(section);
        }

        List<Vector2Int[]> paths = GetRoadPath(sections);

        foreach (Section section in sections)
        {
            map = FillRect(map, section.roomRect, TileType.Floor); //部屋を床で埋める

            foreach (var dir in DirectionTool.baseDirs)
            {
                (Vector2Int? door, Vector2Int? end) = dir switch
                {
                    Direction.Left  => (section.leftDoor,  section.LeftRoadEnd),
                    Direction.Right => (section.rightDoor, section.RightRoadEnd),
                    Direction.Down  => (section.downDoor,  section.DownRoadEnd),
                    Direction.Upper => (section.upperDoor, section.UpperRoadEnd),
                    _ => (null, null)
                };

                if (door.HasValue && end.HasValue)
                {
                    map = DrawLine(map, door.Value, end.Value, TileType.Road); //ドアから一方向を道で埋める
                }
            }
        }

        foreach (Vector2Int[] path in paths)
        {
            map = DrawLine(map, path[0], path[1], TileType.Road); //合流部分を道で埋める
        }

        return map;
    }



    /// <summary>
    /// Sectionsの道を取得し、それぞれのSectionのドアと道が存在するかを整理する
    /// </summary>
    /// <param name="sections">それぞれが1マスずつ重なり合ったもの</param>
    /// <returns></returns>
    private static List<Vector2Int[]> GetRoadPath(List<Section> sections)
    {
        List<Vector2Int[]> vectors = new();

        for (int i = 0; i < sections.Count; i++)
        {
            for (int j = i + 1; j < sections.Count; j++) //同じものを比べない用の j = i + 1;
            {
                Section sI = sections[i];
                Section sJ = sections[j];

                foreach (var dir in DirectionTool.baseDirs)
                {
                    Vector2Int? eI = sI.GetRoadEnd(dir);
                    Vector2Int? eJ = sJ.GetRoadEnd(DirectionExtensions.GetOpposite(dir));

                    if (eI != null && eJ != null && sJ.rect.Contains(eI.Value))
                    {
                        vectors.Add(new Vector2Int[2] {eI.Value,eJ.Value});

                        sI.activeDoors |= dir;
                        sJ.activeDoors |= DirectionExtensions.GetOpposite(dir);
                    }
                }
            }
        }

        for (int i = 0; i < sections.Count; i++)
        {
            sections[i].ClearUnusedDoors();
        }

        return vectors;
    }



    /// <summary>
    /// 引数として受け取ったセクションを指定された数に分割する
    /// </summary>
    /// <param name="parentSection">元のSection</param>
    /// <param name="count">分割数</param>
    /// <param name="sectionSize">一辺の最小サイズ</param>
    /// <returns></returns>
    private static List<Section> GenerateSections(Section parentSection, int roomNumber, int sectionSize)
    {
        List<Section> sections = new(){parentSection};
        List<Section> finalSections = new(); //これ以上分割できないsection

        if (roomNumber <= 0)
        {
            Debug.LogError("MapGenerater GenerateSections : 0個以下の部屋は生成できません");
            return sections;
        }

        while(sections.Count + finalSections.Count < roomNumber && sections.Count > 0)
        {
            sections = sections.OrderBy(s => -s.SectionArea).ToList();
            Section section = sections[0];
            sections.RemoveAt(0);

            var result = DivideSection(section,sectionSize);

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
        return sections;
    }


    public const int overlapOffset = 1; //DivideSectionで分けた部屋を重ねる
    /// <summary>
    /// Sectionの長い方辺を分断して二つのSectionにする
    /// </summary>
    /// <param name="s">元のSection</param>
    /// <param name="sectionSize">Sectionの最小サイズ</param>
    /// <returns></returns>
    private static Section[] DivideSection(Section s, int sectionSize)
    {
        
        if (s.Width <= sectionSize * 2 || s.Height <= sectionSize * 2 || s.SectionArea <= sectionSize * sectionSize) return null;
        if (s.Width >= s.Height)
        {
            int parting = Random.Range(sectionSize, s.Width - sectionSize);
            Section s1 = new(s.X, s.Y, parting, s.Height);
            Section s2 = new(s.X + parting - overlapOffset, s.Y, s.Width-parting + overlapOffset, s.Height);
            return new Section[2]{s1,s2};
        }
        else
        {
            int parting = Random.Range(sectionSize, s.Height - sectionSize);
            Section s1 = new(s.X, s.Y, s.Width, parting);
            Section s2 = new(s.X, s.Y + parting - overlapOffset, s.Width,s.Height - parting + overlapOffset);
            return new Section[2]{s1,s2};
        }
    }

    private const int DistanceFromSectionEnd = 2; //部屋が隣り合ってつながらないように
    private const int DoorPosOffset = 1; //角にドアができないように
    /// <summary>
    /// typeがnullならランダムな部屋を生成し、そうでないならtypeに基づいた部屋情報を取得する
    /// </summary>
    /// <param name="type">元のsection</param>
    public static void GenerateRoom(Section s)
    {

            int w = s.Width /2; //ここはもう少しランダム性を追加してもいいかも?
            int h = s.Height/2;
            int x = s.X + Random.Range(DistanceFromSectionEnd, w - DistanceFromSectionEnd); //生成したRectIntが元のSectionの中心を含むための処理
            int y = s.Y + Random.Range(DistanceFromSectionEnd, h - DistanceFromSectionEnd);
            s.roomRect = new RectInt(x,y,w,h);

            s.leftDoor  = new(x    , y + Random.Range(DoorPosOffset, h - DoorPosOffset));
            s.rightDoor = new(x + w, y + Random.Range(DoorPosOffset, h - DoorPosOffset));
            s.downDoor  = new(x + Random.Range(DoorPosOffset, w - DoorPosOffset), y);
            s.upperDoor = new(x + Random.Range(DoorPosOffset, w - DoorPosOffset), y + h);

            //本当は部屋生成を完全ランダムにせずに、プリセットを用意してもいいかも?
            //部屋にランダムで装飾を置くとしたら大変そう
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
    public static TileType[,] DrawLine(TileType[,] map, Vector2Int start,Vector2Int end, TileType tile)
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
using System.Collections.Generic;
using UnityEngine;

namespace Game.GridMap
{
    public static class MapGenerator
    {

        static public (MapCell[,],SectionNode) GenerateMap(DungeonSettings settings)
        {
            if (settings == null || settings.mapDatabase == null)
            {
                Debug.LogError("[MapGenerator] DungeonSettings または MapDatabase がセットされていません！");
                return (null,null);
            }

            int width = settings.mapSizeX;
            int height = settings.mapSizeY;
            MapCell[,] map = new MapCell[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map[x, y] = new MapCell();
                }
            }

            RectInt mapRect = new(0, 0, width, height);
            FillRect(map, mapRect, settings.mapDatabase.wallData);

            SectionNode rootNode = GenerateBSPTree(mapRect, settings);

            PopulateRooms(rootNode, map, settings);

            ConnectRooms(rootNode, map, settings);

            CleanupUnusedDoors(rootNode, map, settings);


            return (map, rootNode);
        }

        public static SectionNode GenerateBSPTree(RectInt mapRect, DungeonSettings settings)
        {
            SectionNode rootNode = new(mapRect);
            List<SectionNode> leafNodes = new() { rootNode };

            HashSet<SectionNode> failedNodes = new();

            int safetyCounter = 0;
            int maxIterations = 1000;

            // 目標の roomCount に達するまでループ
            while (leafNodes.Count < settings.roomCount && safetyCounter < maxIterations)
            {
                // 分割可能（最小サイズを満たす）なノードの中から、最も面積の大きいノードを選定
                SectionNode targetToSplit = null;
                int maxArea = -1;


                foreach (var leaf in leafNodes)
                {

                    if (!failedNodes.Contains(leaf) && CanSplit(leaf, settings))
                    {
                        int area = leaf.area.width * leaf.area.height;
                        if (area > maxArea)
                        {
                            maxArea = area;
                            targetToSplit = leaf;
                        }
                    }
                }

                // 分割可能な区画がこれ以上存在しない場合は終了
                if (targetToSplit == null)
                {
                    if (leafNodes.Count < settings.roomCount)
                    {
                        Debug.LogWarning($"[MapGenerator] マップサイズ不足のため指定数の部屋を生成できませんでした。(指定: {settings.roomCount}, 生成: {leafNodes.Count})");
                    }
                    break;
                }

                // ノードを2分割して子ノードを作成し、末端リスト（leafNodes）を更新
                if (SplitNode(targetToSplit, settings))
                {
                    leafNodes.Remove(targetToSplit);
                    leafNodes.Add(targetToSplit.left);
                    leafNodes.Add(targetToSplit.right);
                }
                else
                {
                    failedNodes.Add(targetToSplit);
                }
            }

            if (safetyCounter >= maxIterations)
            {
                Debug.LogWarning("[MapGenerator] 無限ループ防止のため処理を安全に中断しました。");
            }

            return rootNode;
        }

        /// <summary>
        /// ノードが分割可能か（最小セクションサイズを維持できるか）判定
        /// </summary>
        private static bool CanSplit(SectionNode node, DungeonSettings settings)
        {
            int minSize = settings.minSectionSize;
            return node.area.width >= minSize * 2 || node.area.height >= minSize * 2;
        }

        /// <summary>
        /// 指定ノードを left / right の子ノードに2分割する
        /// </summary>
        private static bool SplitNode(SectionNode node, DungeonSettings settings)
        {
            int minSize = settings.minSectionSize;
            SplitType splitType = DecideSplitDirection(node.area, minSize);

            int targetLength = (splitType == SplitType.Vertical) ? node.area.width : node.area.height;
            if (targetLength < minSize * 2) return false;

            var (minParting, maxParting) = CalculatePartingRange(targetLength, minSize, settings.partingMargin);
            if (minParting >= maxParting) return false;

            node.splitType = splitType;
            int parting = Random.Range(minParting, maxParting);
            int overlapOffset = settings.useOverlap ? 1 : 0;

            RectInt leftArea, rightArea;

            if (splitType == SplitType.Vertical) // 左右分割（X軸切断）
            {
                leftArea  = new RectInt(node.area.x, node.area.y, parting, node.area.height);
                rightArea = new RectInt(node.area.x + parting - overlapOffset, node.area.y, node.area.width - parting + overlapOffset, node.area.height);
            }
            else // 上下分割（Y軸切断）
            {
                leftArea  = new RectInt(node.area.x, node.area.y, node.area.width, parting);
                rightArea = new RectInt(node.area.x, node.area.y + parting - overlapOffset, node.area.width, node.area.height - parting + overlapOffset);
            }

            node.left  = new SectionNode(leftArea);
            node.right = new SectionNode(rightArea);

            return true;
        }


        public static SplitType DecideSplitDirection(RectInt area, int minSize)
        {
            if (area.width > minSize * 1.75f && area.width >= area.height * 1.25f) 
                return SplitType.Vertical;
            if (area.height > minSize * 1.75f && area.height >= area.width * 1.25f) 
                return SplitType.Horizontal;
            return area.width >= area.height? SplitType.Vertical : SplitType.Horizontal;
        }

        /// <summary>
        /// 旧DivideSectionから抽出：マージン率を考慮した分割位置の（最小値, 最大値）を計算する
        /// </summary>
        private static (int min, int max) CalculatePartingRange(int totalLength, int minSize, float partingMargin)
        {
            float minRate = partingMargin;
            float maxRate = 1.0f - partingMargin;

            int minPart = Mathf.Max(minSize, Mathf.RoundToInt(totalLength * minRate));
            int maxPart = Mathf.Min(totalLength - minSize, Mathf.RoundToInt(totalLength * maxRate));

            return (minPart, maxPart);
        }

        /// <summary>
        /// ツリーの末端ノード（葉）を巡回し、部屋の決定・配置・ドア座標の記録を行う
        /// </summary>
        public static void PopulateRooms(
            SectionNode node, 
            MapCell[,] map, 
            DungeonSettings settings)
        {
            if (node == null) return;

            if (!node.IsLeaf)
            {
                PopulateRooms(node.left, map, settings);
                PopulateRooms(node.right, map, settings);
                return;
            }

            // --- 末端ノードの処理 ---
            RoomData roomData = GetSuitableRoom(node.area, settings.roomDatabase, RoomCategory.Basic);
            if (roomData == null) return;

            // RoomDatabase から解析済みの Room キャッシュを取得
            Room room = settings.roomDatabase.GetParsedRoom(roomData, settings.mapDatabase);
            if (room == null) return;

            node.roomData = roomData;

            int maxOffsetX = node.area.width - roomData.size.x - 2; // 1マスの余白を保持
            int maxOffsetY = node.area.height - roomData.size.y - 2;

            int startX = node.area.x + 1 + (maxOffsetX > 0 ? Random.Range(0, maxOffsetX) : 0);
            int startY = node.area.y + 1 + (maxOffsetY > 0 ? Random.Range(0, maxOffsetY) : 0);

            // int startX = node.area.x + (node.area.width - roomData.size.x) / 2;
            // int startY = node.area.y + (node.area.height - roomData.size.y) / 2;

            node.roomRect = new RectInt(startX, startY, roomData.size.x, roomData.size.y);

            // 1. ドアの「ローカル座標」に startX, startY を足して「絶対座標」として Node に保存
            node.doorPositions.Clear();
            foreach (var (dir, localDoorPos) in room.LocalDoors)
            {
                Vector2Int worldDoorPos = new(startX + localDoorPos.x, startY + localDoorPos.y);
                node.doorPositions[dir] = worldDoorPos;
            }

            // 2. キャッシュデータから MapCell[,] に地形とギミックを焼き込む
            StampRoomFromParsedData(map, startX, startY, room);
        }


        private static RoomData GetSuitableRoom(RectInt rect, RoomDatabase roomDatabase, RoomCategory category)
        {
            if (roomDatabase == null) return null;

            List<RoomData> categoryRooms = roomDatabase.GetRoomsByCategory(category);

            // セクションのサイズ内に収まる部屋だけに絞り込む
            List<RoomData> suitableRooms = categoryRooms.FindAll(r => 
                r.size.x <= rect.width && r.size.y <= rect.height
            );

            if (suitableRooms.Count == 0)
            {
                Debug.LogWarning($"[MapGenerator] セクションサイズ ({rect.width}x{rect.height}) に収まる部屋プリセットが見つかりませんでした。");
                return null;
            }

            // 3. 候補の中からランダムに1つ選ぶ
            int randomIndex = Random.Range(0, suitableRooms.Count);
            return suitableRooms[randomIndex];
        }

        /// <summary>
        /// キャッシュ済み Room オブジェクトから地形とギミックを MapCell[,] に焼き込む
        /// </summary>
        private static void StampRoomFromParsedData(
            MapCell[,] map,
            int startX,
            int startY,
            Room room)
        {
            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);

            int roomWidth = room.roomData.size.x;
            int roomHeight = room.roomData.size.y;

            for (int x = 0; x < roomWidth; x++)
            {
                for (int y = 0; y < roomHeight; y++)
                {
                    int targetX = startX + x;
                    int targetY = startY + y;

                    if (targetX < 0 || targetX >= mapWidth || targetY < 0 || targetY >= mapHeight)
                        continue;

                    // Terrain（地形）の書き込み
                    MapObjectData terrainData = room.TerrainMap[x, y];
                    if (terrainData != null)
                    {
                        map[targetX, targetY].AssignObject(terrainData);
                    }

                    // Gimmick（ドア等）の書き込み
                    MapObjectData gimmickData = room.GimmickMap[x, y];
                    if (gimmickData != null)
                    {
                        map[targetX, targetY].AssignObject(gimmickData);
                    }
                }
            }
        }

        /// <summary>
        /// ノード（またはその子孫）から、指定された方角にあるドア座標をたどって取得する
        /// </summary>
        private static Vector2Int GetDoorFromSubtree(SectionNode node, Direction dir)
        {
            if (node == null) return Vector2Int.zero;

            // 末端（葉）なら、保持しているドア座標を返す
            if (node.IsLeaf)
            {
                if (node.doorPositions.TryGetValue(dir, out Vector2Int pos))
                    return pos;

                // 指定方角のドアがない場合は、部屋の中心座標で代替（フォールバック）
                return new Vector2Int(node.roomRect.xMin + node.roomRect.width / 2, 
                                    node.roomRect.yMin + node.roomRect.height / 2);
            }

            // 枝ノードの場合：方角に応じて探索する子ノードを選択
            // 例: Right（東）のドアが欲しいなら、より右側にある right ノードを優先探索する
            if (dir == Direction.Right || dir == Direction.Upper)
            {
                return GetDoorFromSubtree(node.right ?? node.left, dir);
            }
            else
            {
                return GetDoorFromSubtree(node.left ?? node.right, dir);
            }
        }

        public static void ConnectRooms(SectionNode node, MapCell[,] map, DungeonSettings settings)
        {
            if (node == null || node.IsLeaf) return;

            ConnectRooms(node.left, map, settings);
            ConnectRooms(node.right, map, settings);

            Direction dirA = (node.splitType == SplitType.Vertical) ? Direction.Right : Direction.Upper;
            Direction dirB = (node.splitType == SplitType.Vertical) ? Direction.Left : Direction.Down;

            List<Vector2Int> doorsA = GetAllDoorsFromSubtree(node.left, dirA);
            List<Vector2Int> doorsB = GetAllDoorsFromSubtree(node.right, dirB);

            // 適切なドアを持った部屋が存在しない場合は接続をスキップ（部屋貫通を防止）
            if (doorsA.Count == 0 || doorsB.Count == 0) return;

            (Vector2Int doorA, Vector2Int doorB) = FindClosestDoorPair(doorsA, doorsB);

            DrawCorridor(map, doorA, doorB, node.splitType, settings.mapDatabase.roadData);
        }

        /// <summary>
        /// 2つのドアを正しく対面方向から繋ぐ（Z字/N字接続）
        /// </summary>
        private static void DrawCorridor(
            MapCell[,] map, 
            Vector2Int start, 
            Vector2Int end, 
            SplitType splitType, 
            MapObjectData roadData)
        {
            
            if (splitType == SplitType.Vertical)
            {
                // 中間点を固定せず、ドアAとドアBの間でランダムに曲げる
                int minX = Mathf.Min(start.x, end.x);
                int maxX = Mathf.Max(start.x, end.x);
                int midX = (maxX - minX > 2) ? Random.Range(minX + 1, maxX) : minX;

                Vector2Int p1 = new(midX, start.y);
                Vector2Int p2 = new(midX, end.y);

                DrawLine(map, start, p1, roadData);
                DrawLine(map, p1, p2, roadData);
                DrawLine(map, p2, end, roadData);
            }
            else
            {
                // Y軸の中間地点で横に曲がる（縦 -> 横 -> 縦）
                int midY = (start.y + end.y) / 2;

                Vector2Int point1 = new Vector2Int(start.x, midY);
                Vector2Int point2 = new Vector2Int(end.x, midY);

                DrawLine(map, start, point1, roadData); // 北ドアから中間へ（縦）
                DrawLine(map, point1, point2, roadData); // 中間地点での横移動
                DrawLine(map, point2, end, roadData);   // 中間から南ドアへ（縦）
            }
        }

        /// <summary>
        /// サブツリー内の全リーフノードから、指定方角のドア座標をすべて収集する
        /// </summary>
        private static List<Vector2Int> GetAllDoorsFromSubtree(SectionNode node, Direction dir)
        {
            List<Vector2Int> doors = new();
            if (node == null) return doors;

            if (node.IsLeaf)
            {
                if (node.doorPositions.TryGetValue(dir, out Vector2Int pos))
                {
                    doors.Add(pos);
                }
                return doors;
            }

            doors.AddRange(GetAllDoorsFromSubtree(node.left, dir));
            doors.AddRange(GetAllDoorsFromSubtree(node.right, dir));
            return doors;
        }

        /// <summary>
        /// 2つのドアリストから最も距離が近いペアを見つける
        /// </summary>
        private static (Vector2Int bestA, Vector2Int bestB) FindClosestDoorPair(List<Vector2Int> doorsA, List<Vector2Int> doorsB)
        {
            Vector2Int bestA = doorsA[0];
            Vector2Int bestB = doorsB[0];
            int minDistance = int.MaxValue;

            foreach (var a in doorsA)
            {
                foreach (var b in doorsB)
                {
                    int dist = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        bestA = a;
                        bestB = b;
                    }
                }
            }

            return (bestA, bestB);
        }

        /// <summary>
        /// マップ全体の未使用ドア（道と繋がっていないドア）を検出し、壁タイルに置換する
        /// </summary>
        public static void CleanupUnusedDoors(SectionNode node, MapCell[,] map, DungeonSettings settings)
        {
            if (node == null) return;

            // 枝ノードの場合は子ノードへ再帰処理
            if (!node.IsLeaf)
            {
                CleanupUnusedDoors(node.left, map, settings);
                CleanupUnusedDoors(node.right, map, settings);
                return;
            }

            // リーフノード（部屋）の場合：各ドアの正面マスを検査
            foreach (var (dir, doorPos) in node.doorPositions)
            {
                Vector2Int frontPos = doorPos + GetDirectionOffset(dir);

                // 正面マスが「道」と繋がっていない場合は未使用ドアとみなす
                if (!IsRoadCell(frontPos, map, settings))
                {
                    // ドアが存在していたマスを壁データで上書き
                    map[doorPos.x, doorPos.y].AssignObject(settings.mapDatabase.wallData);
                }
            }
        }

        /// <summary>
        /// 方角に応じた1マス分のオフセット（移動量）を取得する
        /// </summary>
        private static Vector2Int GetDirectionOffset(Direction dir)
        {
            return dir switch
            {
                Direction.Upper => new Vector2Int(0, 1),
                Direction.Down  => new Vector2Int(0, -1),
                Direction.Left  => new Vector2Int(-1, 0),
                Direction.Right => new Vector2Int(1, 0),
                _ => Vector2Int.zero
            };
        }

        /// <summary>
        /// 指定座標がマップ内かつ「道タイル」が配置されているか判定する
        /// </summary>
        private static bool IsRoadCell(Vector2Int pos, MapCell[,] map, DungeonSettings settings)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            // 1. マップ範囲外チェック
            if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
            {
                return false;
            }

            MapCell cell = map[pos.x, pos.y];
            if (cell == null) return false;

            // 2. 正面マスが道（roadData）になっているかチェック
            // ※ MapCell 内で保持している Terrain や Gimmick と roadData の一致を確認します
            return cell.Terrain == settings.mapDatabase.roadData; 
        }



        /// <summary>
        /// 引数として受け取ったTileType[,]を塗りつぶす
        /// </summary>
        /// <param name="map">描画先</param>
        /// <param name="rect">範囲</param>
        private static void FillRect(MapCell[,] map, RectInt rect, MapObjectData data)
        {
            if (data == null) return ;

            for (int x = rect.xMin; x < rect.xMax; x++)
            {
                for (int y = rect.yMin; y < rect.yMax; y++)
                {
                    // すでにCellがnullなら生成してからAssignする
                    map[x, y] ??= new MapCell();
                    map[x, y].AssignObject(data);
                }
            }
        }

        /// <summary>
        /// 指定された2点間に線を引くように MapCell.Terrain に MapObjectData をセットする（通路用）
        /// </summary>
        private static void DrawLine(MapCell[,] map, Vector2Int start, Vector2Int end, MapObjectData data)
        {
            if (data == null) return;

            int minX = Mathf.Min(start.x, end.x);
            int maxX = Mathf.Max(start.x, end.x);
            int minY = Mathf.Min(start.y, end.y);
            int maxY = Mathf.Max(start.y, end.y);

            RectInt lineRect = new(minX, minY, maxX - minX + 1, maxY - minY + 1);
            FillRect(map, lineRect, data);
        }
    }
}


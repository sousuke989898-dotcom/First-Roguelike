using System.Collections.Generic;
using UnityEngine;

public enum InteractType {Move, Unit, Entity, Open, None}; //todo 要検討

public class MapData
{
    public TileType[,] Map;

    public int Width  => Map.GetLength(0);
    public int Height => Map.GetLength(1);

    public Vector2Int upStairsPos;
    public Vector2Int downStairsPos;

    private  HashSet<Entity>[,] _entityMap;


    public void InitMapData(TileType[,] baseData)
    {
        Map = baseData;
        _entityMap = new HashSet<Entity>[Width,Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _entityMap[x, y] = new HashSet<Entity>();
            }
        }
    }

    /// <summary>
    /// 指定の座標がマップ内かを取得する
    /// </summary>
    /// <returns>true = マップ内, false = マップ外</returns>
    public bool IsInsideMap(Vector2Int absPos) =>
        absPos.x >= 0 && absPos.x < Width && absPos.y >= 0 && absPos.y < Height;


    public TileType GetTileType(Vector2Int absPos) =>
        IsInsideMap(absPos) ? Map[absPos.x, absPos.y] : TileType.None;


    /// <summary>
    /// 指定の座標に存在するEntityを取得する
    /// </summary>
    public HashSet<Entity> GetEntities(Vector2Int absPos) =>
        IsInsideMap(absPos) ?  _entityMap[absPos.x, absPos.y] : null;



    public void AddEntity(Entity entity, Vector2Int absPos)
    {
        if (!IsInsideMap(absPos))
        {
            Debug.LogError($"マップ外には配置できません {entity.name} at {absPos}");
        }
        entity.OnSetPosition -= EntitySetPosition;
        entity.OnSetPosition += EntitySetPosition;
        _entityMap[absPos.x, absPos.y].Add(entity);

    }

    public void AddEntity(Entity entity) => AddEntity(entity, entity.Pos);


    /// <summary>
    /// _entityMapからEntityを削除する
    /// </summary>
    public void RemoveEntity(Entity entity ,Vector2Int absPos)
    {
        if (!IsInsideMap(absPos)) return;
        _entityMap[absPos.x, absPos.y].Remove(entity);
    }

    public void RemoveEntity(Entity entity) => RemoveEntity(entity, entity.Pos);

    /// <summary>
    /// _entityMapに存在するEntityを更新する
    /// </summary>
    public void MoveEntity(Entity entity, Vector2Int oldPos, Vector2Int newPos)
    {
        if (IsInsideMap(oldPos)) _entityMap[oldPos.x, oldPos.y].Remove(entity);
        if (IsInsideMap(newPos)) _entityMap[newPos.x, newPos.y].Add(entity);
    }

    public void EntitySetPosition(Entity entity) => MoveEntity(entity, entity.OldPos, entity.Pos);



    /// <summary>
    /// 指定の場所が移動可能かどうかを取得する
    /// </summary>
    public bool CanMove(Vector2Int absPos)
    {
        if (!IsFloor(absPos)) return false;
        var hasStatus = GetEntities(absPos).GetHasStatus();
        return hasStatus == null;
    } //=> IsFloor(absPos) && GetEntities(absPos).GetUnit() == null;

    public bool IsFloor(Vector2Int absPos) => IsInsideMap(absPos) && Map[absPos.x, absPos.y].CanMove();

    /// <summary>
    /// スポーン可能な座標のリストを返す
    /// </summary>
    public List<Vector2Int> GetCanSpawnPositions()
    {
        List<Vector2Int> emptyPositions = new();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector2Int pos = new(x,y);
                if (Map[x,y].CanSpawn() && _entityMap[x,y].Count == 0)
                {
                    emptyPositions.Add(pos);
                }
            }
        }

        return emptyPositions;
    }

}
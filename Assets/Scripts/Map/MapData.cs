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

    private Dictionary<Vector2Int, HashSet<Entity>> _entityMap = new();

    /// <summary>
    /// 指定の座標がマップ内かを取得する
    /// </summary>
    /// <returns>true = マップ内, false = マップ外</returns>
    public bool IsInsideMap(Vector2Int absPos) => absPos.x >= 0 && absPos.x < Width && absPos.y >= 0 && absPos.y < Height;


    /// <summary>
    /// _entityMapの参照エラーを防ぐ
    /// </summary>
    private void EnsureDictionaryAt(Vector2Int absPos)
    {
        if (!_entityMap.ContainsKey(absPos)) _entityMap[absPos] = new HashSet<Entity>();
    }

    public TileType GetTileType(Vector2Int absPos) => IsInsideMap(absPos) ? Map[absPos.x, absPos.y] : TileType.None;

    /// <summary>
    /// 指定の座標に存在するEntityを取得する
    /// </summary>
    public HashSet<Entity> GetEntities(Vector2Int absPos)
    {
        if (!IsInsideMap(absPos)) return null ;
        EnsureDictionaryAt(absPos);
        return _entityMap[absPos];
    }


    /// <summary>
    /// HashSet<Entiey>からUnitを取得する
    /// </summary>
    public Unit GetUnit(HashSet<Entity> entities) //todo HashSet<Entity>の拡張メソッドとして実装する?
    {
        if (entities != null && entities.Count > 0)
        {
            foreach (Entity entity in entities)
            {
                if (entity is Unit unit) return unit;
            }
        }
        return null;
    }

    /// <summary>
    /// 指定の座標に存在するUnitを取得する
    /// </summary>
    public Unit GetUnit(Vector2Int absPos) => GetUnit(GetEntities(absPos));


    public void AddEntity(Entity entity, Vector2Int absPos)
    {
        if (!IsInsideMap(absPos) || !GetTileType(absPos).CanMove())
        {
            Debug.LogError($"{this} : 配置に失敗しました {entity},{absPos}");
        }
        EnsureDictionaryAt(absPos);
        _entityMap[absPos].Add(entity);
    }

    public void AddEntity(Entity entity) => AddEntity(entity, entity.Pos);


    /// <summary>
    /// _entityMapからEntityを削除する
    /// </summary>
    public void RemoveEntity(Entity entity)
    {
        EnsureDictionaryAt(entity.Pos);
        _entityMap[entity.Pos]?.Remove(entity);
    }

    /// <summary>
    /// _entityMapに存在するEntityを更新する
    /// </summary>
    public void MoveEntity(Entity entity, Vector2Int oldPos, Vector2Int newPos)
    {
        if(_entityMap.TryGetValue(oldPos, out var set)) 
        {
            set.Remove(entity);
        }
        EnsureDictionaryAt(newPos);
        _entityMap[newPos].Add(entity);
    }

    /// <summary>
    /// 指定の座標に操作した場合の行動を返す
    /// </summary>
    /// <param name="pos"></param>
    public InteractType InteractCell(Vector2Int pos) //todo HashSet<Entity>を返すだけでいいかも知れない
    {
        if(!IsInsideMap(pos)) return InteractType.None;

        HashSet<Entity> entities = GetEntities(pos);
        if(GetUnit(entities) != null) return InteractType.Unit;


        TileType tile = GetTileType(pos);

        if (tile.CanMove()) return InteractType.Move;

        return InteractType.None;
    }

    /// <summary>
    /// 指定の場所が移動可能かどうかを取得する
    /// </summary>
    public bool CanMove(Vector2Int absPos) => IsFloor(absPos) && GetUnit(absPos) == null;

    public bool IsFloor(Vector2Int pos) => IsInsideMap(pos) && GetTileType(pos).CanMove();

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
                if (Map[x,y].CanSpawn() && GetEntities(pos).Count == 0)
                {
                    emptyPositions.Add(pos);
                }
            }
        }

        return emptyPositions;
    }

}
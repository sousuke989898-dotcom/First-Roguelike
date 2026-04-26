using System.Collections.Generic;
using UnityEngine;

public enum InteractResult {Move, Unit, Entity, Open, None};

public enum TileType{Wall, Floor, UpStairs, DownStairs, Door, Road, None}

public static class TileTypeExtensions
{

    public static bool CanMove(this TileType tile)
    {
        return tile switch
        {
            TileType.Wall or TileType.None => false,
            _ => true,
        };
    }

    public static bool CanSpawn(this TileType tile)
    {
        return tile switch
        {
            TileType.Floor => true,
            _ => false,
        };
    }
}

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
    /// <param name="pos">マップ内の絶対座標</param>
    /// <returns>true = マップ内, false = マップ外</returns>
    public bool IsInsideMap(Vector2Int pos) => pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;


    /// <summary>
    /// _entityMapの参照エラーを防ぐ
    /// </summary>
    /// <param name="pos">マップ内の絶対座標</param>
    private void EnsureDictionaryAt(Vector2Int pos)
    {
        if (!_entityMap.ContainsKey(pos)) _entityMap[pos] = new HashSet<Entity>();
    }

    /// <summary>
    /// 指定の座標のTileTypeを取得する
    /// </summary>
    /// <param name="pos">マップ内の絶対座標</param>
    public TileType GetTile(Vector2Int pos) => (!IsInsideMap(pos)) ? TileType.None : Map[pos.x, pos.y];

    /// <summary>
    /// 指定の座標に存在するEntityを取得する
    /// </summary>
    /// <param name="pos">マップ内の絶対座標</param>
    public HashSet<Entity> GetEntities(Vector2Int pos)
    {
        if (!IsInsideMap(pos)) return null ;
        EnsureDictionaryAt(pos);
        return _entityMap[pos];
    }


    /// <summary>
    /// HashSet<Entiey>からUnitを取得する
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public Unit GetUnit(HashSet<Entity> entities)
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
    /// <param name="pos">マップ内の絶対座標</param>
    public Unit GetUnit(Vector2Int pos) => GetUnit(GetEntities(pos));


    /// <summary>
    /// _entityMapにEntityを追加する
    /// </summary>
    public void AddEntity(Entity entity)
    {
        AddEntity(entity, entity.Pos);
    }

    public void AddEntity(Entity entity, Vector2Int pos)
    {
        if (!IsInsideMap(pos) || !GetTile(pos).CanMove())
        {
            Debug.LogError($"{this} : 配置に失敗しました {entity},{pos}");
        }
        EnsureDictionaryAt(pos);
        _entityMap[pos].Add(entity);
    }

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
    /// <param name="entity"></param>
    /// <param name="oldPos"></param>
    /// <param name="newPos"></param>
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
    public InteractResult InteractCell(Vector2Int pos)
    {
        if(!IsInsideMap(pos)) return InteractResult.None;

        HashSet<Entity> entities = GetEntities(pos);
        if(GetUnit(entities) != null) return InteractResult.Unit;


        TileType tile = GetTile(pos);

        if (tile.CanMove()) return InteractResult.Move;

        return InteractResult.None;
    }

    /// <summary>
    /// 指定の場所が移動可能かどうかを取得する
    /// </summary>
    /// <param name="pos">マップ内の絶対座標</param>
    public bool CanMove(Vector2Int pos) => IsFloor(pos) && GetUnit(pos) == null;

    public bool IsFloor(Vector2Int pos) => IsInsideMap(pos) && GetTile(pos).CanMove();
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
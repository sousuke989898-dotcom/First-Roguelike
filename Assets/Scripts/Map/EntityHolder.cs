using System.Collections.Generic;
using UnityEngine;

namespace Game.GridMap
{
    public class EntityHolder
    {
        public List<Entity>[,] EntityMap {get; private set;}
        public List<Entity> Entities {get; private set;}

        public int Width  => EntityMap.GetLength(0);
        public int Height => EntityMap.GetLength(1);

        public EntityHolder(int width , int height)
        {
            EntityMap = new List<Entity>[width, height];
            Entities = new();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    EntityMap[x,y] = new List<Entity>();
                }
            }
        }

        public bool IsInsideMap(Vector2Int absPos) =>
            absPos.x >= 0 && absPos.x < Width && absPos.y >= 0 && absPos.y < Height;

        /// <summary>
        /// 指定の絶対座標に存在する全てのEntityを取得する
        /// </summary>
        public List<Entity> GetEntities(Vector2Int absPos) =>
            IsInsideMap(absPos) ?  EntityMap[absPos.x, absPos.y] : null;

        /// <summary>
        /// 絶対座標からUnitを取得する
        /// </summary>
        public Unit GetUnit(Vector2Int absPos) => GetEntities(absPos)?.GetUnit();


        public bool AddEntity(Entity entity)
        {
            if (!IsInsideMap(entity.Pos))
            {
                Debug.LogError($"[EntityHolder] マップ外には配置できません: {entity.name} at {entity.Pos}");
                return false;
            }

            if (Entities.Contains(entity)) return false;

            Entities.Add(entity);
            EntityMap[entity.Pos.x, entity.Pos.y].Add(entity);

            RemoveAction(entity);
            AddAction(entity);

            return true;

        }

        

        /// <summary>
        /// EntityMapに存在するEntityを更新する
        /// </summary>
        public void UpdateEntityPos(Entity entity, Vector2Int oldPos, Vector2Int newPos)
        {
            if (IsInsideMap(oldPos)) EntityMap[oldPos.x, oldPos.y].Remove(entity);
            if (IsInsideMap(newPos)) EntityMap[newPos.x, newPos.y].Add(entity);
        }

        /// <summary>
        /// EntityMapに存在するEntityを更新する
        /// </summary>
        /// <remarks>Entityの位置情報（Pos/OldPos）が更新された後に呼び出す必要があります。</remarks>
        public void UpdateEntityPos(Entity entity) => UpdateEntityPos(entity, entity.OldPos, entity.Pos);

        /// <summary>
        /// Entityを削除する
        /// </summary>
        public void RemoveEntity(Entity entity ,Vector2Int absPos)
        {
            if (!IsInsideMap(absPos)) return;
            EntityMap[absPos.x, absPos.y].Remove(entity);
            Entities.Remove(entity);
            RemoveAction(entity);
        }

        /// <summary>
        /// Entityを削除する
        /// </summary>
        public void RemoveEntity(Entity entity) => RemoveEntity(entity, entity.Pos);



        private void RemoveAction(Entity entity)
        {
            entity.OnSetPosition -= UpdateEntityPos;
            entity.OnDisposeEntity -= RemoveEntity;
        }

        private void AddAction(Entity entity)
        {
            entity.OnSetPosition += UpdateEntityPos;
            entity.OnDisposeEntity += RemoveEntity;
        }


    }
}
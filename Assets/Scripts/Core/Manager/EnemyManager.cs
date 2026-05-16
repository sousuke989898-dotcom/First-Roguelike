using System.Collections.Generic;
using Game.Manager;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Enemy EnemyPrefab;

    public static EnemyManager Instance;
    public HashSet<Enemy> Enemies{get; private set;}

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            enabled = false;
            Debug.LogError($"{this}が複数存在しています。");
        }
        Enemies = new();
    }


    public void AddEnemy(Enemy enemy)
    {
        Enemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (Enemies.Contains(enemy)) Enemies.Remove(enemy);
    }

    /// <summary>
    /// エネミーを出現させる
    /// </summary>
    /// <param name="pos">出現させる位置の絶対座標</param>
    public void SpawnEnemy(Vector2Int pos)
    {
        Enemy enemy = Instantiate(EnemyPrefab);
        UnitData unitData = DatabaseManager.Units.Get("Enemy");
        enemy.InitUnit(unitData, pos);
        Enemies.Add(enemy);
    }

    /// <summary>
    /// ランダムな位置にエネミーを出現させる
    /// </summary>
    public void SpawnEnemy()
    {
        SpawnEnemy(MapManager.Instance.GetSpawnPos());
    }
}
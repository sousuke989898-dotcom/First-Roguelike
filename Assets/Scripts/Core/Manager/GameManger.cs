using Game.Manager;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player PlayerPrefab;

    public static GameManager Instance {get; private set;}

    private Player _player;

    public static Player Player => Instance._player;



    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            enabled = false;
            Debug.LogError($"{this}が複数存在しています。");
        }
        _player = Instantiate(PlayerPrefab);
        // UnityEngine.Random.InitState(0);
    }

    void Start()
    {
        UnitData data = DatabaseManager.Units.Get("Player");
        Player.InitUnit(data,MapManager.Instance.GetSpawnPos());
        EnemyManager.Instance.SpawnEnemy();

        TurnManager.Instance.StartCoroutine(TurnManager.Instance.StartRoutine());

        // Vector2Int start = MapManager.Instance.GetSpawnPos();
        // Vector2Int end = MapManager.Instance.GetSpawnPos();
        // var path = PathFinder.GetPath(start, end);
        // if (path != null)
        // {
        //     string pathString = string.Join(", ", path);
        //     Debug.Log($"Start: {start} End: {end} / ルート成功: [{pathString}]");
        // }
        // else
        // {
        //     Debug.Log($"Start: {start} End: {end} / ルートが見つかりませんでした");
        // }
    }
}




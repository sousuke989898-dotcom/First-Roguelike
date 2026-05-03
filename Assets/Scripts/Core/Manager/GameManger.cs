using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player PlayerPrefab;

    public static GameManager Instance {get; private set;}

    public Player CurrentPlayer {get; private set;}


    void Awake()
    {
        if (Instance != null) Instance = this;
        else
        {
            enabled = false;
            Debug.LogError($"{this}が複数存在しています。");
        }
        CurrentPlayer = Instantiate(PlayerPrefab);
        // UnityEngine.Random.InitState(0);
    }

    void Start()
    {
        CurrentPlayer.InitPlayer(MapManager.Instance.GetSpawnPos());
        EnemyManager.Instance.SpawnEnemy();

    }

}




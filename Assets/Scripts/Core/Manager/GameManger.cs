using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player PlayerPrefab;

    public static GameManager Instance {get; private set;}

    public Player CurrentPlayer {get; private set;}


    void Awake()
    {
        Instance = this;
        CurrentPlayer = Instantiate(PlayerPrefab);
        // UnityEngine.Random.InitState(0);
    }

    void Start()
    {
        CurrentPlayer.InitPlayer(MapManager.Instance.GetSpawnPos());
        EnemyManager.Instance.SpawnEnemy();

    }

}




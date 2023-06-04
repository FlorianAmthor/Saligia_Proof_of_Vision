using SaligiaProofOfVision;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public IReadOnlyList<EnemyUnit> Enemies => _enemies;
    private List<EnemyUnit> _enemies;

    [field: SerializeField] public EnemyScalingSO EnemyScaling { get; private set; }

    private Player _player;
    public int BossesSpawned { get; private set; } = 0;

    private List<EnemyUnit> _rootedEnemies;
    private float _rootDurationForList;
    private float _timeRooted;

    private void OnEnable()
    {
        GameEvents.resetGameEvent += OnResetGame;
        GameEvents.trashEnemyDeathEvent += OnTrashEnemyDeath;
        GameEvents.bossEnemyDeathEvent += OnBossEnemyDeath;
    }

    private void OnDisable()
    {
        GameEvents.resetGameEvent -= OnResetGame;
        GameEvents.trashEnemyDeathEvent -= OnTrashEnemyDeath;
        GameEvents.bossEnemyDeathEvent -= OnBossEnemyDeath;
    }

    private void Start()
    {
        _enemies = new List<EnemyUnit>();
        var tempEnemis = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in tempEnemis)
            _enemies.Add(enemy.GetComponent<EnemyUnit>());

        _rootedEnemies = new List<EnemyUnit>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        EnemyScaling.currentGluttonyEnemyCount = 0;
        EnemyScaling.currentPrideEnemyCount = 0;
        EnemyScaling.currentWrathEnemyCount = 0;
    }

    private void Update()
    {
        if (_rootedEnemies.Count > 0)
        {
            if (_timeRooted + _rootDurationForList <= Time.time)
            {
                _rootDurationForList = 0;
                foreach (var enemy in _rootedEnemies)
                    enemy.GetComponent<EnemyController>().navmeshAgent.enabled = false;
                _rootedEnemies.Clear();
            }
        }
    }

    public EnemyUnit GetNearestEnemy(EnemyUnit enemy)
    {
        EnemyUnit nearestEnemy = null;
        float distance = float.MaxValue;

        foreach (var e in _enemies)
        {
            if (e == nearestEnemy || e.CurrentHealth <= 0)
                continue;
            var newDistance = Vector3.Distance(_player.transform.position, e.transform.position);
            if (nearestEnemy == null)
            {
                nearestEnemy = e;
                distance = newDistance;
                continue;
            }
            if (newDistance < distance)
            {
                nearestEnemy = e;
                distance = newDistance;
            }
        }
        return nearestEnemy;
    }

    public void RootEnemies(List<EnemyUnit> enemies, float rootDuration)
    {
        _rootDurationForList = rootDuration;
        _rootedEnemies = enemies;
        foreach (var enemy in _rootedEnemies)
            enemy.GetComponent<EnemyController>().navmeshAgent.enabled = true;
        _timeRooted = Time.time;
    }

    public EnemyUnit SpawnEnemy(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var enemy = Instantiate(prefab, position, rotation).GetComponent<EnemyUnit>();
        enemy.transform.parent = transform;
        _enemies.Add(enemy);
        return enemy;
    }

    private void OnBossEnemyDeath(EnemyUnit enemy)
    {
        _enemies.Remove(enemy);
        _player.RestoreRessources();

        foreach (var item in _enemies)
        {
            item.UpdateBonus();
        }
    }

    private void OnTrashEnemyDeath(EnemyUnit enemy)
    {
        _enemies.Remove(enemy);

        foreach (var item in _enemies)
        {
            item.UpdateBonus();
        }
    }

    //TODO: Restart scene so the monster in the scene would be spawned at the correct place again
    private void OnResetGame()
    {
        foreach (var enemy in _enemies)
            Destroy(enemy.gameObject);
        _enemies.Clear();
        _rootedEnemies.Clear();
    }
}

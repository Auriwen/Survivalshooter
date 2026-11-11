using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;           // one per gate

    [Header("Wave settings")]
    [SerializeField] private int startEnemyCount = 5;           // enemies in wave 1
    [SerializeField] private int extraEnemiesPerWave = 2;       // +2 each wave
    [SerializeField] private float baseEnemySpeed = 2f;         // speed in wave 1
    [SerializeField] private float speedIncreasePerWave = 0.1f; // +10% each wave
    [SerializeField] private float timeBetweenSpawns = 0.5f;    // gap between enemies
    [SerializeField] private float timeBetweenWaves = 3f;       // break after a wave
    [SerializeField] private float spawnSpread = 0.4f;          // random offset so enemies don't stack
     [SerializeField] private int baseScorePerKill = 10;      // points per kill in wave 1
    [SerializeField] private int extraScorePerWave = 5;      // +5 points per kill each wave
    public int CurrentWave { get; private set; } = 0;

    private readonly List<Enemy> aliveEnemies = new List<Enemy>();

    void Start()
    {
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        while (!GameManager.Instance.IsGameOver)
        {
            CurrentWave++;

            int count = startEnemyCount + (CurrentWave - 1) * extraEnemiesPerWave;
            float speed = baseEnemySpeed * (1f + (CurrentWave - 1) * speedIncreasePerWave);
            Debug.Log($"Wave {CurrentWave}: {count} enemies, speed {speed:F2}");

            // 1. Spawn the enemies one by one
            for (int i = 0; i < count; i++)
            {
                if (GameManager.Instance.IsGameOver) yield break;  // stop mid-wave
                SpawnEnemy(speed);
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            // 2. Wait until every enemy of this wave is dead
            yield return new WaitUntil(AllEnemiesDead);

            // 3. Short break, then the next wave
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    bool AllEnemiesDead()
    {
        // Destroyed enemies count as "null", so remove them from the list
        aliveEnemies.RemoveAll(e => e == null);
        return aliveEnemies.Count == 0;
    }

    void SpawnEnemy(float speed)
    {
        Vector2 position = GetSpawnPosition();
        Enemy enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        enemy.SetMoveSpeed(speed);
        enemy.SetScoreValue(baseScorePerKill + (CurrentWave - 1) * extraScorePerWave);
        aliveEnemies.Add(enemy);
    }

    // Pick a random gate, plus a small random offset
    Vector2 GetSpawnPosition()
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * spawnSpread;
        return (Vector2)point.position + offset;
    }
       
}
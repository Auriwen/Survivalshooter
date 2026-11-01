using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Enemy enemyPrefab;

    [Header("Wave settings")]
    [SerializeField] private int startEnemyCount = 5;        // enemies in wave 1
    [SerializeField] private int extraEnemiesPerWave = 2;    // +2 each wave
    [SerializeField] private float baseEnemySpeed = 2f;      // speed in wave 1
    [SerializeField] private float speedIncreasePerWave = 0.1f; // +10% each wave
    [SerializeField] private float timeBetweenSpawns = 0.5f; // gap between enemies
    [SerializeField] private float timeBetweenWaves = 3f;    // break after a wave
    [SerializeField] private float spawnMargin = 1f;         // how far off-screen

    public int CurrentWave { get; private set; } = 0;

    private readonly List<Enemy> aliveEnemies = new List<Enemy>();
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        StartCoroutine(RunWaves());
    }

    // A coroutine can pause with "yield" and continue later,
    // which makes timed sequences like waves easy to write
    IEnumerator RunWaves()
    {
        while (true)
        {
            CurrentWave++;

            int count = startEnemyCount + (CurrentWave - 1) * extraEnemiesPerWave;
            float speed = baseEnemySpeed * (1f + (CurrentWave - 1) * speedIncreasePerWave);
            Debug.Log($"Wave {CurrentWave}: {count} enemies, speed {speed:F2}");

            // 1. Spawn the enemies one by one
            for (int i = 0; i < count; i++)
            {
                SpawnEnemy(speed);
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            // 2. Wait until every enemy of this wave is dead
            yield return new WaitUntil(AllEnemiesDead);

            // 3. Short break, then the loop starts the next wave
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    bool AllEnemiesDead()
    {
        // Destroyed enemies count as "null" in Unity, so remove them from the list
        aliveEnemies.RemoveAll(e => e == null);
        return aliveEnemies.Count == 0;
    }

    void SpawnEnemy(float speed)
    {
        Vector2 position = GetSpawnPosition();
        Enemy enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        enemy.SetMoveSpeed(speed);
        aliveEnemies.Add(enemy);
    }

    // Picks a random point on a circle just outside the visible screen
    Vector2 GetSpawnPosition()
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        float radius = Mathf.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight) + spawnMargin;

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 center = cam.transform.position;
        return center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
    }
}
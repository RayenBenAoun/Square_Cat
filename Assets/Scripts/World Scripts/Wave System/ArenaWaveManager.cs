using UnityEngine;
using System;

public class ArenaWaveManager : MonoBehaviour
{
    public static ArenaWaveManager Instance;

    public event Action OnWaveStarted;
    public event Action OnWaveEnded;

    public int currentWave = 0;
    public static int AliveEnemies = 0;

    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;

    void Awake()
    {
        Instance = this;
    }

    public void StartWave()
    {
        currentWave++;

        Debug.Log("Starting Wave " + currentWave);

        OnWaveStarted?.Invoke();   // <-- THIS FIRES!

        int enemyCount = 3 + currentWave;
        AliveEnemies = enemyCount;

        SpawnEnemies(enemyCount);
    }

    void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int enemyIndex = UnityEngine.Random.Range(0, enemyPrefabs.Length);
            int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);

            GameObject e = Instantiate(enemyPrefabs[enemyIndex], spawnPoints[spawnIndex].position, Quaternion.identity);

            EnemyHealth h = e.GetComponentInChildren<EnemyHealth>();
            h.OnEnemyDied += OnEnemyKilled;
        }
    }

    void OnEnemyKilled()
    {
        AliveEnemies--;

        if (AliveEnemies <= 0)
        {
            Debug.Log("Wave Completed");
            OnWaveEnded?.Invoke(); // <-- THIS FIRES!
        }
    }
}

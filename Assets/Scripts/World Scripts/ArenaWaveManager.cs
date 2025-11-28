using UnityEngine;
using System.Collections.Generic;

public class ArenaWaveManager : MonoBehaviour
{
    public static ArenaWaveManager Instance;

    [Header("Spawning")]
    public Transform[] spawnPoints;
    public GameObject[] enemyPrefabs;

    [Header("Wave Stats")]
    public int currentWave = 0;
    public bool waveActive = false;

    private List<GameObject> activeEnemies = new();

    private void Awake()
    {
        Instance = this;
    }

    public void StartWave()
    {
        Debug.Log("🔥 WAVE START INITIATED");

        if (waveActive)
        {
            Debug.Log("❗ Tried to start wave but it's already active");
            return;
        }

        if (spawnPoints.Length == 0)
            Debug.LogError("❗ ERROR: No spawn points assigned!");

        if (enemyPrefabs.Length == 0)
            Debug.LogError("❗ ERROR: No enemy prefabs assigned!");

        currentWave++;
        waveActive = true;

        int enemyAmount = 3 + currentWave * 2;
        Debug.Log("👹 Spawning " + enemyAmount + " enemies");

        for (int i = 0; i < enemyAmount; i++)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            Debug.Log("⚠️ Instantiating enemy: " + prefab.name + " at " + spawn.position);

            GameObject enemy = Instantiate(prefab, spawn.position, Quaternion.identity);
            activeEnemies.Add(enemy);
        }
    }

    private void EnemyDeathRegister(EnemyHealth e)
    {
        e.OnEnemyDied += HandleEnemyDeath;
    }

    private void HandleEnemyDeath(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
            activeEnemies.Remove(enemy);

        Debug.Log($"☠ Enemy died — {activeEnemies.Count} remaining.");

        if (activeEnemies.Count == 0)
            EndWave();
    }

    private void EndWave()
    {
        Debug.Log($"🏁 WAVE {currentWave} COMPLETE!");

        waveActive = false;

        // Notify other systems
        OnWaveCompleted?.Invoke();
    }

    public delegate void WaveCompleted();
    public static event WaveCompleted OnWaveCompleted;
}

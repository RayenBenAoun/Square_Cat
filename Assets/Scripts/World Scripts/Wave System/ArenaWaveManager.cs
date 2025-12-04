using UnityEngine;
using System.Collections.Generic;

public class ArenaWaveManager : MonoBehaviour
{
    public static ArenaWaveManager Instance;

    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;

    public int currentWave = 0;
    public static int AliveEnemies = 0;

    public event System.Action OnWaveStarted;
    public event System.Action OnWaveEnded;

    void Awake()
    {
        Instance = this;
    }

    List<int[]> waves = new List<int[]>
    {
        new int[] { 25, 26, 27, 28 },
        new int[] { 39, 40, 44, 45 },
        new int[] { 40, 41, 42, 43 },
        new int[] { 5, 6, 7, 8, 9 },
        new int[] { 0, 1 },
        new int[] { 51, 52 },
        new int[] { 31, 32, 33 },
        new int[] { 29, 30 },
        new int[] { 11,12,13,14,15,16,17,18,19,20,21,22,23,24 },
        new int[] { 10 }
    };

    public void StartWave()
    {
        if (currentWave >= waves.Count) return;

        int[] waveEnemies = waves[currentWave];
        AliveEnemies = waveEnemies.Length;

        currentWave++;

        OnWaveStarted?.Invoke();

        foreach (int index in waveEnemies)
        {
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefabs[index], sp.position, Quaternion.identity);
        }
    }

    public void EnemyDied()
    {
        AliveEnemies--;

        if (AliveEnemies <= 0)
        {
            AliveEnemies = 0;
            OnWaveEnded?.Invoke();
        }
    }

    // ======================================================
    // RESET THE CURRENT WAVE WITHOUT CHANGING WAVE NUMBER
    // ======================================================
    public void ResetCurrentWave()
    {
        // Remove all enemies
        foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
            Destroy(e);

        // Kill bosses
        foreach (var d in FindObjectsOfType<DragonAI>())
            Destroy(d.gameObject);

        AliveEnemies = 0;

        // OPEN WALL
        OnWaveEnded?.Invoke();

        // ↓ Set wave back one step so StartWave replays same wave
        currentWave = Mathf.Max(0, currentWave - 1);

        // Reactivate start button
        WaveStarter starter = FindAnyObjectByType<WaveStarter>(FindObjectsInactive.Include);
        if (starter != null)
            starter.ResetStarter();

        // Restart same wave
        StartWave();
    }
}

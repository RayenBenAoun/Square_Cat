using UnityEngine;
using System.Collections.Generic;

public class ArenaWaveManager : MonoBehaviour
{
    public static ArenaWaveManager Instance;

    [Header("Assign ALL Enemy Prefabs In Order")]
    public GameObject[] enemyPrefabs;   // Size 58 in your case

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    public int currentWave = 0;
    public static int AliveEnemies = 0;

    public event System.Action OnWaveStarted;
    public event System.Action OnWaveEnded;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Optionally auto-start the first wave here
        // StartWave();
    }

    // ---------------------------------------------------------
    //  DEFINE WAVES USING YOUR NEW ARRAY INDEXES (0–57)
    // ---------------------------------------------------------
    List<int[]> waves = new List<int[]>
    {
        // -----------------------
        // 🧟 WAVE 1 – BASIC GOBLINS
        // -----------------------
        new int[] { 25, 26, 27, 28 },  

        // -----------------------
        // 🧟‍♂️ WAVE 2 – BASIC SKELETONS
        // -----------------------
        new int[] { 39, 40, 44, 45 },

        // -----------------------
        // 🏹 WAVE 3 – RANGED SKELETONS
        // -----------------------
        new int[] { 40, 41, 42, 43 },

        // -----------------------
        // 🐺 WAVE 4 – CANINES
        // -----------------------
        new int[] { 5, 6, 7, 8, 9 },   

        // -----------------------
        // 🐻 WAVE 5 – BEARS
        // -----------------------
        new int[] { 0, 1 },

        // -----------------------
        // 🐍 WAVE 6 – SNAKES
        // -----------------------
        new int[] { 51, 52 },

        // -----------------------
        // 🕷️ WAVE 7 – SPIDERS
        // -----------------------
        new int[] { 31, 32, 33 },

        // -----------------------
        // 🧱 WAVE 8 – SLIMES
        // -----------------------
        new int[] { 29, 30 },

        // -----------------------
        // 🌪️ WAVE 9 – ELEMENTALS
        // -----------------------
        new int[]
        {
            11,12,13,14,15,16,17,18,19,20,21,22,23,24
        },

        // -----------------------
        // 🐉 WAVE 10 – DRAGON BOSS
        // -----------------------
        new int[] { 10 },  

        // -----------------------
        // OR CONTINUE ADDING WAVES…
        // -----------------------
    };

    // ---------------------------------------------------------
    //  START A WAVE
    // ---------------------------------------------------------
    public void StartWave()
    {
        if (currentWave >= waves.Count)
        {
            Debug.Log("All waves complete!");
            return;
        }

        int[] waveEnemies = waves[currentWave];
        AliveEnemies = waveEnemies.Length;

        OnWaveStarted?.Invoke();

        foreach (int index in waveEnemies)
        {
            SpawnEnemy(enemyPrefabs[index]);
        }

        currentWave++;
    }

    // ---------------------------------------------------------
    //  SPAWN ENEMY
    // ---------------------------------------------------------
    void SpawnEnemy(GameObject prefab)
    {
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(prefab, sp.position, Quaternion.identity);
    }

    // ---------------------------------------------------------
    //  CALLED BY ENEMY ON DEATH
    // ---------------------------------------------------------
    public void EnemyDied()
    {
        AliveEnemies--;

        if (AliveEnemies <= 0)
        {
            OnWaveEnded?.Invoke();
        }
    }
}

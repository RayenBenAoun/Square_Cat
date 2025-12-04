using UnityEngine;
using TMPro;

public class UiWaveCounter : MonoBehaviour
{
    public static UiWaveCounter Instance;

    [SerializeField] TMP_Text waveText;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        ArenaWaveManager.Instance.OnWaveStarted += UpdateCounter;
    }

    void OnDisable()
    {
        if (ArenaWaveManager.Instance != null)
            ArenaWaveManager.Instance.OnWaveStarted -= UpdateCounter;
    }

    void Start()
    {
        waveText.text = "Wave 0";
    }

    // MUST MATCH: Action → void UpdateCounter()
    void UpdateCounter()
    {
        waveText.text = "Wave " + ArenaWaveManager.Instance.currentWave;
    }
}

using UnityEngine;

public class ArenaGateTrigger : MonoBehaviour
{
    private bool hasTriggered = false;
    private ArenaWaveManager waveManager;

    private void Start()
    {
        waveManager = FindObjectOfType<ArenaWaveManager>();
        if (waveManager == null)
            Debug.LogError("❗ ArenaWaveManager not found in scene");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        Debug.Log("🔥 Player entered arena — starting wave");
        hasTriggered = true;
        waveManager.StartWave();
    }
}

using UnityEngine;
using TMPro;

public class WaveStarter : MonoBehaviour
{
    public TextMeshProUGUI prompt;
    public GameObject statueSprite;

    private Collider2D col;
    private bool playerInside = false;

    private void Start()
    {
        col = GetComponent<Collider2D>();

        prompt.gameObject.SetActive(false);
        statueSprite.SetActive(true);
        col.enabled = true;

        // 🔥 THE FIX: Subscribe HERE, not Awake()
        if (ArenaWaveManager.Instance != null)
        {
            Debug.Log("WaveStarter subscribed to events.");
            ArenaWaveManager.Instance.OnWaveStarted += HideInteractable;
            ArenaWaveManager.Instance.OnWaveEnded += ShowInteractable;
        }
        else
        {
            Debug.LogError("ArenaWaveManager.Instance is NULL in Start()");
        }
    }

    private void OnDestroy()
    {
        if (ArenaWaveManager.Instance != null)
        {
            ArenaWaveManager.Instance.OnWaveStarted -= HideInteractable;
            ArenaWaveManager.Instance.OnWaveEnded -= ShowInteractable;
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.X))
        {
            HideInteractable();
            ArenaWaveManager.Instance.StartWave();
        }
    }

    void HideInteractable()
    {
        Debug.Log("Hiding interactable...");
        statueSprite.SetActive(false);
        col.enabled = false;
        prompt.gameObject.SetActive(false);
    }

    void ShowInteractable()
    {
        Debug.Log("SHOWING interactable!");
        statueSprite.SetActive(true);
        col.enabled = true;

        if (playerInside)
            prompt.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        playerInside = true;
        if (statueSprite.activeSelf)
            prompt.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        playerInside = false;
        prompt.gameObject.SetActive(false);
    }
    public void ResetStarter()
    {
        gameObject.SetActive(true);
    }

}

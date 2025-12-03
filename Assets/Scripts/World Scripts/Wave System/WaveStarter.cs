using UnityEngine;
using TMPro;

public class WaveStarter : MonoBehaviour
{
    public TextMeshProUGUI prompt;
    private bool playerInside = false;


    private void Start()
    {
        if (prompt != null)
            prompt.gameObject.SetActive(false);

        ArenaWaveManager.Instance.OnWaveEnded += ReEnableStatue;
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Wave Start Triggered");
            prompt.gameObject.SetActive(false);

            // hide this object
            gameObject.SetActive(false);

            ArenaWaveManager.Instance.StartWave();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInside = true;
            prompt.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInside = false;
            prompt.gameObject.SetActive(false);
        }
    }

    void ReEnableStatue()
    {
        gameObject.SetActive(true);
    }
}

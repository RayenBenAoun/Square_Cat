using UnityEngine;

public class BlacksmithInteraction : MonoBehaviour
{
    public GameObject interactPrompt; // UI text "Press X"
    public UpgradeMenuUI menu; // reference to upgrade menu

    private bool playerInRange = false;

    void Start()
    {
        interactPrompt.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.X))
        {
            OpenMenu();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactPrompt.SetActive(false);
        }
    }

    private void OpenMenu()
    {
        interactPrompt.SetActive(false);
        menu.OpenMainMenu();
    }
}

using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public GameObject popup;  // Assign your popup panel here

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            popup.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            popup.SetActive(false);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class TutorialDrawTrigger : MonoBehaviour
{
    public GameObject popup;                     // The UI popup
    public Slider drawSlider;      // Reference to your draw bar script

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !triggered)
        {
            popup.SetActive(true);
            triggered = true;
        }
    }

    private void Update()
    {
        if (!triggered) return;

        // When the bar reaches 0, hide popup and disable trigger
        if (drawSlider.value <= 0.5f)
        {
            popup.SetActive(false);
            gameObject.SetActive(false); // Optional: permanently disable trigger
        }
    }
}
